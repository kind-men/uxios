using System.Collections.Generic;
using System.Linq;
using System.Text;
using KindMen.Uxios.Http;
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
        private static StringBuilder encodingStringBuilder = new();

        public static string Escape(string str)
        {
            if (str == null) return null;

#if UNITY_WEBGL
            // Use UnityWebRequest.EscapeURL in WebGL builds since there is a bug in the dotnet version used
            var escapedString = UnityWebRequest.EscapeURL(str);
#else
            // Use Uri.EscapeDataString for other platforms
            var escapedString = Uri.EscapeDataString(str);
#endif
            // to reduce GC allocations - check if the string contains %20 and do not attempt to replace it
            // when none is present
            return escapedString.Contains("%20") ? escapedString.Replace("%20", "+") : escapedString;
        }

        public static string Unescape(string str)
        {
            if (str == null) return null;

            // Avoid allocation if '+' isn't present
            var toUnescape = str.Contains("+") ? str.Replace("+", "%20") : str;

#if UNITY_WEBGL
            // Use UnityWebRequest.UnEscapeURL in WebGL builds since there is a bug in the dotnet version used
            return UnityWebRequest.UnEscapeURL(toUnescape);
#else
            return Uri.UnescapeDataString(toUnescape);
#endif
        }

        public static string Encode(QueryParameters parameters, string sep = "&", string eq = "=")
        {
            encodingStringBuilder.Clear();
            bool first = true;

            foreach (var (key, values) in parameters)
            {
                bool isArrayKey = key.EndsWith("[]");
                string keyPart = isArrayKey
                    ? key[..^2]
                    : key;

                string encodedKey = Escape(keyPart);
                if (isArrayKey)
                {
                    encodedKey += "[]";
                }

                foreach (string value in values)
                {
                    if (!first) encodingStringBuilder.Append(sep);
                    first = false;

                    encodingStringBuilder.Append(encodedKey);
                    encodingStringBuilder.Append(eq);
                    encodingStringBuilder.Append(Escape(value));
                }
            }

            return encodingStringBuilder.ToString();
        }

        public static QueryParameters Decode(string str, char sep = '&', char eq = '=')
        {
            var result = new QueryParameters(2);

            if (string.IsNullOrEmpty(str))
            {
                return result;
            }

            int counter = 0;
            int length = str.Length;

            if (str[0] == '?') counter++; // skip leading '?'

            while (counter < length)
            {
                int keyStart = counter;
                int keyEnd = -1;
                int valueStart = -1;
                int valueEnd = -1;

                while (counter < length)
                {
                    char character = str[counter];
                    if (character == eq && keyEnd == -1)
                    {
                        keyEnd = counter;
                        valueStart = counter + 1;
                    }
                    else if (character == sep)
                    {
                        valueEnd = counter;
                        break;
                    }
                    counter++;
                }

                if (valueEnd == -1)
                {
                    valueEnd = counter;
                }

                if (keyEnd == -1)
                {
                    keyEnd = valueEnd;
                    valueStart = valueEnd;
                }

                // Extract key and value substrings
                string key = Unescape(str.Substring(keyStart, keyEnd - keyStart));
                string value = Unescape(str.Substring(valueStart, valueEnd - valueStart));

                // Handle "arrayKey[]"
                if (key.EndsWith("[]"))
                {
                    key = key[..^2];
                }

                result.Add(key, value);

                counter++; // skip past '&'
            }

            return result;
        }

        public static string Merge(string source, string mergeOnto)
        {
            return Encode(Merge(Decode(source), Decode(mergeOnto)));
        }

        public static string Merge(string source, QueryParameters mergeOnto)
        {
            return Encode(Merge(Decode(source), mergeOnto));
        }

        public static QueryParameters Merge(QueryParameters source, string mergeOnto)
        {
            return Merge(source, Decode(mergeOnto));
        }

        public static QueryParameters Merge(QueryParameters source, QueryParameters mergeOnto)
        {
            var result = new QueryParameters();

            foreach (var (key, value) in source)
            {
                result[key] = value;
            }

            foreach (var (key, value) in mergeOnto)
            {
                foreach (var v in value)
                {
                    result.Add(key, v);
                }
            }

            return result;
        }

        /// <summary>
        /// Serializes the given object into a query parameters string.
        /// </summary>
        /// <remarks>
        /// The object is first serialized into JSON using Newtonsoft JSON.net and then converted into a 
        /// QueryParameters collection. Each property of the object becomes a key-value pair. Use attributes like
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
            
            var qp = new QueryParameters();
            foreach (var (key, value) in dictionary)
            {
                qp.Add(key, value);
            }
            
            return Encode(qp);
        }

        /// <summary>
        /// Deserializes the given query parameters string into an object of the specified type.
        /// </summary>
        /// <remarks>
        /// This method uses Newtonsoft JSON.net to populate the given type. The query parameters string is 
        /// first decoded into a QueryParameters object, then serialized into a JSON string, and finally 
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
            QueryParameters parameters = Decode(queryParameters);
            
            string json = JsonConvert.SerializeObject(
                parameters.Keys.ToDictionary(k => k, k => string.Join(',', parameters[k].Values))
            );

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}