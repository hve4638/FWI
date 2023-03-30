using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    internal class RankKeyFilterDictionary<T, K, S>
    {
        readonly IRankDictionary<K, S> rank;
        readonly Dictionary<K, T> reverseDict;
        readonly Func<T, K> onFilter;

        public RankKeyFilterDictionary(Func<T, K> keyFilter)
        {
            rank = new RankDictionary<K, S>();
            reverseDict = new Dictionary<K, T>();
            onFilter = keyFilter;
        }

        K Filter(T key)
        {
            var result = onFilter(key);
            if (!reverseDict.ContainsKey(result)) reverseDict.Add(result, key);
            
            return result;
        }
        T Unfilter(K key) => reverseDict[key];

        public S Get(T item) => rank.Get(Filter(item));
        public bool Has(T item) => rank.Has(Filter(item));
        public bool Remove(T item) => rank.Remove(Filter(item));
        public void Set(T key, S value) => rank.Set(Filter(key), value);
        public void Clear()
        {
            reverseDict.Clear();
            rank.Clear();
        }

        public IEnumerator<(T, S)> GetEnumerator()
        {
            foreach((K key, S value) in rank)
            {
                yield return (Unfilter(key), value);
            }
        }
    }
}
