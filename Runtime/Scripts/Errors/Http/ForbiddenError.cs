using System;

namespace KindMen.Uxios.Errors.Http
{
    public class ForbiddenError : AuthenticationError
    {
        public ForbiddenError(string message, Config request, Response response, Exception e) : base(message, request, response, e)
        {
        }
    }
}