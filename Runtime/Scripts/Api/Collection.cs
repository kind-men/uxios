using System.Collections;
using System.Collections.Generic;

namespace KindMen.Uxios.Api
{
    public class Collection<TData> : ICollection<TData>
    {
        public IEnumerator<TData> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TData item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(TData item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(TData[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(TData item)
        {
            throw new System.NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
    }
}