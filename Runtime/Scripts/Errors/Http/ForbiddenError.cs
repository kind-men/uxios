using System;

namespace KindMen.Uxios.Errors.Http
{
    public class ForbiddenError : AuthenticationError
    {
        public ForbiddenError(string message, Config config, IResponse response, Exception e) : base(message, config, response, e)
        {
        }
    }
}