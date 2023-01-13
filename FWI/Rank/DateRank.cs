using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using FWI.Exceptions;

namespace FWI
{
    public class DateRank
    {
        const string FILENAME_RANK = "fwi.rank.{0}.ranking";
        const string FILENAME_WI = "fwi.rank.{0}.wi";
        readonly Dictionary<string, WindowInfo> WIs;
        readonly Rank rank;
        WindowInfo lastWI;
        DateTime lastTime;

        public DateRank()
        {
            rank = new Rank();
            WIs = new Dictionary<string, WindowInfo>();
            Reset();
        }

        public void Reset()
        {
            WIs.Clear();
            rank.Clear();
            lastWI = null;
            lastTime = DateTime.MinValue;
        }

        public void Add(WindowInfo wi)
        {
            AddLast(wi.Date);
            AddToWIs(wi);

            lastWI = wi;
        }

        void AddToWIs(WindowInfo wi)
        {
            if (!WIs.ContainsKey(wi.Name)) WIs.Add(wi.Name, wi);
        }

        public void AddLast(DateTime date)
        {
            if (lastWI == null) { }
            else if (date >= lastTime) rank.Add(lastWI, date - lastTime);
            else throw new TimeSeqeunceException();

            lastTime = date;
        }

        public IEnumerable<WindowInfo> GetWIs()
        {
            return WIs.Values;
        }

        public Dictionary<int, RankResult<WindowInfo>> GetRanks(int beginRank=1, int endRank=1)
        {
            var rankResults = new Dictionary<int, RankResult<WindowInfo>>();

            for (int i = beginRank; i <= endRank; i++)
            {
                if (rank.TryGetRank(i, out string name, out TimeSpan time))
                {
                    var rankResult = new RankResult<WindowInfo>(item: WIs[name], ranking: i, duration: time);
                    rankResults.Add(i, rankResult);
                }
            }

            return rankResults;
        }

        public int GetContentsHash()
        {
            var hash = rank.GetContentsHash();
            int hashWI = 0;
            foreach(var wi in WIs.Values)
                hashWI ^= wi.GetContentsHashCode();

            return hash ^ hashWI;
        }

        public void Import(string filename)
        {   
            var reader = new StreamReader(filename);
            Import(reader, filename);
            reader.Close();
        }

        public void Import(StreamReader reader, string path)
        {
            string dir;
            (dir, _) = HUtils.SplitPath(path);

            var config = ImportMeta(reader);
            
            var rankFilename = HUtils.MergePath(dir, config.RankPath);
            var wiFilename = HUtils.MergePath(dir, config.WIPath);

            ImportContents(rankFilename, wiFilename);
        }
        Config ImportMeta(StreamReader reader)
        {
            string contents = reader.ReadToEnd();
            var deserialized = JsonSerializer.Deserialize<Config>(contents);

            return deserialized;
        }

        public void ImportContents(string rankFilename, string wiFilename)
        {
            var rankReader = new StreamReader(rankFilename);
            var wiReader = new StreamReader(wiFilename);
            ImportContents(rankReader, wiReader);
            rankReader.Close();
            wiReader.Close();
        }
        public void ImportContents(StreamReader rankReader, StreamReader wiReader)
        {
            rank.Import(rankReader);

            while (!wiReader.EndOfStream)
            {
                var line = wiReader.ReadLine();
                var item = WindowInfo.Decode(line);
                AddToWIs(item);
            }
        }


        public void Export(string name, string signiture = null)
        {
            string path;
            (path, _) = HUtils.SplitPath(name);
            var writer = new StreamWriter(name);
            Export(writer, path, signiture);
            writer.Close();
        }
        public void Export(StreamWriter writer, string path, string signiture = null)
        {
            string name, dir;
            (dir, name) = HUtils.SplitPath(path);
            signiture = signiture ?? name;

            var rankFilename = FileNameFilter(FILENAME_RANK, signiture);
            var wiFilename = FileNameFilter(FILENAME_WI, signiture);

            ExportMeta(writer, rankFilename, wiFilename);
            ExportContents(MergeFullPath(dir, rankFilename), MergeFullPath(dir, wiFilename));
        }
        public void ExportMeta(StreamWriter writer, string rankFilename, string wiFilename)
        {
            var config = new Config
            {
                RankPath = rankFilename,
                WIPath = wiFilename,
            };
            var serialized = JsonSerializer.Serialize(config);

            writer.WriteLine(serialized);
        }
        public void ExportContents(string rankFilename, string wiFilename)
        {
            var rankWriter = new StreamWriter(rankFilename);
            var wiWriter = new StreamWriter(wiFilename);
            ExportContents(rankWriter, wiWriter);
            rankWriter.Close();
            wiWriter.Close();
        }
        public void ExportContents(StreamWriter rankWriter, StreamWriter wiWriter)
        {
            rank.Export(rankWriter);

            foreach (var wi in WIs.Values)
            {
                var encoded = wi.Encoding();
                wiWriter.WriteLine(encoded);
            }
        }
        static string FileNameFilter(string format, string name, string directory = "")
        {
            var formatted = string.Format(format, name);
            if (directory == "") return formatted;
            else return directory + @"\" + formatted;
        }
        static string MergeFullPath(string dir, string name)
        {
            if (dir == "") return name;
            else return dir + @"\" + name;
        }


        public int Count => rank.Count;

        class Config
        {
            public string RankPath { get; set; }
            public string WIPath { get; set; }
        }
    }

    public class RankResult<T>
    {
        readonly T item;
        readonly TimeSpan duration;
        readonly int ranking;
        public RankResult(T item, TimeSpan duration, int ranking)
        {
            this.item = item;
            this.duration = duration;
            this.ranking = ranking;
        }

        public T Item => item;
        public TimeSpan Duration => duration;
        public int Ranking => ranking;
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }
        public override int GetHashCode()
        {
            int hashCode = item.GetHashCode();
            hashCode ^= duration.GetHashCode();
            hashCode^= ranking.GetHashCode();
            return hashCode;
        }
    }
}
