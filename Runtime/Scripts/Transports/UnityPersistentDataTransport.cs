using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using KindMen.Uxios.Errors;
using KindMen.Uxios.ExpectedTypesOfResponse;
using RSG;
using UnityEngine;
using UnityEngine.Networking;

namespace KindMen.Uxios.Transports
{
    /// <summary>
    /// Files stores in Unity's persistent data path should be retrieved using UnityWebRequest as WebGL will store
    /// those files as indexeddb files, and the regular `System.IO` interactions do not work there.
    /// </summary>
    internal sealed class UnityPersistentDataTransport : MonoBehaviour, IUxiosTransport
    {
        private static UnityPersistentDataTransport unityPersistentDataTransport;

        public string[] SupportedSchemes => new[] { "unity+persistent" };

        public static UnityPersistentDataTransport Instance()
        {
            if (unityPersistentDataTransport != null) return unityPersistentDataTransport;

            var gameObject = new GameObject
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            DontDestroyOnLoad(gameObject);

            unityPersistentDataTransport = gameObject.AddComponent<UnityPersistentDataTransport>();

            return unityPersistentDataTransport;
        }

        public Promise<Response> PerformRequest<TData>(Config config) where TData : class
        {
            var promise = new Promise<Response>();

            StartCoroutine(DoRequest<TData>(config, promise));

            return promise;
        }

        private IEnumerator DoRequest<TData>(Config config, Promise<Response> promise) where TData : class
        {
            var uxiosRequest = TransportActions.CreateRequest<TData>(ref config);

            // By default, any unhandled Method type will return a MethodNotAllowed response; this will not reject the
            // promise outright, as the ValidateResponse action in the config will do that
            Func<Config, Request, Response> responseCreator = (config, request) => 
                CreateResponse(config, request, HttpStatusCode.MethodNotAllowed);

            // Convert Url into a Path to retrieve the file from the persistentDataPath
            var path = uxiosRequest.Url.AbsolutePath.TrimStart('/', '\\');
            if (string.IsNullOrEmpty(path))
            {
                TransportActions.RejectWithErrorDuringRequest(
                    promise,
                    new ConnectionError(
                        "No file path was provided when using the scheme 'unity+persistent', please check if " 
                        + "there are 3 slashes after the scheme instead of 2", 
                        config, 
                        null
                    )
                );

                yield break;
            }
            path = Path.Combine(Application.persistentDataPath, path);

            if (uxiosRequest.Method == HttpMethod.Get || uxiosRequest.Method == HttpMethod.Head)
            {
                var unityWebRequest = ConvertToUnityWebRequest(config, uxiosRequest, path);

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

                if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError
                    or UnityWebRequest.Result.DataProcessingError)
                {
                    RejectDuringRequest(config, promise, unityWebRequest);
                    yield break;
                }

                responseCreator = (config, request) => CreateUnityWebResponse(config, request, unityWebRequest);
            } 
            else if (uxiosRequest.Method == HttpMethod.Post || uxiosRequest.Method == HttpMethod.Put)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytes(path, uxiosRequest.Data);
#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif
                responseCreator = (config, request) => CreateResponse(config, request, HttpStatusCode.OK);
            }
            // TODO: figure out how to do deletes, that is also interesting
            
            TransportActions.HandleResponse(promise, config, uxiosRequest, responseCreator);
        }

        private static UnityWebResponse CreateUnityWebResponse(Config config, Request request, UnityWebRequest unityWebRequest)
        {
            return new UnityWebResponse(
                config,
                request,
                unityWebRequest
            );
        }

        private static Response CreateResponse(Config config, Request request, HttpStatusCode status)
        {
            return new Response
            {
                Config = config,
                Request = request,
                Status = status
            };
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

        private UnityWebRequest ConvertToUnityWebRequest(Config config, Request uxiosRequest, string path)
        {
            // Directly use the Path - this will work cross platform
            var webRequest = new UnityWebRequest(path, uxiosRequest.Method.ToString());

            webRequest.downloadHandler = config.TypeOfResponseType switch
            {
                TextureResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                FileResponse responseType => new DownloadHandlerFile(responseType.Path),
                SpriteResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                _ => new DownloadHandlerBuffer()
            };
            webRequest.uploadHandler = new UploadHandlerRaw(uxiosRequest.Data ?? new byte[] { });

            return webRequest;
        }
    }
}