using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    internal class RankKeyFilterDictionary<T, S> : IRankDictionary<T>
    {
        readonly IRankDictionary<S> rank;
        readonly Dictionary<S, T> reverseDict;
        readonly Func<T, S> onFilter;

        public RankKeyFilterDictionary(Func<T, S> keyFilter)
        {
            rank = new RankDictionary<S>();
            reverseDict = new Dictionary<S, T>();
            onFilter = keyFilter;
        }

        S Filter(T key)
        {
            var result = onFilter(key);
            if (!reverseDict.ContainsKey(result)) reverseDict.Add(result, key);
            
            return result;
        }
        T Unfilter(S key) => reverseDict[key];

        public TimeSpan Get(T item) => rank.Get(Filter(item));
        public bool Has(T item) => rank.Has(Filter(item));
        public bool Remove(T item) => rank.Remove(Filter(item));
        public void Set(T key, TimeSpan value) => rank.Set(Filter(key), value);
        public void Clear()
        {
            reverseDict.Clear();
            rank.Clear();
        }

        public IEnumerator<(T, TimeSpan)> GetEnumerator()
        {
            foreach((S key, TimeSpan value) in rank)
            {
                yield return (Unfilter(key), value);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
