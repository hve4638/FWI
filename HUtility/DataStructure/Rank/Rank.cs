using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HUtility
{
    public class Rank<T>
    {
        int currentHash;
        readonly Dictionary<T, TimeSpan> rankDict;

        public Rank()
        {
            rankDict = new Dictionary<T, TimeSpan>();
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
        public bool TryGetRank(int value, out T output, out TimeSpan time)
        {
            int rank;
            int lastRank = 0;
            TimeSpan duration;

            output = default;

            if (value <= rankDict.Count)
            {
                foreach (var name in rankDict.Keys)
                {
                    rank = 1;
                    duration = rankDict[name];
                    foreach (var n in rankDict.Values)
                    {
                        if (n > duration) rank++;
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
                time = TimeSpan.Zero;
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

        public void Add(Rank<T> other)
        {
            foreach (var item in other.rankDict) Add(item.Key, item.Value);
        }
        public void Add(T key, TimeSpan duration)
        {
            if (rankDict.ContainsKey(key)) UpdateKeyHash(key);
            else rankDict.Add(key, TimeSpan.Zero);

            rankDict[key] += duration;
            UpdateKeyHash(key);
        }
        void UpdateKeyHash(T key)
        {
            var hash = key.GetHashCode() * 7 + rankDict[key].GetHashCode();

            currentHash ^= hash;
        }

        public int Count => rankDict.Count;

        public override bool Equals(object obj)
        {
            if (obj is Rank<T>)
            {
                Rank<T> other = obj as Rank<T>;
                return rankDict.SequenceEqual(other.rankDict);
            }
            else return false;
        }
        public int GetContentsHash() => currentHash;
        public override int GetHashCode() => base.GetHashCode();
    }

    public class RankNotFoundException : Exception
    {
        public RankNotFoundException() : base()
        {

        }

        public RankNotFoundException(string message) : base(message)
        {

        }
    }
}
