using System;
using System.Net.Http;
using KindMen.Uxios.Interceptors;
using KindMen.Uxios.Transports;
using RSG;
using UnityEngine;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios
{
    public sealed class Uxios
    {
        private static Uxios defaultInstance;
        public Interceptors.Interceptors Interceptors { get; } = new();

        private readonly IUxiosTransport uxiosTransport;
        private readonly Config defaultConfig;
        private readonly ExpectedTypeOfResponseResolver expectedTypeOfResponseResolver;

        /// <summary>
        /// Although it is recommended to inject an Uxios instance where you need it, you can also access a global
        /// DefaultInstance. This is mostly used internally by the Resource and Collection object proxies, but if needed
        /// others could use this too.
        /// </summary>
        public static Uxios DefaultInstance => defaultInstance ??= new Uxios();

        /// <summary>
        /// Default constructor - when all you need is to make HTTP Requests.
        /// </summary>
        public Uxios() : this(config: new Config())
        {
        }

        /// <summary>
        /// Advanced Constructor - when you want to pass a default config or even your own custom services for
        /// customizing Uxios' interactions or response resolving.
        /// </summary>
        public Uxios(
            Config config,
            IUxiosTransport transport = null, 
            ExpectedTypeOfResponseResolver expectedTypeOfResponseResolver = null
        ) {
            this.defaultConfig = config;
            this.uxiosTransport = transport ?? UnityWebRequestTransport.Instance();
            this.expectedTypeOfResponseResolver = expectedTypeOfResponseResolver ?? new ExpectedTypeOfResponseResolver();

            // Load default Interceptors; we may add more later
            var jsonConverter = new JsonConverter();
            Interceptors.response.Add(new ResponseInterceptor(jsonConverter.OnResponseSuccess), jsonConverter.Priority);
        }

        private Promise<Response> Request<TData>(Config config) where TData : class
        {
            // When the user or none of the other methods set a response type, grab the default one from the resolver
            config.TypeOfResponseType ??= expectedTypeOfResponseResolver.Resolve(config); 

            return uxiosTransport.PerformRequest<TData>(config);
        }

        public Promise<Response> Get(Uri url, Config config = null)
        {
            config = Config.BasedOn(config ?? defaultConfig)
                .At(url)
                .UsingMethod(HttpMethod.Get);

            return Request<byte[]>(config);
        }

        public Promise<Response> Get(Uri url, QueryParameters parameters, Config config = null)
        {
            config = Config.BasedOn(config ?? defaultConfig);
            config.Params = parameters;

            return Get(url, config);
        }

        public Promise<Response> Get<TResponse>(Uri url, Config config = null)
        {
            config = Config.BasedOn(config ?? defaultConfig);

            config.At(url).UsingMethod(HttpMethod.Get);
            config.TypeOfResponseType = expectedTypeOfResponseResolver.Resolve<TResponse>(config);

            return Request<byte[]>(config);
        }

        public Promise<Response> Get<TResponse>(Uri url, QueryParameters parameters, Config config = null)
        {
            config = Config.BasedOn(config ?? defaultConfig);
            config.Params = parameters;

            return Get<TResponse>(url, config);
        }

        public Promise<Response> Delete(Uri url, Config config = null)
        {
            config = Config.BasedOn(config ?? defaultConfig);
            config.At(url).UsingMethod(HttpMethod.Delete);

            return Request<byte[]>(config);
        }

        public Promise<Response> Head(Uri url, Config config = null)
        {
            config = Config.BasedOn(config ?? defaultConfig);
            config.At(url).UsingMethod(HttpMethod.Head);

            return Request<byte[]>(config);
        }

        public Promise<Response> Options(Uri url, Config config = null)
        {
            config = Config.BasedOn(config ?? defaultConfig);
            config.At(url).UsingMethod(HttpMethod.Options);

            return Request<byte[]>(config);
        }

        public Promise<Response> Post(Uri url, byte[] data = null, Config config = null)
        {
            return Post<byte[], byte[]>(url, data, config);
        }

        public Promise<Response> Post(Uri url, string data = null, Config config = null)
        {
            return Post<string, byte[]>(url, data, config);
        }

        public Promise<Response> Post<TRequestData, TResponse>(
            Uri url, 
            TRequestData data = null, 
            Config config = null
        ) where TRequestData : class
        {
            config = Config.BasedOn(config ?? defaultConfig);
            config.At(url).UsingMethod(HttpMethod.Post);
            config.Data = data;
            config.TypeOfResponseType = expectedTypeOfResponseResolver.Resolve<TResponse>(config);

            return Request<TRequestData>(config);
        }

        public Promise<Response> Put(Uri url, byte[] data, Config config = null)
        {
            return Put<byte[], byte[]>(url, data, config);
        }

        public Promise<Response> Put(Uri url, string data, Config config = null)
        {
            return Put<string, byte[]>(url, data, config);
        }

        public Promise<Response> Put<TRequestData, TResponse>(
            Uri url, 
            TRequestData data = null, 
            Config config = null
        ) where TRequestData : class
        {
            config = Config.BasedOn(config ?? defaultConfig);
            
            config.At(url).UsingMethod(HttpMethod.Put);
            config.Data = data;
            config.TypeOfResponseType = expectedTypeOfResponseResolver.Resolve<TResponse>(config);

            return Request<TRequestData>(config);
        }

        public Promise<Response> Patch(Uri url, byte[] data, Config config = null)
        {
            return Patch<byte[], byte[]>(url, data, config);
        }

        public Promise<Response> Patch(Uri url, string data, Config config = null)
        {
            return Patch<string, byte[]>(url, data, config);
        }

        public Promise<Response> Patch<TRequestData, TResponse>(
            Uri url, 
            TRequestData data = null, 
            Config config = null
        ) where TRequestData : class
        {
            config = Config.BasedOn(config ?? defaultConfig);
            
            config.At(url).UsingMethod(HttpMethod.Patch);

            config.Data = data;
            config.TypeOfResponseType = expectedTypeOfResponseResolver.Resolve<TResponse>(config);

            return Request<TRequestData>(config);
        }

        /// <summary>
        /// Sometimes you want a blocking flow inside a coroutine, this method will help to wait for the completion of a
        /// promise inside a CoRoutine. Do mind, this is not a recommended practice as promises are more than capable of
        /// running their own course.
        ///
        /// The main function of this is when you depend on other libraries to do stuff after the request completed, or
        /// to assist in writing PlayMode tests where you need to run the asserts outside of the promise return
        /// functions / wait because otherwise NUnit has moved on; see our own tests at
        /// <see cref="KindMen.Uxios.Tests.GetRequests"/>.
        /// </summary>
        /// <param name="request">The promise to wait for</param>
        /// <returns>Custom yield instruction - this method can be as as part of a `yield return` clause</returns>
        public static CustomYieldInstruction WaitForRequest<TResponse>(Promise<TResponse> request)
        {
            return new WaitUntil(() => request.CurState != PromiseState.Pending);
        }

        public static CustomYieldInstruction WaitForRequest(IPromise request)
        {
            return new WaitUntil(() => ((Promise)request).CurState != PromiseState.Pending);
        }
    }
}