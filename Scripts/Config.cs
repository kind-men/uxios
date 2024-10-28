using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using UnityEngine.Networking;

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
        public QueryParameters Params = new(); // TODO: Do something with this
        public object Data;
        public int Timeout = 0;
        public BasicAuthenticationCredentials Auth; // TODO: Do something with this
        
        public ExpectedResponse ResponseType = ExpectedResponse.Json();
        
        public Func<HttpStatusCode, bool> ValidateStatus = status => (int)status >= 200 && (int)status < 300;
        public int MaxRedirects = 5;
        public CancellationToken CancelToken;
        #endregion
        
        #region Unity specific fields
        public DownloadHandler DownloadHandler;
        internal UnityWebRequest UnityWebRequest;
        #endregion

        public void CreateUnityWebRequest<TData>() where TData : class
        {
            var url = !Url.IsAbsoluteUri ? new Uri(BaseUrl, Url) : Url;

            UnityWebRequest = new UnityWebRequest(url, Method.ToString());
            UnityWebRequest.timeout = Timeout;
            UnityWebRequest.redirectLimit = MaxRedirects;
            // TODO: Fetch "Accept" header from ResponseType and if no Accept header was set: set it here
            UnityWebRequest.downloadHandler = DownloadHandler ?? ResponseType switch
            {
                ExpectTextureResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                _ => new DownloadHandlerBuffer()
            };

            var bytes = ConvertToByteArray<TData>(Data);
            if (bytes != null)
            {
                UnityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
            }

            foreach (var header in Headers)
            {
                UnityWebRequest.SetRequestHeader(header.Key, header.Value);
            }
        }

        private byte[] ConvertToByteArray<T>(object data) where T : class
        {
            T dataToSend = data as T;
            byte[] bytes = null;
            if (dataToSend is byte[] asByteArray)
            {
                bytes = asByteArray;
            } 
            else if (dataToSend is string asString)
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(asString);
            }
            else if (dataToSend is object asObject)
            {
                // TODO: make setting configurable
                var serializedString = JsonConvert.SerializeObject(asObject, typeof(T), null);
                bytes = System.Text.Encoding.UTF8.GetBytes(serializedString);
            }

            return bytes;
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
                ResponseType = ResponseType,
                ValidateStatus = ValidateStatus,
                MaxRedirects = MaxRedirects,
                CancelToken = CancelToken,
                DownloadHandler = DownloadHandler
            };
        }
    }
}