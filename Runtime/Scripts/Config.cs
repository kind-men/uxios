using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using KindMen.Uxios.Http;
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
        
        /// <summary>
        /// The number of milliseconds after which a request will timeout, default is 0; meaning it will not timeout.
        ///
        /// Some transports, such as UnityWebRequest, do not support fractions of seconds. It is assumed that transports
        /// will round up to prevent accidentally setting the value to 0 - and thus no timeout.
        /// </summary>
        public int Timeout = 0;
        public Credentials Auth;
        
        public ExpectedTypeOfResponse TypeOfResponseType = null;
        
        public Func<HttpStatusCode, bool> ValidateStatus = status => (int)status >= 200 && (int)status < 300;
        public int MaxRedirects = 5;
        public CancellationToken CancelToken;
        #endregion

        public object Clone()
        {
            return new Config
            {
                Url = Url,
                Method = Method,
                BaseUrl = BaseUrl,
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
                CancelToken = CancelToken
            };
        }
    }
}