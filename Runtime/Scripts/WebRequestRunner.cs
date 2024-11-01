using System;
using System.Collections;
using RSG;
using UnityEngine;
using UnityEngine.Networking;

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

        public Promise<Response> PerformRequest(Config config)
        {
            var promise = new Promise<Response>();
            
            // We don't need to store the coroutine reference because the cancellation token in the 
            // config will self-abort the coroutine if it needs to be cancelled
            StartCoroutine(DoRequest(config, promise));

            return promise;
        }
        
        private IEnumerator DoRequest(Config config, Promise<Response> promise)
        {
            // TODO: Perform transforms for request and return a error promise if it fails
            var request = config.UnityWebRequest;

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
 
            // something in the request's connection went wrong
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError)
            {
                var error = new Error(request.error, config, null);
                foreach (var requestInterceptor in Uxios.Interceptors.request)
                {
                    error = requestInterceptor.error.Invoke(error);
                }

                promise.Reject(error);
                yield break;
            }

            // If an exception occurs in the whole response interpretation chain, reject the promise
            try
            {
                var response = new Response(config);

                if (response.IsValid() == false) 
                {
                    var error = new Error(response.Data.ToString(), config, response);
                    foreach (var responseInterceptor in Uxios.Interceptors.response)
                    {
                        error = responseInterceptor.error.Invoke(error);
                    }

                    promise.Reject(error);
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
                var error = new Error(e.Message, config, null);
                foreach (var responseInterceptor in Uxios.Interceptors.response)
                {
                    error = responseInterceptor.error.Invoke(error);
                }

                promise.Reject(error);
            }
        }        
    }
}