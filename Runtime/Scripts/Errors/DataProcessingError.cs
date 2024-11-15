using System;

namespace KindMen.Uxios.Errors
{
    public class DataProcessingError : Error
    {
        public DataProcessingError(string message, Config request, Response response) : base(message, request, response)
        {
        }

        public DataProcessingError(string message, Config request, Response response, Exception innerException) : base(message, request, response, innerException)
        {
        }
    }
}