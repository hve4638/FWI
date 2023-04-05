using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWI
{
    public class Rank
    {
        int currentHashCode;
        readonly Dictionary<string, TimeSpan> rankDict;

        public Rank()
        {
            rankDict = new Dictionary<string, TimeSpan>();
            Clear();
        }

        public void Clear()
        {
            currentHashCode = 0;
            rankDict.Clear();
        }

        /// <summary>
        /// Rank 1을 가져옵니다.
        /// </summary>
        public string One() => GetRank(1);
        public bool HasOne() => rankDict.Count > 0;

        /// <summary>
        /// value 등수의 값을 가져옵니다
        /// </summary>
        public bool TryGetRank(int value, out string output) => TryGetRank(value, out output, out _);
        /// <summary>
        /// value 등수의 값을 가져옵니다
        /// </summary>
        public bool TryGetRank(int value, out string output, out TimeSpan time)
        {
            int rank;
            int lastRank = 0;
            TimeSpan duration;

            output = "";

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

        public string GetRank(int value)
        {
            if (TryGetRank(value, out string name)) return name;
            else throw new RankNotFoundException();
        }

        public void Add(Rank other)
        {
            foreach(var item in other.rankDict)
            {
                Add(item.Key, item.Value);
            }
        }
        public void Add(WindowInfoLegacy wi, TimeSpan duration) => Add(wi.Name, duration);
        public void Add(string name, TimeSpan duration)
        {
            if (rankDict.ContainsKey(name)) currentHashCode ^= GetItemHash(name, rankDict[name]);
            else rankDict.Add(name, TimeSpan.Zero);

            rankDict[name] += duration;
            currentHashCode ^= GetItemHash(name, rankDict[name]);
        }

        int GetItemHash(string name, TimeSpan time) => $"{name}{time}".GetHashCode();

        public int Count => rankDict.Count;

        public void Import(string filename)
        {
            using var reader = new StreamReader(filename);
            Import(reader);
        }
        public void Import(StreamReader reader)
        {
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                DecodeRank(line);
            }
        }
        void DecodeRank(string line)
        {
            string name, timef;
            var splited = line.Split('|');
            try
            {
                name = splited[0];
                timef = splited[1];
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }

            if (TimeSpan.TryParse(timef, out TimeSpan ts))
            {
                var decoded = HUtils.DecodeVertical(name);

                Add(decoded, ts);
            }
        }

        public void Export(string filename)
        {
            using var writer = new StreamWriter(filename);
            Export(writer);
        }

        public void Export(StreamWriter writer)
        {
            foreach(var name in rankDict.Keys)
            {
                var encoded = EncodeRank(name);
                writer.WriteLine(encoded);
            }
        }
        string EncodeRank(string name)
        {
            var encodedName = HUtils.EncodeVertical(name);
            var timeStr = rankDict[name].ToString();

            return $"{encodedName}|{timeStr}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Rank)
            {
                Rank other = obj as Rank;
                return rankDict.SequenceEqual(other.rankDict);
            }
            else return false;
        }
        public int GetContentsHash() => currentHashCode;
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
