using System;

namespace KindMen.Uxios.Errors.Http
{
    public class UnauthorizedError : AuthenticationError
    {
        public UnauthorizedError(string message, Config request, Response response, Exception e) : base(message, request, response, e)
        {
        }
    }
}