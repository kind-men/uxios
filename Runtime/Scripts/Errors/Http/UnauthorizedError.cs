﻿using System;

namespace KindMen.Uxios.Errors.Http
{
    public class UnauthorizedError : AuthenticationError
    {
        public UnauthorizedError(string message, Config config, IResponse response, Exception e) : base(message, config, response, e)
        {
        }
    }
}