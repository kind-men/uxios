using System.Collections.Generic;

namespace KindMen.Uxios.Interceptors
{
    public delegate Config RequestInterception(Config request);
    public delegate Response ResponseInterception(Response response);
    public delegate Error ErrorInterception(Error error);

    public sealed class Interceptors
    {
        public readonly PriorityList<RequestInterceptor> request = new();
        public readonly PriorityList<ResponseInterceptor> response = new();
    }
}