using System;
using System.Net;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using UnityEngine;
using UnityEngine.Networking;

namespace KindMen.Uxios
{
    public sealed class Response
    {
        public readonly object Data;
        public readonly HttpStatusCode Status;
        public readonly Headers Headers;
        public readonly Config Config;
        public readonly UnityWebRequest Request;

        public Response(Config config)
        {
            var webRequest = config.UnityWebRequest;
            // TODO: Check status of UnityWebRequest if it is done
            
            // TODO apply transformation according to config.responseType and then config.Transforms
            Data = config.ResponseType switch
            {
                ExpectJsonResponse expectedResponse => AsJsonObject(webRequest, expectedResponse),
                ExpectTextResponse _ => webRequest.downloadHandler.text,
                ExpectArrayBufferResponse _ => webRequest.downloadHandler.data,
                ExpectTextureResponse _ => AsTexture(webRequest),
                _ => webRequest.downloadHandler.data
            };

            Config = config;
            Status = (HttpStatusCode)webRequest.responseCode;
            Headers = new Headers(webRequest.GetResponseHeaders());
            Request = webRequest;
        }

        private static Texture2D AsTexture(UnityWebRequest webRequest)
        {
            if (webRequest.downloadHandler is not DownloadHandlerTexture webRequestDownloadHandler)
            {
                var unexpectedType = webRequest.downloadHandler.GetType();
                throw new InvalidCastException(
                    $"The Texture response expects a DownloadHandlerTexture object as DownloadHandler, but an " 
                    + $"instance of {unexpectedType} was received"
                );
            }
            
            return webRequestDownloadHandler.texture;
        }

        private static object AsJsonObject(UnityWebRequest webRequest, ExpectJsonResponse expectedResponse)
        {
            var jsonText = webRequest.downloadHandler.text;
            var expectedType = expectedResponse.DeserializeAs;
            var settings = expectedResponse.Settings;

            Debug.Log(jsonText);
            object asJsonObject;
            try
            {
                asJsonObject = JsonConvert.DeserializeObject(jsonText, expectedType, settings);
            }
            catch (JsonReaderException e)
            {
                throw new Exception("Unable to parse response as JSON, received: " + jsonText, e);
            }

            return asJsonObject;
        }

        public bool IsValid()
        {
            return Config.ValidateStatus.Invoke(Status);
        }
    }
}