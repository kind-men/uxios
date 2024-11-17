using System;

namespace KindMen.Uxios
{
    public abstract class Error : Exception
    {
        public readonly Config Config;
        public readonly Response Response;

        protected Error(string message, Config config, Response response) : base(message)
        {
            Config = config;
            Response = response;
        }

        protected Error(string message, Config config, Response response, Exception innerException) : base(message, innerException)
        {
            Config = config;
            Response = response;
        }
    }
}