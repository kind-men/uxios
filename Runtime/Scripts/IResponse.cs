using System.Net;
using KindMen.Uxios.Http;

namespace KindMen.Uxios
{
    public interface IResponse
    {
        public HttpStatusCode Status { get; }

        public Headers Headers { get; }

        public Request Request { get; }

        public object Data { get; set; }

        public bool IsValid();
        
        public Config Config { get; }
    }
}