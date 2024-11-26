using System;
using System.Net;

namespace KindMen.Uxios.Errors
{
    public class ProtocolError : Error
    {
        public HttpStatusCode Status => Response.Status;

        public ProtocolError(string message, Config config, IResponse response, Exception e) : base(message, config, response, e)
        {
        }
    }
}