using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HUtility
{
    public class Rank<T, S> where S : IComparable<S>
    {
        int currentHash;
        readonly Dictionary<T, S> rankDict;

        public Rank()
        {
            rankDict = new Dictionary<T, S>();
            Clear();
        }
        
        public void Clear()
        {
            currentHash = 0;
            rankDict.Clear();
        }

        public T One() => GetRank(1);
        public bool HasOne() => rankDict.Count > 0;

        public bool TryGetRank(int value, out T output) => TryGetRank(value, out output, out _);
        public bool TryGetRank(int value, out T output, out S time)
        {
            int rank;
            int lastRank = 0;
            S duration;

            output = default;

            if (value <= rankDict.Count)
            {
                foreach (var name in rankDict.Keys)
                {
                    rank = 1;
                    duration = rankDict[name];
                    foreach (var n in rankDict.Values)
                    {
                        if (n.CompareTo(duration) > 0) rank++;
                    }

                    if (rank == value)
                    {
                        output = name;
                        time = rankDict[output];
                        return true;
                    }
                    else if (rank < value && rank > lastRank)
                    {
                        lastRank = rank;
                        output = name;
                    }
                }
            }

            if (lastRank == 0)
            {
                time = default;
                return false;
            }
            else
            {
                time = rankDict[output];
                return true;
            }
        }

        public T GetRank(int value)
        {
            if (TryGetRank(value, out T result)) return result;
            else throw new RankNotFoundException();
        }
        /*
        public void Add(Rank<T, S> other)
        {
            foreach (var item in other.rankDict) Add(item.Key, item.Value);
        }
        public void Add(T key, S duration)
        {
            if (rankDict.ContainsKey(key)) UpdateKeyHash(key);
            else rankDict.Add(key, default);

            var p = rankDict[key];
            var a = p + duration;
            UpdateKeyHash(key);
        }*/
        void UpdateKeyHash(T key)
        {
            var hash = key.GetHashCode() * 7 + rankDict[key].GetHashCode();

            currentHash ^= hash;
        }

        public int Count => rankDict.Count;

        public override bool Equals(object obj)
        {
            if (obj is Rank<T, S>)
            {
                Rank<T, S> other = obj as Rank<T, S>;
                return rankDict.SequenceEqual(other.rankDict);
            }
            else return false;
        }
        public int GetContentsHash() => currentHash;
        public override int GetHashCode() => base.GetHashCode();
    }


}
