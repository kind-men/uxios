using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
#if UNITY_WEBGL
using UnityEngine.Networking;
#endif

namespace KindMen.Uxios
{
    /// <summary>
    /// A utility class for encoding, decoding, parsing, and stringifying URL query strings, 
    /// providing a C# implementation inspired by Node.js's querystring module.
    /// <see href="https://nodejs.org/api/querystring.html">Node.js Query String API</see>
    /// </summary>
    /// <remarks>
    /// This class replicates much of the functionality of Node.js's querystring API, including:
    /// - Encoding and decoding query strings with support for converting spaces to/from the "+" character.
    /// - Parsing query strings into a collection of key-value pairs and handling array-like keys (e.g., "key[]").
    /// - Stringifying collections into valid query strings with configurable separators and delimiters.
    ///
    /// It is designed to work in Unity, including WebGL builds, with conditional compilation to use Unity's
    /// <c>UnityWebRequest</c> for encoding/decoding on platforms where <c>Uri.EscapeDataString</c> has known issues.
    /// </remarks>
    public static class QueryString
    {
        public static string Escape(string str)
        {
#if UNITY_WEBGL
            // Use UnityWebRequest.EscapeURL in WebGL builds since there is a bug in the dotnet version used
            return UnityWebRequest.EscapeURL(str).Replace("%20", "+");
#else
            // Use Uri.EscapeDataString for other platforms
            return Uri.EscapeDataString(str).Replace("%20", "+");
#endif
        }

        public static string Unescape(string str)
        {
#if UNITY_WEBGL
            // Use UnityWebRequest.UnEscapeURL in WebGL builds since there is a bug in the dotnet version used
            return UnityWebRequest.UnEscapeURL(str.Replace("+", "%20"));
#else
            // Use Uri.UnescapeDataString for other platforms
            return Uri.UnescapeDataString(str.Replace("+", "%20"));
#endif
        }

        public static string Encode(NameValueCollection collection, string sep = "&", string eq = "=")
        {
            var sb = new StringBuilder();
            foreach (string key in collection)
            {
                foreach (string value in collection.GetValues(key))
                {
                    if (sb.Length > 0) sb.Append(sep);
                    string encodedKey;
                    if (key.EndsWith("[]"))
                    {
                        encodedKey = Escape(key.Substring(0, key.Length - 2)) + "[]";
                    }
                    else
                    {
                        encodedKey = Escape(key);
                    }

                    sb.Append(encodedKey + eq + Escape(value));
                }
            }
            return sb.ToString();
        }

        public static NameValueCollection Decode(string str, string sep = "&", string eq = "=")
        {
            var collection = new NameValueCollection();
            var pairs = str.Split(new string[] { sep }, StringSplitOptions.RemoveEmptyEntries);
        
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(new string[] { eq }, 2, StringSplitOptions.None);
                if (keyValue.Length == 2)
                {
                    var key = Unescape(keyValue[0]);
                    var value = Unescape(keyValue[1]);

                    // If the key ends with [], treat it as an array-like key
                    if (key.EndsWith("[]"))
                    {
                        var baseKey = key.Substring(0, key.Length - 2);  // Remove "[]" from the end
                        collection.Add(baseKey, value);
                    }
                    else
                    {
                        collection.Add(key, value);
                    }
                }
            }
            return collection;
        }

        public static string Merge(string source, string mergeOnto)
        {
            return Encode(Merge(Decode(source), Decode(mergeOnto)));
        }

        public static string Merge(string source, NameValueCollection mergeOnto)
        {
            return Encode(Merge(Decode(source), mergeOnto));
        }

        public static NameValueCollection Merge(NameValueCollection source, string mergeOnto)
        {
            return Merge(source, Decode(mergeOnto));
        }

        public static NameValueCollection Merge(NameValueCollection source, NameValueCollection mergeOnto)
        {
            var result = new NameValueCollection();

            // Add all keys from the first collection
            foreach (string key in source)
            {
                foreach (var value in source.GetValues(key))
                {
                    result.Add(key, value);
                }
            }

            // Add keys from the second collection, merging values for duplicate keys
            foreach (string key in mergeOnto)
            {
                foreach (var value in mergeOnto.GetValues(key))
                {
                    result.Add(key, value);
                }
            }

            return result;
        }

        /// <summary>
        /// Serializes the given object into a query parameters string.
        /// </summary>
        /// <remarks>
        /// The object is first serialized into JSON using Newtonsoft JSON.net and then converted into a 
        /// NameValueCollection. Each property of the object becomes a key-value pair. Use attributes like
        /// <see cref="JsonPropertyAttribute"/> to map property names when keys in the query parameters contain
        /// special characters (e.g., hyphens).
        ///
        /// Note that this method does not support nested objects or complex data structures, as they cannot be
        /// represented as query parameters. Ensure your object properties are simple types like strings, numbers, or
        /// booleans.
        /// </remarks>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="parameters">The object containing the data to serialize.</param>
        /// <returns>A query parameters string where keys and values correspond to the object's properties.</returns>
        public static string Serialize<T>(T parameters)
        {
            var json = JsonConvert.SerializeObject(parameters);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            
            var nameValueCollection = new NameValueCollection();
            foreach (var kvp in dictionary)
            {
                nameValueCollection.Add(kvp.Key, kvp.Value);
            }
            
            return Encode(nameValueCollection);
        }

        /// <summary>
        /// Deserializes the given query parameters string into an object of the specified type.
        /// </summary>
        /// <remarks>
        /// This method uses Newtonsoft JSON.net to populate the given type. The query parameters string is 
        /// first decoded into a NameValueCollection, then serialized into a JSON string, and finally 
        /// deserialized into the target object type. Use attributes like <see cref="JsonPropertyAttribute"/> 
        /// to map property names when keys in the query parameters contain special characters (e.g., hyphens).
        /// 
        /// Note: The type <typeparamref name="T"/> must be a class or struct with properties that match 
        /// the query parameter keys. Nested types or complex data structures are not supported by this method.
        /// </remarks>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <param name="queryParameters">The query parameters string to deserialize.</param>
        /// <returns>An object of type <typeparamref name="T"/> populated with data from the query parameters.</returns>
        public static T Deserialize<T>(string queryParameters)
        {
            var nvc = Decode(queryParameters);
            string json = JsonConvert.SerializeObject(nvc.AllKeys.ToDictionary(k => k, k => nvc[k]));

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}