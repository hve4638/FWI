using FWI.Results;
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
        readonly Dictionary<string, string> pathDict;
        readonly AliasMap aliasMap;
        readonly IgnoreMap ignoreMap;
        readonly Logger logger;

        public string Signiture { get; set; }
        public int TraceCount { get; set; }

        public FWIManager(string signiture)
        {
            logger = new SingleLogger();
            aliasMap = new AliasMap();
            ignoreMap = new IgnoreMap();

            Signiture = signiture;
            TraceCount = 0;

            pathDict = new Dictionary<string, string>
            {
                ["rank"] = "fwi.rank",
                ["timeline"] = "fwi.timeline",
                ["alias"] = "fwi.alias",
                ["ignore"] = "fwi.ignore",
            };
        }

        public void SetPath(Dictionary<string, string> path)
        {
            foreach(var key in path.Keys) pathDict[key] = path[key];
            
            if (logger is SingleLogger)
            {
                var singleLogger = logger as SingleLogger;
                singleLogger.SetPath(pathDict);
            }
        }

        public FWIManager AddWI(WindowInfo wi)
        {
            if (ignoreMap.Contains(wi)) return this;
            aliasMap.Filter(ref wi);

            logger.AddWI(wi);
            return this;
        }
        public FWIManager AddEmpty(DateTime date)
        {
            logger.ClearLast(date);
            return this;
        }

        public void Update() => Update(DateTime.Now);
        public void Update(DateTime date)
        {
            logger.Update(date);
        }

        public void SetLoggingInterval(int minutes = 0) => logger.SetLoggingInterval(minutes);
        public void SetOnLoggingListener(Action<WindowInfo> onLoggingListener) => logger.SetOnLoggingListener(onLoggingListener);

        public ReadOnlyCollection<WindowInfo> GetTimeline()
        {
            var log = logger.GetLog();
            aliasMap.Filter(log);

            return log;
        }
        public ReadOnlyCollection<WindowInfo> GetTimeline(DateTime from, DateTime to)
        {
            var log = logger.GetLog(from, to);
            aliasMap.Filter(log);

            return log;
        }

        public Dictionary<int, RankResult<WindowInfo>> GetRanks()
        {
            var ranks = logger.GetRanks();
            var enumerable = ranks.Values.Select(result => result.Item);
            aliasMap.Filter(enumerable);

            return ranks;
        }
        public Dictionary<int, RankResult<WindowInfo>> GetRanks(int beginRank = 1, int endRank = 1) => logger.GetRanks(beginRank, endRank);

        public void Import(string path)
        {
            logger.Import(path);
        }
        public void Export(string path)
        {
            logger.Export(path);
        }

        public void SaveFilter()
        {
            aliasMap.Export(pathDict["alias"]);
            ignoreMap.Export(pathDict["ignore"]);
        }
        public Results<string> LoadFilter()
        {
            var results = new Results<string>();
            
            try
            {
                aliasMap.Import(pathDict["alias"]);
            }
            catch (FileNotFoundException)
            {
                results.State = ResultState.HasProblem;
                results += $"Import Fail : Alias List '{pathDict["alias"]}'";
            }

            try
            {
                ignoreMap.Import(pathDict["ignore"]);
            }
            catch (FileNotFoundException)
            {
                results.State = ResultState.HasProblem;
                results += $"Import Fail : Ignore List '{pathDict["ignore"]}'";
            }

            return results;
        }

        public ReadOnlyDictionary<string, string> GetAlias() => aliasMap.Items;
        public HashSet<string> GetIgnore() => ignoreMap.Items;

        public int GetLogHashCode()
        {
            return logger.GetHashCode();
        }
    }
}
