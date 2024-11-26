using System;
using static KindMen.Uxios.Interceptors.Interceptors;

namespace KindMen.Uxios.Interceptors.NetworkInspector
{
    public abstract class Logger : ILogger, IDisposable
    {
        private bool disposed = false;

        private readonly RequestInterceptor requestInterceptor;
        private readonly ResponseInterceptor responseInterceptor;

        public Logger()
        {
            requestInterceptor = new RequestInterceptor(OnRequestSuccess, OnRequestError);
            responseInterceptor = new ResponseInterceptor(OnResponseSuccess, OnResponseError);

            Uxios.DefaultInstance.Interceptors.request.Add(requestInterceptor, DefaultLoggingPriority);
            Uxios.DefaultInstance.Interceptors.response.Add(responseInterceptor, DefaultLoggingPriority);
        }

        public void Dispose()
        {
            if (disposed) return;

            Uxios.DefaultInstance.Interceptors.request.Remove(requestInterceptor);
            Uxios.DefaultInstance.Interceptors.response.Remove(responseInterceptor);

            disposed = true;
        }
        
        public abstract Config OnRequestSuccess(Config config);
        public abstract Error OnRequestError(Error error);
        public abstract IResponse OnResponseSuccess(IResponse response);
        public abstract Error OnResponseError(Error error);
    }
}