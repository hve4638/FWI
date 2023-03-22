using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    internal class RankDictionary<T, S> : IRankDictionary<T, S>
    {
        readonly Dictionary<T, S> dict;

        public RankDictionary()
        {
            dict = new Dictionary<T, S>();
        }

        public bool Has(T item) => dict.ContainsKey(item);
        public bool Remove(T item) => dict.Remove(item);
        public S Get(T item) => dict[item];
        public void Set(T key, S value) => dict.Add(key, value);
        public void Clear() => dict.Clear();

        public IEnumerator<(T, S)> GetEnumerator()
        {
            foreach (var item in dict)
            {
                yield return (item.Key, item.Value);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
