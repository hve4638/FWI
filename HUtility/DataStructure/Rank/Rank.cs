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
    /// <typeparam name="T">랭크에 저장하고 비교할 대상</typeparam>
    /// <typeparam name="S">순위를 비교하기 위한 값</typeparam>
    public class Rank<T, S> : IRank<T, S> where S : IComparable<S>
    {
        readonly RankDictionary<T, S> rankDict;
        public Rank()
        {
            rankDict = new RankDictionary<T, S>();
        }
        public int Count => rankDict.Count;

        public S this[T index]
        {
            get {
                if (rankDict.Has(index)) return rankDict.Get(index);
                else return default;
            }
            set {
                if (rankDict.Has(index)) rankDict.Remove(index);

                rankDict.Set(index, value);
            }
        }

        public T One()
        {
            if (!HasOne()) throw new ArgumentException("첫번째 순위 값이 없습니다");

            bool begin = true;
            T one = default;
            S oneTime = default;
            foreach (var (t, s) in rankDict)
            {
                if (begin)
                {
                    one = t;
                    oneTime = s;
                    begin = false;
                }
                else if (oneTime.CompareTo(s) <= 0)
                {
                    one = t;
                    oneTime = s;
                }
            }
            return one;
        }
        public bool HasOne() => rankDict.Count >= 1;

        public bool TryGetRank(int value, out T output)
        {
            var sorted = rankDict.Ordered();
            var index = 1;
            foreach (var item in sorted)
            {
                if (index == value)
                {
                    output = item.Key;
                    return true;
                }
                index++;
            }
            output = default;
            return false;
        }

        public T GetRank(int value)
        {
            if (TryGetRank(value, out T result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"{value}번째 순위 값이 없습니다");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Rank<T, S>)
            {
                Rank<T, S> other = obj as Rank<T, S>;
                return rankDict.SequenceEqual(other.rankDict);
            }
            else return false;
        }

        public void Clear()
        {
            rankDict.Clear();
        }

        public override int GetHashCode() => base.GetHashCode();

        public IEnumerator<(T, S)> GetEnumerator()
        {
            foreach (var pair in rankDict)
            {
                yield return (pair.Item1, pair.Item2);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Merge(IRank<T, S> other, Func<S, S, S> onMerge)
        {
            foreach (var (key, value) in other)
            {
                this[key] = onMerge(this[key], value);
            }
        }
    }
}
