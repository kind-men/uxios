using System;

namespace KindMen.Uxios
{
    public abstract class Error : Exception
    {
        public readonly Config Request;
        public readonly Response Response;

        protected Error(string message, Config request, Response response) : base(message)
        {
            Request = request;
            Response = response;
        }

        protected Error(string message, Config request, Response response, Exception innerException) : base(message, innerException)
        {
            Request = request;
            Response = response;
        }
    }
}