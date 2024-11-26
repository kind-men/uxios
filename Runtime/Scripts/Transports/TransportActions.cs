using System;
using KindMen.Uxios.Errors;
using KindMen.Uxios.ExpectedTypesOfResponse;
using Newtonsoft.Json;
using RSG;

namespace KindMen.Uxios.Transports
{
    public static class TransportActions
    {
        public static Request CreateRequest<TData>(ref Config config) where TData : class
        {
            // Clone config so that any changes we do - do not affect the caller; as this may mess with the global
            // config if that is used
            config = Config.BasedOn(config);

            config = ApplyRequestInterceptors(config);

            return Request.FromConfig<TData>(config);
        }

        public static void HandleResponse(
            Promise<IResponse> promise, 
            Config config, 
            Request uxiosRequest, 
            Func<Config, Request, IResponse> responseCreator
        )
        {
            IResponse response = null;
            
            // If an exception occurs in the whole response interpretation chain, reject the promise
            try
            {
                response = responseCreator(config, uxiosRequest);
                response = ApplyResponseInterceptors(response);

                if (response.IsValid() == false)
                {
                    RejectResponse(promise, response);
                    return;
                }

                promise.Resolve(response);
            }
            catch (JsonReaderException e)
            {
                RejectWithErrorDuringResponse(
                    promise, 
                    new DataProcessingError(e.Message, config, response)
                );
            }
            catch (Exception e)
            {
                RejectWithErrorDuringResponse(
                    promise, 
                    response != null 
                        ? ErrorFactory.Create(response, e) 
                        : new DataProcessingError(e.Message, config, null, e)
                );
            }
        }

        public static Config ApplyRequestInterceptors(Config config)
        {
            foreach (var requestInterceptor in Uxios.DefaultInstance.Interceptors.request)
            {
                config = requestInterceptor.success.Invoke(config);
            }

            return config;
        }

        public static IResponse ApplyResponseInterceptors(IResponse response)
        {
            foreach (var responseInterceptor in Uxios.DefaultInstance.Interceptors.response)
            {
                response = responseInterceptor.success.Invoke(response);
            }

            return response;
        }
        
        public static void RejectResponse(Promise<IResponse> promise, IResponse response)
        {
            RejectWithErrorDuringResponse(promise, ErrorFactory.Create(response));
        }

        public static void RejectWithErrorDuringRequest(Promise<IResponse> promise, Error error)
        {
            foreach (var interceptor in Uxios.DefaultInstance.Interceptors.request)
            {
                error = interceptor.error.Invoke(error);
            }

            promise.Reject(error);
        }

        public static void RejectWithErrorDuringResponse(Promise<IResponse> promise, Error error)
        {
            foreach (var responseInterceptor in Uxios.DefaultInstance.Interceptors.response)
            {
                error = responseInterceptor.error.Invoke(error);
            }

            promise.Reject(error);
        }
    }
}