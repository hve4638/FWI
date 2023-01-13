using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class FWIManager
    {
        const string FILENAME_RANK = "fwi.{0}.rank";
        const string FILENAME_ALIAS = "fwi.{0}.alias";
        const string FILENAME_IGNORE = "fwi.{0}.ignore";
        readonly AliasMap aliasMap;
        readonly IgnoreMap ignoreMap;
        readonly Logger logger;
        readonly DateRank rank;
        public string Signiture { get; set; }

        public int TraceCount { get; set; }

        public FWIManager(string signiture)
        {
            logger = new SingleLogger();
            rank = new DateRank();
            aliasMap = new AliasMap();
            ignoreMap = new IgnoreMap();

            Signiture = signiture;
            TraceCount = 0;
        }

        public FWIManager AddWI(WindowInfo wi) => AppendWindowInfo(wi);
        public FWIManager AppendWindowInfo(WindowInfo wi)
        {
            if (ignoreMap.Contains(wi)) return this;
            aliasMap.Filter(ref wi);

            logger.AddWI(wi);
            rank.Add(wi);
            return this;
        }

        public void Update()
        {
            rank.AddLast(DateTime.Now);
        }

        public void SetLoggingInterval(int minutes = 0) => logger.SetLoggingInterval(minutes);
        public void SetOnLoggingListener(Action<WindowInfo> onLoggingListener) => logger.SetOnLoggingListener(onLoggingListener);

        public ReadOnlyCollection<WindowInfo> GetTimeline()
        {
            var log = logger.GetLog();
            FilterAlias(log);

            return log;
        }
        public ReadOnlyCollection<WindowInfo> GetTimeline(DateTime from, DateTime to)
        {
            var log = logger.GetLog(from, to);
            FilterAlias(log);

            return log;
        }

        public void FilterAlias(IEnumerable<WindowInfo> enumerable)
        {
            foreach (var item in enumerable)
            {
                var wi = item;
                aliasMap.Filter(ref wi);
            }
        }

        public Dictionary<int, RankResult<WindowInfo>> GetRanks() => GetRanks(1, rank.Count);
        public Dictionary<int, RankResult<WindowInfo>> GetRanks(int beginRank = 1, int endRank = 1)
        {
            var ranks = rank.GetRanks(beginRank, endRank);
            
            return ranks;
        }

        static public FWIManager operator +(FWIManager manager, WindowInfo wi) => manager.AppendWindowInfo(wi);
        public void Import(string name)
        {
            logger.Import(name);
            rank.Import(name);
        }
        public void Export(string path)
        {
            string dir, name;
            (dir, name) = HUtils.SplitPath(path);
            var rankName = FileNameFilter(FILENAME_RANK, name, dir);

            logger.Export(path);
            rank.Export(rankName, name);
        }
        public void LoadFilter(string path)
        {
            string dir, name;
            (dir, name) = HUtils.SplitPath(path);
            var aliasPath = FileNameFilter(FILENAME_ALIAS, name, dir);
            var ignorePath = FileNameFilter(FILENAME_IGNORE, name, dir);
            bool isLoad = true;

            ExecuteFileIOSafe(() => aliasMap.Import(aliasPath), () => {
                Console.WriteLine($"Import Fail : Alias List '{aliasPath}'");
                isLoad = false;
            });
            ExecuteFileIOSafe(() => ignoreMap.Import(ignorePath), () => Console.WriteLine($"Import Fail : Ignore List '{ignorePath}'"));

            if (isLoad) FilterAlias(rank.GetWIs());
        }

        public void SaveFilter(string path)
        {
            string dir, name;
            (dir, name) = HUtils.SplitPath(path);
            var aliasPath = FileNameFilter(FILENAME_ALIAS, name, dir);
            var ignorePath = FileNameFilter(FILENAME_IGNORE, name, dir);

            aliasMap.Export(aliasPath);
            ignoreMap.Export(ignorePath);
        }

        string FileNameFilter(string format, string name, string directory = "")
        {
            var formatted = string.Format(format, name);
            if (directory == "") return formatted;
            else return directory + @"\" + formatted;
        }


        static void ExecuteFileIOSafe(Action execute, Action onCatchException = null)
        {
            try
            {
                execute();
            }
            catch (FileNotFoundException)
            {
                onCatchException?.Invoke();
            }
        }

        public int GetLogHashCode()
        {
            return logger.GetHashCode() ^ rank.GetHashCode();
        }
    }
}
