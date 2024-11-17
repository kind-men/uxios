using System;

namespace KindMen.Uxios.Errors
{
    public class ConnectionError : Error
    {
        public ConnectionError(string message, Config config, Response response) : base(message, config, response)
        {
        }
    }
}