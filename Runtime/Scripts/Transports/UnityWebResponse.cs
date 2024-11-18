using System;
using System.IO;
using System.Net;
using KindMen.Uxios.ExpectedTypesOfResponse;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KindMen.Uxios.Transports
{
    public sealed class UnityWebResponse : Response
    {
        [JsonIgnore] public UnityWebRequest UnityWebRequest;

        public UnityWebResponse(Config config, Request uxiosRequest, UnityWebRequest webRequest)
        {
            Config = config;
            Status = (HttpStatusCode)webRequest.responseCode;
            Headers = new Headers(webRequest.GetResponseHeaders() ?? new ());
            Request = uxiosRequest;
            UnityWebRequest = webRequest;
            Data = UnityWebRequest.downloadHandler is not DownloadHandlerFile 
                ? UnityWebRequest.downloadHandler.data // Default to the raw byte[] representation 
                : new byte[]{}; // unless the DownloadHandler does not support this
            
            // TODO: Move this higher up in the chain as a post-step to prevent exceptions here from
            // preventing the instantiation of request
            ResolveData();
        }

        private void ResolveData()
        {
            // TODO apply transformation according to config.responseType and then config.Transforms
            Data = Config.TypeOfResponseType switch
            {
                // JsonResponse is not listed on purpose - this is handled in the TransportActions as a post-creation step
                TextResponse _ => UnityWebRequest.downloadHandler.text,
                ArrayBufferResponse _ => UnityWebRequest.downloadHandler.data,
                TextureResponse _ => AsTexture(UnityWebRequest),
                SpriteResponse _ => AsSprite(UnityWebRequest),
                FileResponse type => AsFileInfo(UnityWebRequest, type),
                _ => UnityWebRequest.downloadHandler.data
            };
        }

        private FileInfo AsFileInfo(UnityWebRequest webRequest, FileResponse typeOfResponse)
        {
            if (webRequest.downloadHandler is not DownloadHandlerFile)
            {
                var unexpectedType = webRequest.downloadHandler.GetType();
                throw new InvalidCastException(
                    $"The File response expects a DownloadHandlerFile object as DownloadHandler, but an " 
                    + $"instance of {unexpectedType} was received"
                );
            }
            
            return new FileInfo(typeOfResponse.Path);
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

        private static Sprite AsSprite(UnityWebRequest webRequest)
        {
            if (webRequest.downloadHandler is not DownloadHandlerTexture webRequestDownloadHandler)
            {
                var unexpectedType = webRequest.downloadHandler.GetType();
                throw new InvalidCastException(
                    $"The Sprite response expects a DownloadHandlerTexture object as DownloadHandler, but an " 
                    + $"instance of {unexpectedType} was received"
                );
            }
            
            Texture2D texture = webRequestDownloadHandler.texture;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}