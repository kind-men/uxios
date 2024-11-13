using System.Net;
using KindMen.Uxios.Http;

namespace KindMen.Uxios
{
    /// <summary>
    /// Represents the response of an HTTP request in Uxios.
    /// </summary>
    public abstract class Response
    {
        /// <summary>
        /// Gets the HTTP status code returned by the server.
        /// Indicates the success or failure of the request (e.g., 200 for success, 404 for not found).
        /// </summary>
        public HttpStatusCode Status;
        
        /// <summary>
        /// Gets the headers returned in the response.
        /// Contains key-value pairs representing HTTP headers, such as "Content-Type" or "Authorization".
        /// </summary>
        public Headers Headers;

        /// <summary>
        /// Gets the original request that initiated this response.
        /// Provides access to details about the request, such as the URL and HTTP method.
        /// </summary>
        public Request Request;

        /// <summary>
        /// Gets the configuration settings used for the request.
        /// This includes options such as timeout settings, custom headers, and validation rules.
        /// </summary>        
        public Config Config;

        /// <summary>
        /// Contains the data returned by the request. 
        /// This is the deserialized content of the response, typically the class you provided, and its type varies
        /// based on the request configuration.
        /// </summary>
        public object Data;

        /// <summary>
        /// Validates the response status using the validation function defined in the configuration.
        /// Checks if the status code meets the expected range or criteria for a successful response.
        /// </summary>
        /// <returns>True if the status code is valid according to the configuration, otherwise false.</returns>
        public bool IsValid()
        {
            return Config.ValidateStatus.Invoke(Status);
        }
    }
}