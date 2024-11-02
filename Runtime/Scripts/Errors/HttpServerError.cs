namespace KindMen.Uxios.Errors
{
    public class HttpServerError : ProtocolError
    {
        public HttpServerError(Config request, Response response) : base(request, response)
        {
        }

        public HttpServerError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}