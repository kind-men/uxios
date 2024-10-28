using System.Collections.Specialized;
using JetBrains.Annotations;

namespace KindMen.Uxios.Http
{
    public sealed class QueryParameters : NameValueCollection
    {
        public QueryParameters()
        {
        }

        public QueryParameters(QueryParameters col) : base(col)
        {
        }
    }
}