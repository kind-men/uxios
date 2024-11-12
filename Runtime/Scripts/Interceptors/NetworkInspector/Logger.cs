using System;

namespace KindMen.Uxios.Interceptors.NetworkInspector
{
    public abstract class Logger : ILogger, IDisposable
    {
        private bool disposed = false;

        private RequestInterceptor requestInterceptor;
        private ResponseInterceptor responseInterceptor;

        public Logger()
        {
            requestInterceptor = new RequestInterceptor(OnRequestSuccess, OnRequestError);
            responseInterceptor = new ResponseInterceptor(OnResponseSuccess, OnResponseError);

            Uxios.Interceptors.request.Add(requestInterceptor);
            Uxios.Interceptors.response.Add(responseInterceptor);
        }

        public void Dispose()
        {
            if (disposed) return;

            Uxios.Interceptors.request.Remove(requestInterceptor);
            Uxios.Interceptors.response.Remove(responseInterceptor);

            disposed = true;
        }
        
        public abstract Config OnRequestSuccess(Config request);
        public abstract Error OnRequestError(Error error);
        public abstract Response OnResponseSuccess(Response response);
        public abstract Error OnResponseError(Error error);
    }
}