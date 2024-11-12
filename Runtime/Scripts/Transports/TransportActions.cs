using System;
using RSG;

namespace KindMen.Uxios.Transports
{
    public static class TransportActions
    {
        public static Request CreateRequest<TData>(ref Config config) where TData : class
        {
            // Clone config so that any changes we do - do not affect the caller; as this may mess with the global
            // config if that is used
            config = config.Clone() as Config;

            config = ApplyRequestInterceptors(config);

            return Request.FromConfig<TData>(config);
        }

        public static void HandleResponse(
            Promise<Response> promise, 
            Config config, 
            Request uxiosRequest, 
            Func<Config, Request, Response> responseCreator
        )
        {
            // If an exception occurs in the whole response interpretation chain, reject the promise
            try
            {
                Response response = responseCreator(config, uxiosRequest);

                if (response.IsValid() == false)
                {
                    RejectResponse(promise, response);
                    return;
                }

                promise.Resolve(ApplyResponseInterceptors(response));
            }
            catch (Exception e)
            {
                RejectWithErrorDuringResponse(promise, new Error(e.Message, config, null));
            }
        }

        public static Config ApplyRequestInterceptors(Config config)
        {
            foreach (var requestInterceptor in Uxios.Interceptors.request)
            {
                config = requestInterceptor.success.Invoke(config);
            }

            return config;
        }

        public static Response ApplyResponseInterceptors(Response response)
        {
            foreach (var responseInterceptor in Uxios.Interceptors.response)
            {
                response = responseInterceptor.success.Invoke(response);
            }

            return response;
        }
        
        public static void RejectResponse(Promise<Response> promise, Response response)
        {
            RejectWithErrorDuringResponse(promise, ErrorFactory.Create(response));
        }

        public static void RejectWithErrorDuringRequest(Promise<Response> promise, Error error)
        {
            foreach (var interceptor in Uxios.Interceptors.request)
            {
                error = interceptor.error.Invoke(error);
            }

            promise.Reject(error);
        }

        public static void RejectWithErrorDuringResponse(Promise<Response> promise, Error error)
        {
            foreach (var responseInterceptor in Uxios.Interceptors.response)
            {
                error = responseInterceptor.error.Invoke(error);
            }

            promise.Reject(error);
        }
    }
}