using System;
using System.Collections.Generic;
using UriParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios
{
    public class TemplatedUri
    {
        private readonly Uri uri;
        private UriParameters parameters;

        public TemplatedUri(Uri uri, UriParameters parameters = null)
        {
            this.uri = uri;
            this.parameters = parameters ?? new UriParameters();
        }
        
        public TemplatedUri(string uri, UriParameters parameters = null) : this(new Uri(uri), parameters)
        {
        }

        /// <summary>
        /// Fluidly add parameters so that the templated URI can be used a template without initial parameters, and
        /// it can be chained into calls with more readability
        /// </summary>
        public TemplatedUri With(string key, string value)
        {
            // The parameters collection is replaced with a new one so that the UriTemplate can really be used as a
            // template, and we do not inadvertently alter the original parameters
            var newParameters = new UriParameters(parameters);
            newParameters.Add(key, value);

            // Return a new instance with a similar footprint, this will have the original TemplatedUri remain untouched
            // and reusable as a template for other locations.
            return new TemplatedUri(uri, newParameters);
        }

        /// <summary>
        /// Fluidly use parameters from another collection so that the templated URI can be used a template without
        /// initial parameters, and it can be chained into calls with more readability. Any parameters used in the
        /// process are consumed upon resolving, and will be removed so that only unused parameters remain.
        /// </summary>
        public TemplatedUri Using(UriParameters parameters)
        {
            // Return a new instance with a similar footprint, this will have the original TemplatedUri remain untouched
            // and reusable as a template for other locations.
            return new TemplatedUri(uri, parameters);
        }

        private Uri Resolve()
        {
            UriBuilder uriBuilder = new UriBuilder(uri);
            string pathAndQuery = uri.PathAndQuery
                .Replace("%7B", "{")
                .Replace("%7D", "}");

            foreach (var uriTemplatePart in GetUriTemplateParts())
            {
                var replacement = parameters.Consume(uriTemplatePart);
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