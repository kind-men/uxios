using System;
using Newtonsoft.Json;

namespace KindMen.Uxios
{
    /// <summary>
    /// Configurable 'enum' type, some of these types of responses can be configured; such as the JSON
    /// response where you can define into what type the response should be deserialized 
    /// </summary>
    public abstract class ExpectedResponse
    {
        public static ExpectedResponse Text()
        {
            return new ExpectTextResponse();
        }

        public static ExpectedResponse ArrayBuffer()
        {
            return new ExpectArrayBufferResponse();
        }

        public static ExpectedResponse Texture(bool readOnly = false)
        {
            return new ExpectTextureResponse { Readable = readOnly };
        }

        public static ExpectedResponse Json(Type deserializeAs = null)
        {
            return new ExpectJsonResponse { DeserializeAs = deserializeAs };
        }
    }

    public sealed class ExpectTextResponse : ExpectedResponse
    {
    }

    public sealed class ExpectArrayBufferResponse : ExpectedResponse
    {
    }

    public sealed class ExpectTextureResponse : ExpectedResponse
    {
        public bool Readable = false;
    }

    public sealed class ExpectJsonResponse : ExpectedResponse
    {
        public Type DeserializeAs = null;
        public JsonSerializerSettings Settings = new();
    }
}