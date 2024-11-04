using System.Net;
using KindMen.Uxios.Http;
using UnityEngine.Networking;

namespace KindMen.Uxios
{
    public abstract class Response
    {
        public object Data;
        public HttpStatusCode Status;
        public Headers Headers;
        public Config Config;
        public Request Request;

        public bool IsValid()
        {
            return Config.ValidateStatus.Invoke(Status);
        }
    }
}