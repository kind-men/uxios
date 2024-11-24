using System;
using System.Collections;
using KindMen.Uxios.Errors;
using KindMen.Uxios.ExpectedTypesOfResponse;
using KindMen.Uxios.Transports.Unity;
using RSG;
using UnityEngine;
using UnityEngine.Networking;
using DataProcessingError = KindMen.Uxios.Errors.DataProcessingError;

namespace KindMen.Uxios.Transports
{
    internal sealed class UnityWebRequestTransport : MonoBehaviour, IUxiosTransport
    {
        private static UnityWebRequestTransport unityWebRequestTransport;

        public string[] SupportedSchemes => new[] { "http", "https", "file" };

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

        private IEnumerator DoRequest<TData>(Config config, Promise<Response> promise) where TData : class
        {
            var uxiosRequest = TransportActions.CreateRequest<TData>(ref config);
            var unityWebRequest = ConvertToUnityWebRequest(config, uxiosRequest);
            
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
                RejectDuringRequest(config, promise, unityWebRequest);
                yield break;
            }

            TransportActions.HandleResponse(
                promise, 
                config, 
                uxiosRequest, 
                (config, request) => new UnityWebResponse(config, request, unityWebRequest)
            );
        }

        private void RejectDuringRequest(Config config, Promise<Response> promise, UnityWebRequest request)
        {
            TransportActions.RejectWithErrorDuringRequest(
                promise, 
                request.result switch
                {
                    UnityWebRequest.Result.DataProcessingError => new DataProcessingError(request.error, config, null),
                    _ => new ConnectionError(request.error, config, null)
                }
            );
        }

        private UnityWebRequest ConvertToUnityWebRequest(Config config, Request uxiosRequest)
        {
            // Attach all params to the URL for UnityWebRequest
            var url = new UriBuilder(uxiosRequest.Url) { Query = uxiosRequest.QueryString.ToString() }.Uri;

            var webRequest = new UnityWebRequest(url, uxiosRequest.Method.ToString());
            // Unity's timeout is in whole seconds only, so we divide by 1000 (multiplication is done as a performance
            // improvement) and round up (ceil) to prevent a value of -for example- 300ms accidentally becoming 0 and
            // thus: no timeout
            webRequest.timeout = Mathf.CeilToInt(config.Timeout * 0.001f);
            webRequest.redirectLimit = config.MaxRedirects;
            webRequest.downloadHandler = HandlerFactory.DownloadHandler(config.TypeOfResponseType);
            webRequest.uploadHandler = HandlerFactory.UploadHandler(uxiosRequest);

            foreach (var header in uxiosRequest.Headers)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }

            return webRequest;
        }
    }
}