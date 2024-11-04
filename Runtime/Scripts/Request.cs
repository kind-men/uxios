using System;
using System.Linq;
using System.Net.Http;
using KindMen.Uxios.Http;
using Newtonsoft.Json;

namespace KindMen.Uxios
{
    public class Request
    {
        public Uri Url;
        public HttpMethod Method = HttpMethod.Get;
        public Headers Headers = new();
        public QueryParameters QueryString = new();
        public byte[] Data;

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
            if (config.Auth is BasicAuthenticationCredentials credentials)
            {
                request.Headers.TryAdd("Authorization", credentials.ToAuthorizationToken());
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
                    return ("text/plain", bytes: System.Text.Encoding.UTF8.GetBytes(asString));
                case object asObject:
                {
                    // TODO: make setting configurable
                    var serializedString = JsonConvert.SerializeObject(asObject, typeof(T), null);

                    return ("application/json", bytes: System.Text.Encoding.UTF8.GetBytes(serializedString));
                }
                default:
                    throw new Exception("Unable to determine how to convert this into a byte array");
            }
        }
    }
}