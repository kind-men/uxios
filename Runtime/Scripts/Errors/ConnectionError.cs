namespace KindMen.Uxios.Errors
{
    public class ConnectionError : Error
    {
        public ConnectionError(Config request, Response response) : base(request, response)
        {
        }

        public ConnectionError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}