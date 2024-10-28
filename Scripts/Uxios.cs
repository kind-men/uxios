using System;
using System.Collections.Generic;
using System.Net.Http;
using KindMen.Uxios.Interceptors;
using Newtonsoft.Json.Linq;
using RSG;
using UnityEngine;

namespace KindMen.Uxios
{
    namespace Interceptors
    {
        // TODO: rework these to have a 'use' method, eject and that it is a tuple of a success and error interceptor 
        public delegate Config RequestInterceptor(Config request);
        public delegate Response ResponseInterceptor(Response response);
        public delegate Error ErrorInterceptor(Error error);
    }

    // TODO: REPLACE PROMISES LIB WITH UNITASK??
    public sealed class Uxios
    {
        private static (
            List<(RequestInterceptor success, ErrorInterceptor error)> request,
            List<(ResponseInterceptor success, ErrorInterceptor error)> response
        ) interceptors = new();

        public static (
            List<(RequestInterceptor success, ErrorInterceptor error)> request,
            List<(ResponseInterceptor success, ErrorInterceptor error)> response
        ) Interceptors
        {
            get
            {
                // Force initialization of members so that we do not have to add checks in other places in the code.
                interceptors.request ??= new List<(RequestInterceptor success, ErrorInterceptor error)>();
                interceptors.response ??= new List<(ResponseInterceptor success, ErrorInterceptor error)>();

                return interceptors;
            }
        }

        private readonly IRequestRunner requestRunner;
        private readonly Config defaultConfig;

        public Uxios()
        {
            this.defaultConfig = new Config();
            
            // The default runner, but you could use a different one if you want
            requestRunner = WebRequestRunner.Instance();
        }

        public Uxios(IRequestRunner requestRunner)
        {
            this.defaultConfig = new Config();
            this.requestRunner = requestRunner;
        }

        public Promise<Response> Request<TData>(Config config) where TData : class
        {
            config.CreateUnityWebRequest<TData>();
            
            return requestRunner.PerformRequest(config);
        }

        public Promise<Response> Get(Uri url, Config config = null)
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Get;

            return Request<byte[]>(clone);
        }

        public Promise<Response> Get<TResponse>(Uri url, Config config = null)
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Get;
            clone!.ResponseType = ResolveExpectedResponseBasedOnType<TResponse>();

            return Request<byte[]>(clone);
        }

        private ExpectedResponse ResolveExpectedResponseBasedOnType<T>()
        {
            // TODO: is this the right location? Should it even be a separate service?
            if (typeof(T) == typeof(Texture2D)) return ExpectedResponse.Texture();
            if (typeof(T) == typeof(string)) return ExpectedResponse.Text();
            if (typeof(T) == typeof(JObject)) return ExpectedResponse.Json();
            if (typeof(T) == typeof(byte[])) return ExpectedResponse.ArrayBuffer();
            
            return ExpectedResponse.Json(typeof(T));
        }

        public Promise<Response> Delete(Uri url, Config config = null)
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Delete;

            return Request<byte[]>(clone);
        }

        public Promise<Response> Head(Uri url, Config config = null)
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Head;

            return Request<byte[]>(clone);
        }

        public Promise<Response> Options(Uri url, Config config = null)
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Options;

            return Request<byte[]>(clone);
        }

        public Promise<Response> Post(Uri url, object data = null, Config config = null)
        {
            return Post<byte[], byte[]>(url, data, config);
        }

        public Promise<Response> Post<TRequestData, TResponse>(
            Uri url, 
            object data = null, 
            Config config = null
        ) where TRequestData : class
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Post;
            clone!.Data = data;
            clone!.ResponseType = ResolveExpectedResponseBasedOnType<TResponse>();

            return Request<TRequestData>(clone);
        }

        public Promise<Response> Put(Uri url, object data, Config config = null)
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Put;
            clone!.Data = data;

            return Request<byte[]>(clone);
        }

        public Promise<Response> Patch(Uri url, object data, Config config = null)
        {
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Patch;
            clone!.Data = data;

            return Request<byte[]>(clone);
        }
    }
}