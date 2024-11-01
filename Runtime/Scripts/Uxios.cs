using System;
using System.Net.Http;
using KindMen.Uxios.Interceptors;
using Newtonsoft.Json.Linq;
using RSG;
using UnityEngine;

namespace KindMen.Uxios
{
    public sealed class Uxios
    {
        public static Interceptors.Interceptors Interceptors { get; } = new();

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
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Post;
            clone!.Data = data;
            clone!.ResponseType = ResolveExpectedResponseBasedOnType<TResponse>();

            return Request<TRequestData>(clone);
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
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Put;
            clone!.Data = data;
            clone!.ResponseType = ResolveExpectedResponseBasedOnType<TResponse>();

            return Request<TRequestData>(clone);
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
            config ??= this.defaultConfig;

            var clone = config.Clone() as Config;
            clone!.Url = url;
            clone!.Method = HttpMethod.Patch;
            clone!.Data = data;
            clone!.ResponseType = ResolveExpectedResponseBasedOnType<TResponse>();

            return Request<TRequestData>(clone);
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
        public static CustomYieldInstruction WaitForRequest(Promise<Response> request)
        {
            return new WaitUntil(() => request.CurState != PromiseState.Pending);
        }
    }
}