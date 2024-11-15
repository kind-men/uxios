using KindMen.Uxios.Errors;
using KindMen.Uxios.Errors.Http;

namespace KindMen.Uxios
{
    public static class ErrorFactory
    {
        public static Error Create(Response response)
        {
            return (int)response.Status switch
            {
                404 => new NotFoundError((string)response.Data, response.Config, response),
                401 => new UnauthorizedError((string)response.Data, response.Config, response),
                403 => new ForbiddenError((string)response.Data, response.Config, response),
                >= 400 and < 500 => new HttpClientError((string)response.Data, response.Config, response),
                >= 500 and < 600 => new HttpServerError((string)response.Data, response.Config, response),
                _ => new ProtocolError((string)response.Data, response.Config, response)
            };
        }
    }
}