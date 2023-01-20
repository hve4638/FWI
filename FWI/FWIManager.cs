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
        readonly History<WindowInfo> history;
        WindowInfo lastWI;
        bool isAFK;

        public WindowInfo LastWI => (history.Count == 0 ?  new NoWindowInfo() : history.GetLast());
        public string Signiture { get; set; }
        public int TraceCount { get; set; }

        public FWIManager(string signiture)
        {
            logger = new SingleLogger();
            aliasMap = new AliasMap();
            ignoreMap = new IgnoreMap();
            history = new History<WindowInfo>();

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

        public FWIManager AddWI(WindowInfo wi) => AppendWindowInfo(wi);
        public FWIManager AppendWindowInfo(WindowInfo wi)
        {
            if (ignoreMap.Contains(wi)) return this;
            aliasMap.Filter(ref wi);
            isAFK = false;
            lastWI = wi;

            logger.AddWI(wi);
            history.Add(wi);
            return this;
        }
        public FWIManager SetAFK(DateTime date)
        {
            var afkWI = new AFKWindowInfo(date);
            isAFK = true;

            logger.AddWI(afkWI);
            history.Add(afkWI);
            return this;
        }
        public FWIManager SetNoAFK(DateTime date)
        {
            isAFK = false;
            if (lastWI != null)
            {
                var wi = lastWI.Copy();
                wi.Date = date;

                AddWI(wi);
            }

            return this;
        }

        public void Update()
        {
            logger.Update(DateTime.Now);
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

        static public FWIManager operator +(FWIManager manager, WindowInfo wi) => manager.AppendWindowInfo(wi);
        public void Import(string path)
        {
            logger.Import(path);
        }
        public void Export(string path)
        {
            logger.Export(path);
        }
        public Results<string> LoadFilter()
        {
            var results = new Results<string>();
            
            ExecuteFileIOSafe(() => aliasMap.Import(pathDict["alias"]), () => {
                results += $"Import Fail : Alias List '{pathDict["alias"]}'";
                results.State = ResultState.HasProblem;
            });
            ExecuteFileIOSafe(() => ignoreMap.Import(pathDict["ignore"]), () => {
                results += $"Import Fail : Ignore List '{pathDict["ignore"]}'";
                results.State = ResultState.HasProblem;
            });

            return results;
        }

        public void SaveFilter()
        {
            aliasMap.Export(pathDict["alias"]);
            ignoreMap.Export(pathDict["ignore"]);
        }

        public List<WindowInfo> History => history.GetAll();

        public ReadOnlyDictionary<string, string> GetAlias() => aliasMap.Items;
        public HashSet<string> GetIgnore() => ignoreMap.Items;


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
            return logger.GetHashCode();
        }
    }
}
