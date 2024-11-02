namespace KindMen.Uxios.Errors
{
    public class HttpClientError : ProtocolError
    {
        public HttpClientError(Config request, Response response) : base(request, response)
        {
        }

        public HttpClientError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}