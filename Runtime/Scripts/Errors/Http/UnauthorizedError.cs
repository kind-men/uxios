namespace KindMen.Uxios.Errors.Http
{
    public class UnauthorizedError : AuthenticationError
    {
        public UnauthorizedError(Config request, Response response) : base(request, response)
        {
        }

        public UnauthorizedError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}