namespace KindMen.Uxios.Interceptors
{
    public sealed class RequestInterceptor
    {
        public readonly RequestInterception success;
        public readonly ErrorInterception error;

        public RequestInterceptor(RequestInterception success, ErrorInterception error)
        {
            this.success = success;
            this.error = error;
        }

        public RequestInterceptor(RequestInterception success)
        {
            this.success = success;
            this.error = arg => arg;
        }

        public RequestInterceptor(ErrorInterception error)
        {
            this.success = arg => arg;
            this.error = error;
        }
    }
}