using System;

namespace KindMen.Uxios.Errors
{
    public class HttpServerError : ProtocolError
    {
        public HttpServerError(string message, Config config, Response response, Exception e) : base(message, config, response, e)
        {
        }
    }
}