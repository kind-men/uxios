using System;
using System.Net;
using KindMen.Uxios.ExpectedTypesOfResponse;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using UnityEngine;

namespace KindMen.Uxios.Transports
{
    public sealed class FakedWebResponse : Response
    {
        public FakedWebResponse(
            Config config, 
            Request uxiosRequest, 
            HttpStatusCode responseCode, 
            Headers headers, 
            object responseData
        ) {
            Data = config.TypeOfResponseType switch
            {
                JsonResponse expectedResponse => AsJsonObject(responseData as string, expectedResponse),
                TextResponse _ => responseData as string,
                ArrayBufferResponse _ => responseData as byte[],
                TextureResponse _ => AsTexture(responseData as Texture2D),
                SpriteResponse _ => AsSprite(responseData as Texture2D),
                _ => responseData as byte[]
            };

            Config = config;
            Status = responseCode;
            Headers = headers;
            Request = uxiosRequest;
        }

        private static Texture2D AsTexture(Texture2D content)
        {
            return content;
        }

        private static Sprite AsSprite(Texture2D content)
        {
            Texture2D texture = content;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        private static object AsJsonObject(string jsonText, JsonResponse expectedResponse)
        {
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