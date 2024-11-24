using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using KindMen.Uxios.Interceptors;
using KindMen.Uxios.Transports;
using RSG;
using UnityEngine;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios
{
    public sealed class Uxios
    {
        public const string Version = "0.2.1";

        private static Uxios defaultInstance;
        public Interceptors.Interceptors Interceptors { get; } = new();

        private readonly IUxiosTransport uxiosTransport;
        private readonly Config defaultConfig;
        private readonly ExpectedTypeOfResponseResolver expectedTypeOfResponseResolver;

        /// <summary>
        /// Dictionary of a scheme => transport combination to determine which transport will handle
        /// a request based on the scheme of the url. 
        /// </summary>
        private Dictionary<string, IUxiosTransport> transports = new();

        private readonly AbortController abortController;

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
            ExpectedTypeOfResponseResolver expectedTypeOfResponseResolver = null,
            AbortController abortController = null
        ) {
            this.defaultConfig = config;
            this.expectedTypeOfResponseResolver = expectedTypeOfResponseResolver ?? new ExpectedTypeOfResponseResolver();
            this.abortController = abortController ? abortController : AbortController.Instance();

            RegisterTransport(transport ?? UnityWebRequestTransport.Instance());
            RegisterTransport(UnityPersistentDataTransport.Instance());

            // Load default Interceptors; we may add more later
            var jsonConverter = new JsonConverter();
            Interceptors.response.Add(new ResponseInterceptor(jsonConverter.OnResponseSuccess), jsonConverter.Priority);
        }

        public void RegisterTransport(IUxiosTransport transport)
        {
            foreach (var scheme in transport.SupportedSchemes)
            {
                transports.Add(scheme, transport);
            }
        }

        private Promise<Response> Request<TData>(Config config) where TData : class
        {
            // When the user or none of the other methods set a response type, grab the default one from the resolver
            config.TypeOfResponseType ??= expectedTypeOfResponseResolver.Resolve(config);

            var scheme = config.Url.IsAbsoluteUri ? config.Url.Scheme : config.BaseUrl.Scheme;

            // Ensure a request is cancellable
            var cancellationTokenSource = new CancellationTokenSource();
            config.CancelUsing(cancellationTokenSource);
            
            // Initiate the request on the transport - and thus obtain the promise
            var promise = transports[scheme].PerformRequest<TData>(config);
            
            // Provide the setup and deinitializiation on the abort controller
            abortController.Register(promise, cancellationTokenSource);
            promise.Finally(() => abortController.Unregister(promise));
            
            return promise;
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

        public void Abort(IPromiseInfo promise)
        {
            abortController.Abort(promise);
        }
        
        /// <summary>
        /// Sometimes you want a blocking flow inside a coroutine, this method will help to wait for the completion of a
        /// promise inside a Coroutine. Do mind, this is not a recommended practice as promises are more than capable of
        /// running their own course.
        ///
        /// The main function of this is when you depend on other libraries to do stuff after the request completed, or
        /// to assist in writing PlayMode tests where you need to run the asserts outside of the promise return
        /// functions / wait because otherwise NUnit has moved on; see our own tests at
        /// <see cref="KindMen.Uxios.Tests.GetRequests"/>.
        /// </summary>
        /// <param name="request">The promise to wait for</param>
        /// <returns>Custom yield instruction - this method can be as as part of a `yield return` clause</returns>
        public static CustomYieldInstruction WaitForRequest<TResponse>(IPromise<TResponse> request)
        {
            var promise = request as Promise<TResponse>;
            
            return new WaitUntil(() =>
            {
                // Signal to the abort controller that the Promise should be kept alive as long as this method is called
                // every frame
                AbortController.Instance().KeepAlive(promise);

                return promise?.CurState != PromiseState.Pending;
            });
        }

        public static CustomYieldInstruction WaitForRequest(IPromise request)
        {
            var promise = request as Promise;

            return new WaitUntil(() =>
            {
                // Signal to the abort controller that the Promise should be kept alive as long as this method is called
                // every frame
                AbortController.Instance().KeepAlive(promise);

                return promise?.CurState != PromiseState.Pending;
            });
        }
        
        /// <summary>
        /// Sometimes, external libraries do not know how to work with promises and just need an IEnumerator, so we
        /// provide a convenience method to wrap a promise inside a method that can be started as a Coroutine (or
        /// yielded when needed inside another).
        ///
        /// This is not a replacement for using the Then, Catch and Finally method 
        /// </summary>
        /// <param name="promise"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IEnumerator AsCoroutine<T>(IPromise<T> promise)
        {
            yield return WaitForRequest(promise);
        }
        
        private static IEnumerator AsCoroutine(IPromise promise)
        {
            yield return WaitForRequest(promise);
        }
    }
}