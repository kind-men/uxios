﻿namespace KindMen.Uxios.Interceptors.NetworkInspector
{
    public interface ILogger
    {
        public Config OnRequestSuccess(Config config);

        public Error OnRequestError(Error error);

        public IResponse OnResponseSuccess(IResponse response);

        public Error OnResponseError(Error error);
    }
}