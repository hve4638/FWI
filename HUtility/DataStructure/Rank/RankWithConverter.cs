using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HUtility
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K">랭크에 저장하고 비교할 대상</typeparam>
    /// <typeparam name="T">K를 내부적으로 변환해 저장되는 값</typeparam>
    /// <typeparam name="S">순위를 비교하기 위한 값</typeparam>
    public class Rank<K, T, S> : IRank<K, S> where S : IComparable<S>
    {
        readonly Rank<T, S> rank;
        readonly Converter<K, T> converter;
        readonly Dictionary<T, K> reversed;

        public Rank(Converter<K, T> converter)
        {
            reversed = new Dictionary<T, K>();
            rank = new Rank<T, S>();

            this.converter = converter;
        }

        T Convert(K input)
        {
            var output = converter(input);
            if (!reversed.ContainsKey(output)) reversed[output] = input;

            return output;
        }

        public S this[K index] {
            get
            {
                var key = Convert(index);

                return rank[key];
            }
            set
            {
                var key = Convert(index);

                rank[key] = value;
            }
        }

        public bool HasOne() => rank.HasOne();

        public K One()
        {
            var result = rank.One();

            return reversed[result];
        }

        public bool TryGetRank(int value, out K koutput)
        {
            var result = rank.TryGetRank(value, out T toutput);
            if (result)
            {
                koutput = reversed[toutput];
                return true;
            }
            else
            {
                koutput = default;
                return false;
            }
        }

        public K GetRank(int value)
        {
            var result = rank.GetRank(value);

            return reversed[result];
        }

        public int Count => rank.Count;
        public void Clear() => rank.Clear();

        public IEnumerator<(K, S)> GetEnumerator()
        {
            foreach (var (tKey, value) in rank)
            {
                var kKey = reversed[tKey];

                yield return (kKey, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Merge(IRank<K, S> other, Func<S, S, S> onMerge)
        {
            foreach (var (key, value) in other) this[key] = onMerge(this[key], value);
        }

        public void Merge(Rank<K, T, S> other, Func<S, S, S> onMerge)
        {
            foreach (var (key, value) in other) this[key] = onMerge(this[key], value);
        }
    }
}
