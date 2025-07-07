using System.Collections;
using System.Collections.Generic;

namespace KindMen.Uxios.Http
{
    [System.Serializable]
    public class QueryParameter : IEnumerable<string>
    {
        public string Key { get; }
        public List<string> Values { get; } = new(1);
        public string Single =>
            Values.Count switch
            {
                0 => null,
                1 => Values[0],
                _ => string.Join(',', Values)
            };

        public string this[int i] => Values[i];

        public QueryParameter(string key)
        {
            Key = key;
        }

        public QueryParameter(string key, List<string> values)
        {
            Key = key;
            Set(values);
        }

        public QueryParameter(string key, string value)
        {
            Key = key;
            Add(value);
        }

        public void Add(string value)
        {
            Values.Add(value);
        }

        public void Set(List<string> value)
        {
            Values.Clear();
            Values.AddRange(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public override string ToString()
        {
            return $"{Key}={string.Join(",", Values)}";
        }
    }
}