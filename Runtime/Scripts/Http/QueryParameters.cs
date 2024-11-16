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

        /// <summary>
        /// Query parameters can be consumed when they are absorbed into another part of the url, such as a
        /// URI Template part. This will have as effect that the parameter with the given name will be returned and
        /// removed from this collection.
        /// </summary>
        /// <returns>string or null if none is found</returns>
        public string Consume(string uriTemplatePart)
        {
            var parameter = this.Get(uriTemplatePart);
            if (parameter == null) return null;
            
            Remove(uriTemplatePart);
            return parameter;
        }
    }
}