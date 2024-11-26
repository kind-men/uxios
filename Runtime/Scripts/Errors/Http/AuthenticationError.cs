using System;

namespace KindMen.Uxios.Errors.Http
{
    /// <summary>
    /// Base class for all errors related to authentication. This allows clients to perform actions on any type of
    /// authentication error instead of checking forbidden or unauthenticated specifically.
    /// </summary>
    public abstract class AuthenticationError : HttpClientError
    {
        protected AuthenticationError(string message, Config config, IResponse response, Exception e) : base(message, config, response, e)
        {
        }
    }
}