using System.Collections.Generic;

namespace KindMen.Uxios.Interceptors
{
    public delegate Config RequestInterception(Config request);
    public delegate Response ResponseInterception(Response response);
    public delegate Error ErrorInterception(Error error);

    public sealed class Interceptors
    {
        public readonly List<RequestInterceptor> request = new();
        public readonly List<ResponseInterceptor> response = new();
    }
}