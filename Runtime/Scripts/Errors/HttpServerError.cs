using System;

namespace KindMen.Uxios.Errors
{
    public class HttpServerError : ProtocolError
    {
        public HttpServerError(string message, Config request, Response response, Exception e) : base(message, request, response, e)
        {
        }
    }
}