using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using KindMen.Uxios.ExpectedTypesOfResponse;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using UnityEngine.Networking;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios
{
    public sealed class Config : ICloneable
    {
        public delegate object RequestTransformer(object data, Headers headers);
        public delegate object ResponseTransformer(object data);

        #region Ported axios fields
        public Uri Url;
        public HttpMethod Method = HttpMethod.Get;
        public Uri BaseUrl;
        public List<RequestTransformer> TransformRequest = new(); // TODO: Do something with this
        public List<ResponseTransformer> TransformResponse = new(); // TODO: Do something with this
        public Headers Headers = new();
        public QueryParameters Params = new();
        public object Data;
        public int Timeout = 0;
        public BasicAuthenticationCredentials Auth; // TODO: Do something with this; test using https://httpbin.org/#/Auth
        
        public ExpectedTypeOfResponse TypeOfResponseType = null;
        
        public Func<HttpStatusCode, bool> ValidateStatus = status => (int)status >= 200 && (int)status < 300;
        public int MaxRedirects = 5; // TODO: Test using https://httpbin.org/#/Redirects/get_absolute_redirect__n_
        public CancellationToken CancelToken;
        #endregion
        
        #region Unity specific fields
        public DownloadHandler DownloadHandler;
        internal UnityWebRequest UnityWebRequest;

        #endregion

        public void CreateUnityWebRequest<TData>() where TData : class
        {
            var urlBuilder = new UriBuilder(!Url.IsAbsoluteUri ? new Uri(BaseUrl, Url) : Url);
            urlBuilder.Query = QueryString.Merge(urlBuilder.Query.TrimStart('?'), Params.ToString());
            var url = urlBuilder.Uri;

            UnityWebRequest = new UnityWebRequest(url, Method.ToString());
            UnityWebRequest.timeout = Timeout;
            UnityWebRequest.redirectLimit = MaxRedirects;
            UnityWebRequest.downloadHandler = DownloadHandler ?? TypeOfResponseType switch
            {
                TextureResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                _ => new DownloadHandlerBuffer()
            };

            var (contentType, bytes) = ConvertToByteArray<TData>(Data);
            if (bytes != null)
            {
                UnityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
            }

            if (string.IsNullOrEmpty(contentType) == false)
            {
                UnityWebRequest.SetRequestHeader("Content-Type", contentType);
            }

            foreach (var header in Headers)
            {
                UnityWebRequest.SetRequestHeader(header.Key, header.Value);
            }
        }

        private (string contentType, byte[] bytes) ConvertToByteArray<T>(object data) where T : class
        {
            T dataToSend = data as T;
            if (data == null)
            {
                return (null, null);
            }
            
            if (dataToSend is byte[] asByteArray)
            {
                return ("application/octet-stream", bytes: asByteArray);
            } 
            
            if (dataToSend is string asString)
            {
                return ("text/plain", bytes: System.Text.Encoding.UTF8.GetBytes(asString));
            }
            
            if (dataToSend is object asObject)
            {
                // TODO: make setting configurable
                var serializedString = JsonConvert.SerializeObject(asObject, typeof(T), null);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(serializedString);
                return ("application/json", bytes);
            }

            throw new Exception("Unable to determine how to convert this into a byte array");
        }

        public object Clone()
        {
            return new Config
            {
                Url = Url != null ? new UriBuilder(Url).Uri : null,
                Method = Method,
                BaseUrl = BaseUrl != null ? new UriBuilder(BaseUrl).Uri : null,
                TransformRequest = TransformRequest.ToList(),
                TransformResponse = TransformResponse.ToList(),
                Headers = new Headers(Headers),
                Params = new QueryParameters(Params),
                Data = Data,
                Timeout = Timeout,
                Auth = Auth,
                TypeOfResponseType = TypeOfResponseType,
                ValidateStatus = ValidateStatus,
                MaxRedirects = MaxRedirects,
                CancelToken = CancelToken,
                DownloadHandler = DownloadHandler
            };
        }
    }
}