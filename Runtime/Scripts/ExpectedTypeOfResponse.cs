﻿using System;
using KindMen.Uxios.ExpectedTypesOfResponse;

namespace KindMen.Uxios
{
    public abstract class ExpectedTypeOfResponse
    {
        public abstract void AddMetadataToRequest(Request request);
        
        public static ExpectedTypeOfResponse Text()
        {
            return new TextResponse();
        }

        public static ExpectedTypeOfResponse ArrayBuffer()
        {
            return new ArrayBufferResponse();
        }

        public static ExpectedTypeOfResponse Texture(bool readOnly = false)
        {
            return new TextureResponse { Readable = readOnly };
        }

        public static ExpectedTypeOfResponse File(string path)
        {
            return new FileResponse { Path = path };
        }

        public static ExpectedTypeOfResponse Sprite(bool readOnly = false)
        {
            return new SpriteResponse { Readable = readOnly };
        }

        public static ExpectedTypeOfResponse Json(Type deserializeAs = null)
        {
            return new JsonResponse { DeserializeAs = deserializeAs };
        }
    }
}