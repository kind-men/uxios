using System;
using System.Net;

namespace KindMen.Uxios.Errors
{
    public class ProtocolError : Error
    {
        public HttpStatusCode Status => Response.Status;

        public ProtocolError(string message, Config request, Response response, Exception e) : base(message, request, response, e)
        {
        }
    }
}