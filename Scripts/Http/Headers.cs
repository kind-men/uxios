using System.Collections.Generic;

namespace KindMen.Uxios.Http
{
    public sealed class Headers : Dictionary<string, string>
    {
        public Headers()
        {
        }

        public Headers(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }
    }
}