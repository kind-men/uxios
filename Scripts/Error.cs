using System;

namespace KindMen.Uxios
{
    public class Error : Exception
    {
        public readonly Config Request;
        public readonly Response Response;

        public Error(Config request, Response response) : base()
        {
            Request = request;
            Response = response;
        }

        public Error(string message, Config request, Response response) : base(message)
        {
            Request = request;
            Response = response;
        }
    }
}