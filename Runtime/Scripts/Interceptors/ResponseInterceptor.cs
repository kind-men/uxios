namespace KindMen.Uxios.Interceptors
{
    public sealed class ResponseInterceptor
    {
        public readonly ResponseInterception success;
        public readonly ErrorInterception error;

        public ResponseInterceptor(ResponseInterception success, ErrorInterception error)
        {
            this.success = success;
            this.error = error;
        }

        public ResponseInterceptor(ResponseInterception success)
        {
            this.success = success;
            this.error = arg => arg;
        }

        public ResponseInterceptor(ErrorInterception error)
        {
            this.success = arg => arg;
            this.error = error;
        }
    }
}