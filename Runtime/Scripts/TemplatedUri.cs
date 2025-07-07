using System;
using System.Collections.Generic;
using System.Text;
using KindMen.Uxios.Http;

namespace KindMen.Uxios
{
    public class TemplatedUri
    {
        private readonly Uri uri;
        private QueryParameters parameters;

        public TemplatedUri(Uri uri, QueryParameters parameters = null)
        {
            this.uri = uri;
            this.parameters = parameters ?? new QueryParameters();
        }
        
        public TemplatedUri(string uri, QueryParameters parameters = null) : this(new Uri(uri), parameters)
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
            var newParameters = new QueryParameters(parameters);
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
        public TemplatedUri Using(QueryParameters parameters)
        {
            // Return a new instance with a similar footprint, this will have the original TemplatedUri remain untouched
            // and reusable as a template for other locations.
            return new TemplatedUri(uri, parameters);
        }

        private Uri Resolve()
        {
            var uriTemplateParts = GetUriTemplateParts();
            if (uriTemplateParts.Count == 0) return uri;

            var pathAndQueryBuilder = new StringBuilder(uri.PathAndQuery);

            pathAndQueryBuilder
                .Replace("%7B", "{")
                .Replace("%7D", "}");

            foreach (var uriTemplatePart in uriTemplateParts)
            {
                var replacement = parameters.Consume(uriTemplatePart);
                if (replacement == null) continue;

                pathAndQueryBuilder.Replace(
                    $"{{{uriTemplatePart}}}",
                    QueryString.Escape(replacement)
                );
            }

            var pathAndQuery = pathAndQueryBuilder.ToString();
            int queryIndex = pathAndQuery.IndexOf('?');

            UriBuilder uriBuilder = new UriBuilder(uri)
            {
                Path = queryIndex >= 0 ? pathAndQuery.Substring(0, queryIndex) : pathAndQuery,
                Query = queryIndex >= 0 ? pathAndQuery.Substring(queryIndex + 1) : string.Empty
            };

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

        public Uri Uri()
        {
            return Resolve();
        }

        public override string ToString()
        {
            return Resolve().ToString();
        }

        public static implicit operator TemplatedUri(string uriString) => new(uriString);

        public static implicit operator Uri(TemplatedUri customUri) => customUri.Uri();

        public static implicit operator string(TemplatedUri customUri) => customUri.ToString();
    }
}