using System;
using System.Collections;
using KindMen.Uxios.Errors;
using KindMen.Uxios.ExpectedTypesOfResponse;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using RSG;
using UnityEngine;
using UnityEngine.Networking;
using DataProcessingError = KindMen.Uxios.Errors.DataProcessingError;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios
{
    internal sealed class WebRequestRunner : MonoBehaviour, IRequestRunner
    {
        private static WebRequestRunner webRequestRunner;

        public static WebRequestRunner Instance()
        {
            if (webRequestRunner != null) return webRequestRunner;

            var gameObject = new GameObject
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            DontDestroyOnLoad(gameObject);

            webRequestRunner = gameObject.AddComponent<WebRequestRunner>();

            return webRequestRunner;
        }

        public Promise<Response> PerformRequest<TData>(Config config) where TData : class
        {
            var promise = new Promise<Response>();
            
            // We don't need to store the coroutine reference because the cancellation token in the 
            // config will self-abort the coroutine if it needs to be cancelled
            StartCoroutine(DoRequest<TData>(config, promise));

            return promise;
        }

        private UnityWebRequest ConvertToUnityWebRequest(Config config, byte[] bytes)
        {
            // Attach all params to the URL for UnityWebRequest
            var url = new UriBuilder(config.Url) { Query = config.Params.ToString() }.Uri;

            var webRequest = new UnityWebRequest(url, config.Method.ToString());
            webRequest.timeout = config.Timeout;
            webRequest.redirectLimit = config.MaxRedirects;
            webRequest.downloadHandler = config.TypeOfResponseType switch
            {
                TextureResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                _ => new DownloadHandlerBuffer()
            };
            webRequest.uploadHandler = new UploadHandlerRaw(bytes ?? new byte[]{});

            foreach (var header in config.Headers)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }

            return webRequest;
        }

        private IEnumerator DoRequest<TData>(Config config, Promise<Response> promise) where TData : class
        {
            // Clone config so that any changes we do - do not affect the caller; as this may mess with the global
            // config if that is used
            config = config.Clone() as Config;

            var request = ConvertToUnityWebRequest(
                config, 
                NormalizeConfigAndReturnByteArray<TData>(config)
            );

            foreach (var requestInterceptor in Uxios.Interceptors.request)
            {
                config = requestInterceptor.success.Invoke(config);
            }

            request.SendWebRequest();
            while (!request.isDone) 
            {
                if (config.CancelToken.IsCancellationRequested)
                {
                    // Abort the request and continue in the loop, abort attempts to finish
                    // as soon as possible, but may not be immediate
                    request.Abort();
                }

                yield return null;
            }

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError)
            {
                RejectRequest(config, promise, request);
                yield break;
            }

            // If an exception occurs in the whole response interpretation chain, reject the promise
            try
            {
                Response response = new UnityWebResponse(config, request);

                if (response.IsValid() == false)
                {
                    RejectResponse(promise, response);
                    yield break;
                }

                foreach (var responseInterceptor in Uxios.Interceptors.response)
                {
                    response = responseInterceptor.success.Invoke(response);
                }
                
                promise.Resolve(response);
            }
            catch (Exception e)
            {
                RejectWithError(promise, new Error(e.Message, config, null));
            }
        }

        private static void RejectRequest(Config config, Promise<Response> promise, UnityWebRequest request)
        {
            RejectWithError(
                promise, 
                request.result switch
                {
                    UnityWebRequest.Result.DataProcessingError => new DataProcessingError(request.error, config, null),
                    _ => new ConnectionError(request.error, config, null)
                }
            );
        }

        private static void RejectResponse(Promise<Response> promise, Response response)
        {
            RejectWithError(promise, ErrorFactory.Create(response));
        }

        private static void RejectWithError(Promise<Response> promise, Error error)
        {
            foreach (var responseInterceptor in Uxios.Interceptors.response)
            {
                error = responseInterceptor.error.Invoke(error);
            }

            promise.Reject(error);
        }

        private byte[] NormalizeConfigAndReturnByteArray<TData>(Config config) where TData : class
        {
            // Make URI an absolute Uri for easy handling
            var urlBuilder = new UriBuilder(!config.Url.IsAbsoluteUri ? new Uri(config.BaseUrl, config.Url) : config.Url);
            // Move any query parameters from the URL to the Params collection ...
            config.Params = new QueryParameters(QueryString.Merge(urlBuilder.Query.TrimStart('?'), config.Params.ToString()));
            // ... meaning that the query part of the URI becomes empty
            urlBuilder.Query = "";
            // Overwrite URI
            config.Url = urlBuilder.Uri;

            config.TypeOfResponseType.AddResponseMetadataToConfig(config);
            if (config.Auth is BasicAuthenticationCredentials credentials)
            {
                config.Headers.TryAdd("Authorization", credentials.ToAuthorizationToken());
            }
            
            var (contentType, bytes) = ConvertToByteArray<TData>(config.Data);
            if (string.IsNullOrEmpty(contentType) == false)
            {
                config.Headers.TryAdd("Content-Type", contentType);
            }

            return bytes;
        }

        private static (string contentType, byte[] bytes) ConvertToByteArray<T>(object data) where T : class
        {
            T dataToSend = data as T;
            if (data == null)
            {
                return (null, null);
            }
            
            switch (dataToSend)
            {
                case byte[] asByteArray:
                    return ("application/octet-stream", bytes: asByteArray);
                case string asString:
                    return ("text/plain", bytes: System.Text.Encoding.UTF8.GetBytes(asString));
                case object asObject:
                {
                    // TODO: make setting configurable
                    var serializedString = JsonConvert.SerializeObject(asObject, typeof(T), null);

                    return ("application/json", bytes: System.Text.Encoding.UTF8.GetBytes(serializedString));
                }
                default:
                    throw new Exception("Unable to determine how to convert this into a byte array");
            }
        }
    }
}