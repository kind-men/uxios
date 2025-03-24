using System.Collections.Generic;

namespace KindMen.Uxios.Http
{
    public sealed class Headers : Dictionary<string, string>
    {
        // A non-exhaustive list of known header keys to help developers with selecting an appropriate header
        public const string RequestId = "X-Request-Id";
        public const string ContentType = "Content-Type";
        public const string Accept = "Accept";
        public const string UserAgent = "User-Agent";
        public const string Authorization = "Authorization";
        
        public Headers()
        {
        }

        public void Add(Header header)
        {
            this.Add(header.Name, header.Value);
        }

        public Headers(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }
    }
}