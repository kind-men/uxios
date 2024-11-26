using System;

namespace KindMen.Uxios
{
    public abstract class Error : Exception
    {
        public readonly Config Config;
        public readonly IResponse Response;

        protected Error(string message, Config config, IResponse response) : base(message)
        {
            Config = config;
            Response = response;
        }

        protected Error(string message, Config config, IResponse response, Exception innerException) : base(message, innerException)
        {
            Config = config;
            Response = response;
        }
    }
}