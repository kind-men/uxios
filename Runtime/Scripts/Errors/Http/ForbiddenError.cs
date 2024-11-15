namespace KindMen.Uxios.Errors.Http
{
    public class ForbiddenError : AuthenticationError
    {
        public ForbiddenError(Config request, Response response) : base(request, response)
        {
        }

        public ForbiddenError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}