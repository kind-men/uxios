using System;

namespace KindMen.Uxios.Errors
{
    public class HttpClientError : ProtocolError
    {
        public HttpClientError(string message, Config request, Response response, Exception e) : base(message, request, response, e)
        {
        }
    }
}