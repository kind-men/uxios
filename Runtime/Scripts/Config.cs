using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using KindMen.Uxios.Http;
using Newtonsoft.Json;

namespace KindMen.Uxios
{
    public sealed class Config
    {
        #region Ported axios fields
        public Uri Url;
        public HttpMethod Method = HttpMethod.Get;
        public Uri BaseUrl;
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
        
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public ExpectedTypeOfResponse TypeOfResponseType = null;

        public int MaxRedirects = 5;
        [JsonIgnore] public Func<HttpStatusCode, bool> ValidateStatus = status => (int)status >= 200 && (int)status < 300;
        [JsonIgnore] public CancellationToken CancelToken = CancellationToken.None;
        #endregion

        public IEnvironment Environment { get; set; }
        
        public static Config Default()
        {
            return new Config();
        }
        
        public static Config BasedOn(Config config)
        {
            return new Config
            {
                // Uri's are immutable - so I do not have to copy it
                Url = config.Url,
                Method = config.Method,
                BaseUrl = config.BaseUrl,
                // Headers and Params are not immutable, and collection types, so they need to be duplicated
                Headers = new Headers(config.Headers),
                Params = new QueryParameters(config.Params),
                Data = config.Data,
                Timeout = config.Timeout,
                Auth = config.Auth,
                TypeOfResponseType = config.TypeOfResponseType,
                ValidateStatus = config.ValidateStatus,
                MaxRedirects = config.MaxRedirects,
                CancelToken = config.CancelToken,
                Environment = config.Environment
            };
        }
        
        public Config At(Uri url)
        {
            Url = url;

            return this;
        }

        public Config At(Uri url, Uri baseUrl)
        {
            Url = url;
            BaseUrl ??= baseUrl;

            return this;
        }

        public Config UsingMethod(HttpMethod method)
        {
            Method = method;

            return this;
        }

        public Config AddParam(string key, string value)
        {
            Params.Add(key, value);

            return this;
        }

        public Config AddHeader(string key, string value)
        {
            Headers.Add(key, value);

            return this;
        }

        public Config AddHeader(Header header)
        {
            Headers.Add(header);

            return this;
        }

        public Config CancelUsing(CancellationTokenSource cancellationSource)
        {
            CancelToken = cancellationSource.Token;

            return this;
        }
        
        public Config WithPayload<TPayload>(TPayload payload) where TPayload : class
        {
            if (Environment is not Environment<TPayload> env)
            {
                env = new Environment<TPayload>();
                Environment = env;
            }
            env.Payload = payload;

            return this;
        }
        
        public TPayload GetPayload<TPayload>() where TPayload : class
        {
            return (Environment as Environment<TPayload>)?.Payload;
        }
    }
}