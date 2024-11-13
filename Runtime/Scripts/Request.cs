using System;
using System.Net.Http;
using System.Text;
using KindMen.Uxios.Http;
using Newtonsoft.Json;

namespace KindMen.Uxios
{
    /// <summary>
    /// Represents the HTTP request configuration and data for Uxios.
    /// The Request object contains information about the request URL, HTTP method,
    /// headers, query parameters, and body data, and it can be created from a Config object.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// The full URI for the request, including base URL and endpoint.
        /// The URL may be modified by interceptors.
        /// </summary>
        public Uri Url;
        
        /// <summary>
        /// The HTTP method used for the request (e.g., GET, POST, PUT, DELETE).
        /// Defaults to HttpMethod.Get unless specified otherwise in the configuration.
        /// </summary>
        public HttpMethod Method = HttpMethod.Get;
        
        /// <summary>
        /// A collection of HTTP headers included in the request, stored as key-value pairs.
        /// Headers provide additional information such as authorization, content type, etc.
        /// </summary>
        public Headers Headers = new();
        
        /// <summary>
        /// The query parameters for the request, represented as a collection of key-value pairs.
        /// These parameters are appended to the URL as part of the query string.
        /// </summary>
        public QueryParameters QueryString = new();
        
        /// <summary>
        /// The body data to be sent with the request, serialized into a byte array.
        /// Used primarily for POST and PUT requests to transfer JSON, text, or binary data.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Creates a new Request object based on the specified configuration.
        /// The configuration includes settings such as the URL, headers, query parameters, data, and authentication.
        /// This method extracts and organizes these settings into a Request object ready for further modification or
        /// dispatch.
        /// </summary>
        /// <typeparam name="TData">The type of data provided for serialization in the request body.</typeparam>
        /// <param name="config">The configuration object that defines request settings.</param>
        /// <returns>A Request object initialized with the settings specified in the config.</returns>
        public static Request FromConfig<TData>(Config config) where TData : class
        {
            var request = new Request();

            // Make URI an absolute Uri for easy handling
            var urlBuilder = new UriBuilder(!config.Url.IsAbsoluteUri ? new Uri(config.BaseUrl, config.Url) : config.Url);
            
            // Move any query parameters from the URL to the QueryString collection ...
            request.QueryString = new QueryParameters(
                KindMen.Uxios.QueryString.Merge(
                    urlBuilder.Query.TrimStart('?'), 
                    config.Params.ToString()
                )
            );
            
            // ... meaning that the query part of the URI becomes empty
            urlBuilder.Query = "";
            request.Url = urlBuilder.Uri;
            request.Method = config.Method;
            request.Headers = new Headers(config.Headers);

            switch (config.Auth)
            {
                case ICredentialsUsingAuthorizationToken credentials:
                    request.Headers.TryAdd("Authorization", credentials.ToAuthorizationToken());
                    break;
                case ICredentialsUsingQueryString queryStringCredentials:
                    request.QueryString.Add(queryStringCredentials.ToQueryStringSegments());
                    break;
            }

            config.TypeOfResponseType.AddMetadataToRequest(request);
            var (contentType, bytes) = ConvertToByteArray<TData>(config.Data);
            if (string.IsNullOrEmpty(contentType) == false)
            {
                request.Headers.TryAdd("Content-Type", contentType);
            }
            request.Data = bytes;
            
            return request;
        }
        
        /// <summary>
        /// Converts the provided data into a byte array, setting the Content-Type based on the data type.
        /// This helper method handles various data types, such as byte arrays, strings, and JSON-serializable objects.
        /// </summary>
        /// <typeparam name="T">The type of data to convert.</typeparam>
        /// <param name="data">The data to be converted into a byte array for the request body.</param>
        /// <returns>A tuple containing the content type as a string and the serialized data as a byte array.</returns>
        private static (string contentType, byte[] bytes) ConvertToByteArray<T>(object data) where T : class
        {
            T dataToSend = data as T;
            if (data == null)
            {
                return (null, null);
            }
            
            switch (dataToSend)
            {
                case byte[] asByteArray:
                    return ("application/octet-stream", bytes: asByteArray);
                case string asString:
                    return ("text/plain", bytes: Encoding.UTF8.GetBytes(asString));
                case object asObject:
                {
                    // TODO: make setting configurable
                    var serializedString = JsonConvert.SerializeObject(asObject, typeof(T), null);

                    return ("application/json", bytes: Encoding.UTF8.GetBytes(serializedString));
                }
                default:
                    throw new Exception("Unable to determine how to convert this into a byte array");
            }
        }
    }
}