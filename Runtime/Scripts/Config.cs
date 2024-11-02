using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using KindMen.Uxios.Http;
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
        public int Timeout = 30;
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