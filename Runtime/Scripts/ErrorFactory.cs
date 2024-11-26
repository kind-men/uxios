using System;
using KindMen.Uxios.Errors;
using KindMen.Uxios.Errors.Http;

namespace KindMen.Uxios
{
    public static class ErrorFactory
    {
        public static Error Create(IResponse response, Exception e = null)
        {
            return (int)response.Status switch
            {
                404 => new NotFoundError(response.Data as string, response.Config, response, e),
                401 => new UnauthorizedError(response.Data as string, response.Config, response, e),
                403 => new ForbiddenError(response.Data as string, response.Config, response, e),
                >= 400 and < 500 => new HttpClientError(response.Data as string, response.Config, response, e),
                >= 500 and < 600 => new HttpServerError(response.Data as string, response.Config, response, e),
                _ => new ProtocolError(e.Message, response.Config, response, e)
            };
        }
    }
}