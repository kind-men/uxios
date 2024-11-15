using System;

namespace KindMen.Uxios.Errors
{
    public class ConnectionError : Error
    {
        public ConnectionError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}