using System;

namespace KindMen.Uxios.Errors
{
    public class DataProcessingError : Error
    {
        public DataProcessingError(string message, Config config, Response response) : base(message, config, response)
        {
        }

        public DataProcessingError(string message, Config config, Response response, Exception innerException) : base(message, config, response, innerException)
        {
        }
    }
}