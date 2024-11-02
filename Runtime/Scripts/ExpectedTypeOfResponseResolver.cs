using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KindMen.Uxios
{
    public class ExpectedTypeOfResponseResolver
    {
        /// <summary>
        /// The default expected response is a Json interpretation returning a JObject to query.
        /// </summary>
        public ExpectedTypeOfResponse Resolve(Config config)
        {
            return ExpectedTypeOfResponse.Json();
        }

        public ExpectedTypeOfResponse Resolve<T>(Config config)
        {
            if (typeof(T) == typeof(Texture2D)) return ExpectedTypeOfResponse.Texture();
            if (typeof(T) == typeof(string)) return ExpectedTypeOfResponse.Text();
            if (typeof(T) == typeof(JObject)) return ExpectedTypeOfResponse.Json();
            if (typeof(T) == typeof(byte[])) return ExpectedTypeOfResponse.ArrayBuffer();
            
            return ExpectedTypeOfResponse.Json(typeof(T));
        }
    }
}