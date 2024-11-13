using System;
using System.IO;
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
            if (typeof(T) == typeof(FileInfo))
            {
                // The passed type cannot have properties by definition, meaning that if the Generic way (using
                // FileInfo as hint) was used, we generate a random path in the persistentDataPath, keeping the
                // origin extension. This will make for a good foundation upon which an uploads folder can be based,
                // or just to upload files and do not worry about name clashes (which happens with WebGL because
                // you cannot perform File.Exists .... rly)
                var destinationPath = Path.Combine(Application.persistentDataPath, Guid.NewGuid().ToString());
                if (Path.HasExtension(config.Url.LocalPath))
                {
                    destinationPath += "." +Path.GetExtension(config.Url.LocalPath);
                }

                return ExpectedTypeOfResponse.File(destinationPath);
            }

            if (typeof(T) == typeof(Texture2D)) return ExpectedTypeOfResponse.Texture();
            if (typeof(T) == typeof(Sprite)) return ExpectedTypeOfResponse.Sprite();
            if (typeof(T) == typeof(string)) return ExpectedTypeOfResponse.Text();
            if (typeof(T) == typeof(JObject)) return ExpectedTypeOfResponse.Json();
            if (typeof(T) == typeof(byte[])) return ExpectedTypeOfResponse.ArrayBuffer();
            
            return ExpectedTypeOfResponse.Json(typeof(T));
        }
    }
}