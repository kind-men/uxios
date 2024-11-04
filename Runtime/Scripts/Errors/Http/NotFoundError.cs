namespace KindMen.Uxios.Errors.Http
{
    public class NotFoundError : HttpClientError
    {
        public NotFoundError(Config request, Response response) : base(request, response)
        {
        }

        public NotFoundError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}