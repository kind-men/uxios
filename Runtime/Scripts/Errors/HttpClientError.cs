using System;

namespace KindMen.Uxios.Errors
{
    public class HttpClientError : ProtocolError
    {
        public HttpClientError(string message, Config config, Response response, Exception e) : base(message, config, response, e)
        {
        }
    }
}