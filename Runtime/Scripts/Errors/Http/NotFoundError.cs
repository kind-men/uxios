using System;

namespace KindMen.Uxios.Errors.Http
{
    public class NotFoundError : HttpClientError
    {
        public NotFoundError(string message, Config config, Response response, Exception e) : base(message, config, response, e)
        {
        }
    }
}