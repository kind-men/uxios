using System;

namespace KindMen.Uxios.Errors.Http
{
    public class NotFoundError : HttpClientError
    {
        public NotFoundError(string message, Config request, Response response, Exception e) : base(message, request, response, e)
        {
        }
    }
}