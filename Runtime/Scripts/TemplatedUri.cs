using System;
using System.Collections.Generic;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios
{
    public class TemplatedUri
    {
        private readonly Uri uri;
        private readonly QueryParameters parameters;

        public TemplatedUri(Uri uri, QueryParameters parameters = null)
        {
            this.uri = uri;
            this.parameters = parameters ?? new QueryParameters();
        }
        
        public TemplatedUri(string uri, QueryParameters parameters = null)
        {
            this.uri = new Uri(uri);
            this.parameters = parameters ?? new QueryParameters();
        }

        private Uri Resolve()
        {
            UriBuilder uriBuilder = new UriBuilder(uri);
            string pathAndQuery = uri.PathAndQuery
                .Replace("%7B", "{")
                .Replace("%7D", "}");

            foreach (var uriTemplatePart in GetUriTemplateParts())
            {
                var replacement = parameters.Get(uriTemplatePart);
                if (replacement == null) continue;

                pathAndQuery = pathAndQuery.Replace(
                    $"{{{uriTemplatePart}}}",
                    QueryString.Escape(replacement)
                );
            }

            var pathAndQueryArray = pathAndQuery.Split('?');
            uriBuilder.Path = pathAndQueryArray[0];
            uriBuilder.Query = pathAndQueryArray.Length > 1 ? pathAndQueryArray[1] : "";
            
            return uriBuilder.Uri;
        }

        public List<string> GetUriTemplateParts()
        {
            var parameterNames = new List<string>();
            int i = 0;

            var pathAndQuery = uri.PathAndQuery
                .Replace("%7B", "{")
                .Replace("%7D", "}");
            
            // Using a RegEx would have been easier, but because of Garbage Collection in Unity we will avoid that
            while (i < pathAndQuery.Length)
            {
                if (pathAndQuery[i] == '{')
                {
                    int endIndex = pathAndQuery.IndexOf('}', i);
                    if (endIndex > i)
                    {
                        // Extract the parameter name within the braces
                        var startIndex = i + 1;
                        string paramName = pathAndQuery.Substring(startIndex, endIndex - i - 1);

                        // Only add unique parameter names
                        if (!parameterNames.Contains(paramName))
                        {
                            parameterNames.Add(paramName);
                        }

                        // Move the index past the closing brace
                        i = endIndex + 1;
                        continue;
                    }
                }

                // If it's not a placeholder, just move to the next character
                i++;
            }

            return parameterNames;
        }
    
        public override string ToString()
        {
            return Resolve().ToString();
        }

        public static implicit operator TemplatedUri(string uriString) => new(uriString);

        public static implicit operator Uri(TemplatedUri customUri) => customUri.Resolve();

        public static implicit operator string(TemplatedUri customUri) => customUri.ToString();
    }
}