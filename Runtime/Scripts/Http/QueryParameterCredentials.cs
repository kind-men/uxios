using System.Collections.Specialized;

namespace KindMen.Uxios.Http
{
    public class QueryParameterCredentials : Credentials, ICredentialsUsingQueryString
    {
        private readonly string key;
        private readonly string value;

        public QueryParameterCredentials(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
        
        public QueryParameters ToQueryStringSegments()
        {
            return new QueryParameters { {key, value} };
        }
    }
}