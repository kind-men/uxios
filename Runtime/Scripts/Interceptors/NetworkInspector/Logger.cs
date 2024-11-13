using System;

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

            // Loggers are expected to be ran last so that all other interceptors have done their thing; as such
            // we give them a priority index of 10.000
            Uxios.Interceptors.request.Add(requestInterceptor, 10000);
            Uxios.Interceptors.response.Add(responseInterceptor, 10000);
        }

        public void Dispose()
        {
            if (disposed) return;

            Uxios.Interceptors.request.Remove(requestInterceptor);
            Uxios.Interceptors.response.Remove(responseInterceptor);

            disposed = true;
        }
        
        public abstract Config OnRequestSuccess(Config config);
        public abstract Error OnRequestError(Error error);
        public abstract Response OnResponseSuccess(Response response);
        public abstract Error OnResponseError(Error error);
    }
}