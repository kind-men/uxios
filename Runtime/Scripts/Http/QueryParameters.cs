using System.Collections.Specialized;

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

        public QueryParameters(string queryParameters)
        {
            this.Add(QueryString.Decode(queryParameters));
        }

        public override string ToString()
        {
            return QueryString.Encode(this);
        }
    }
}