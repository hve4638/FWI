using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    internal class RankDictionary<T> : IRankDictionary<T>
    {
        readonly Dictionary<T, TimeSpan> dict;

        public RankDictionary()
        {
            dict = new Dictionary<T, TimeSpan>();
        }

        public bool Has(T item) => dict.ContainsKey(item);
        public bool Remove(T item) => dict.Remove(item);
        public TimeSpan Get(T item) => dict[item];
        public void Set(T key, TimeSpan value) => dict.Add(key, value);
        public void Clear() => dict.Clear();

        public IEnumerator<(T, TimeSpan)> GetEnumerator()
        {
            foreach (var item in dict)
            {
                yield return (item.Key, item.Value);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
