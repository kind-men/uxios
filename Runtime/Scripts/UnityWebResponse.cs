﻿using System;
using System.Net;
using KindMen.Uxios.ExpectedTypesOfResponse;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KindMen.Uxios
{
    public sealed class UnityWebResponse : Response
    {
        public UnityWebResponse(Config config, UnityWebRequest webRequest)
        {
            // TODO apply transformation according to config.responseType and then config.Transforms
            Data = config.TypeOfResponseType switch
            {
                JsonResponse expectedResponse => AsJsonObject(webRequest, expectedResponse),
                TextResponse _ => webRequest.downloadHandler.text,
                ArrayBufferResponse _ => webRequest.downloadHandler.data,
                TextureResponse _ => AsTexture(webRequest),
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

        private static object AsJsonObject(UnityWebRequest webRequest, JsonResponse expectedResponse)
        {
            var jsonText = webRequest.downloadHandler.text;
            var expectedType = expectedResponse.DeserializeAs;
            var settings = expectedResponse.Settings;

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
    }
}