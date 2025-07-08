using System;
using System.Collections;
using System.Collections.Generic;

namespace KindMen.Uxios.Http
{
    [System.Serializable]
    public class QueryParameter : IEnumerable<string>
    {
        public string Key { get; }

        public int Count =>
            values is { Count: > 0 }
                ? values.Count
                : firstValue != null ? 1 : 0;

        public IEnumerable<string> Values
        {
            get {
                if (values is { Count: > 0 })
                {
                    foreach (var v in values) yield return v;
                }
                else if (firstValue != null)
                {
                    yield return firstValue;
                }
            }
        }

        private string firstValue = null;
        private List<string> values = null;

        public string Single => Count switch
        {
            0 => null,
            1 => firstValue,
            _ => string.Join(",", this)
        };

        public string this[int i]
        {
            get
            {
                if (i < 0 || i >= Count) throw new IndexOutOfRangeException();
                
                return (values?.Count ?? 0) > 0 ? values[i] : firstValue; 
            }
        }

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
            if (firstValue == null)
            {
                firstValue = value;
                return;
            }

            // Will not create an allocation because it will keep the reference and not make a copy 
            values ??= new List<string>(2) { firstValue };
            values.Add(value);
        }

        public void Set(string value)
        {
            firstValue = value;
            values?.Clear();
        }

        public void Set(List<string> value)
        {
            if (value == null || value.Count == 0)
            {
                firstValue = null;
                values?.Clear();
                return;
            }

            firstValue = value[0];
            if (value.Count == 1)
            {
                values?.Clear();
                return;
            }

            values ??= new List<string>(value.Count);
            values.Clear();
            values.AddRange(value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<string> GetEnumerator() => Values.GetEnumerator();

        public override string ToString() => $"{Key}={Single}";
    }
}