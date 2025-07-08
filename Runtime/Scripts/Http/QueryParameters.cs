using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace KindMen.Uxios.Http
{
    [System.Serializable]
    public class QueryParameters : Dictionary<string, QueryParameter>
    {
        private static readonly IEnumerable<string> cachedEmptyValuesArray = Array.Empty<string>();

        public IEnumerable<string> GetValues(string key)
            => TryGetValue(key, out var param)
                ? param.Values
                : cachedEmptyValuesArray;

        public int Length => Count;

        public QueryParameters()
        {
        }

        public QueryParameters(int expectedKeys) : base(expectedKeys) { }

        public QueryParameters(QueryParameters col) : base(col)
        {
        }

        public QueryParameters(string queryParameters) : this(QueryString.Decode(queryParameters))
        {
        }

        public void Add(string key, string value)
        {
            if (!TryGetValue(key, out var list))
            {
                list = new QueryParameter(key);
                base[key] = list;
            }

            list.Add(value);
        }

        public void Add(QueryParameters other)
        {
            foreach (var kv in other)
            {
                foreach (var v in kv.Value)
                {
                    Add(kv.Key, v);
                }
            }
        }

        public void Set(string key, string value)
        {
            if (!TryGetValue(key, out var param))
            {
                base[key] = new QueryParameter(key, value);
                return;
            }

            param.Set(value);
        }

        public void Set(string key, List<string> value)
        {
            if (!TryGetValue(key, out var param))
            {
                base[key] = new QueryParameter(key, value);
                return;
            }

            param.Set(value);
        }

        [CanBeNull]
        public QueryParameter Get(string key)
        {
            if (!TryGetValue(key, out QueryParameter param)) return null;
            
            return param;
        }

        [CanBeNull]
        public string Single(string key)
        {
            if (!TryGetValue(key, out QueryParameter param)) return null;
            
            return param.Single;
        }

        public IEnumerable<KeyValuePair<string, string>> AsPairs()
        {
            foreach (var (key, value) in this)
            {
                foreach (var v in value)
                {
                    yield return new KeyValuePair<string, string>(key, v);
                }
            }
        }

        /// <summary>
        /// Query parameters can be consumed when they are absorbed into another part of the url, such as a
        /// URI Template part. This will have as effect that the parameter with the given name will be returned and
        /// removed from this collection.
        /// </summary>
        /// <returns>string or null if none is found</returns>
        public string Consume(string uriTemplatePart)
        {
            if (!TryGetValue(uriTemplatePart, out var qp)) return null;
            Remove(uriTemplatePart);
            return qp.Single;
        }

        public override string ToString()
        {
            return QueryString.Encode(this);
        }
    }
}