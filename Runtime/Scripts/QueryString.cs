﻿using System;
using System.Collections.Specialized;
using System.Text;

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
    }
}