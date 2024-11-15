namespace KindMen.Uxios.Errors.Http
{
    /// <summary>
    /// Base class for all errors related to authentication. This allows clients to perform actions on any type of
    /// authentication error instead of checking forbidden or unauthenticated specifically.
    /// </summary>
    public abstract class AuthenticationError : HttpClientError
    {
        protected AuthenticationError(Config request, Response response) : base(request, response)
        {
        }

        protected AuthenticationError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}