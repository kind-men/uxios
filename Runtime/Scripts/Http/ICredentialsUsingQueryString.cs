using System.Collections.Specialized;

namespace KindMen.Uxios.Http
{
    public interface ICredentialsUsingQueryString
    {
        public QueryParameters ToQueryStringSegments();
    }
}