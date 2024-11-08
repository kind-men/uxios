using System;
using System.Collections;
using KindMen.Uxios.Errors;
using KindMen.Uxios.ExpectedTypesOfResponse;
using RSG;
using UnityEngine;
using UnityEngine.Networking;
using DataProcessingError = KindMen.Uxios.Errors.DataProcessingError;

namespace KindMen.Uxios.Transports
{
    internal sealed class UnityWebRequestTransport : MonoBehaviour, IUxiosTransport
    {
        private static UnityWebRequestTransport unityWebRequestTransport;

        public static UnityWebRequestTransport Instance()
        {
            if (unityWebRequestTransport != null) return unityWebRequestTransport;

            var gameObject = new GameObject
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            DontDestroyOnLoad(gameObject);

            unityWebRequestTransport = gameObject.AddComponent<UnityWebRequestTransport>();

            return unityWebRequestTransport;
        }

        public Promise<Response> PerformRequest<TData>(Config config) where TData : class
        {
            var promise = new Promise<Response>();
            
            // We don't need to store the coroutine reference because the cancellation token in the 
            // config will self-abort the coroutine if it needs to be cancelled
            StartCoroutine(DoRequest<TData>(config, promise));

            return promise;
        }

        private UnityWebRequest ConvertToUnityWebRequest(Config config, Request uxiosRequest)
        {
            // Attach all params to the URL for UnityWebRequest
            var url = new UriBuilder(uxiosRequest.Url) { Query = uxiosRequest.QueryString.ToString() }.Uri;

            var webRequest = new UnityWebRequest(url, uxiosRequest.Method.ToString());
            webRequest.timeout = config.Timeout;
            webRequest.redirectLimit = config.MaxRedirects;
            webRequest.downloadHandler = config.TypeOfResponseType switch
            {
                TextureResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                SpriteResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                _ => new DownloadHandlerBuffer()
            };
            webRequest.uploadHandler = new UploadHandlerRaw(uxiosRequest.Data ?? new byte[]{});

            foreach (var header in uxiosRequest.Headers)
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

            var uxiosRequest = Request.FromConfig<TData>(config);
            var unityWebRequest = ConvertToUnityWebRequest(config, uxiosRequest);

            foreach (var requestInterceptor in Uxios.Interceptors.request)
            {
                config = requestInterceptor.success.Invoke(config);
            }

            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone) 
            {
                if (config.CancelToken.IsCancellationRequested)
                {
                    // Abort the request and continue in the loop, abort attempts to finish
                    // as soon as possible, but may not be immediate
                    unityWebRequest.Abort();
                }

                yield return null;
            }

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError)
            {
                RejectRequest(config, promise, unityWebRequest);
                yield break;
            }

            // If an exception occurs in the whole response interpretation chain, reject the promise
            try
            {
                Response response = new UnityWebResponse(config, uxiosRequest, unityWebRequest);

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
    }
}