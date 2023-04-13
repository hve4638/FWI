using FWI.Results;
using HUtility;
using Microsoft.SqlServer.Server;
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
        readonly WindowInfoAliasFilter aliasFilter;
        readonly WindowInfoIgnoreFilter ignoreFilter;
        readonly ILogger logger;

        public string Signiture { get; set; }
        public int TraceCount { get; set; }

        public FWIManager(string signiture)
        {
            logger = new SingleLogger();
            aliasFilter = new WindowInfoAliasFilter();
            ignoreFilter = new WindowInfoIgnoreFilter();

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
                //singleLogger.SetPath(pathDict);
            }
        }

        /// <exception cref="TimeSequenceException"></exception>
        public Result<ResultState, string> AddWI(WindowInfo wi)
        {
            var results = new Result<ResultState, string>(ResultState.Normal);
            
            ignoreFilter.Filter(ref wi);
            if (wi.IsNoWindow())
            {
                results += "ignoreMap 필터링됨";
            }
            else
            {
                aliasFilter.Filter(ref wi);
                logger.AddWI(wi);
            }

            return results;
        }

        /// <exception cref="TimeSequenceException"></exception>
        public FWIManager AddEmpty(DateTime date)
        {
            logger.AddDefaultWI(date);
            return this;
        }

        public void Update() => Update(DateTime.Now);
        public void Update(DateTime date)
        {
            logger.Update(date);
        }

        public void SetLoggingInterval(int minutes = 0) => logger.SetLoggingInterval(minutes);
        public void SetOnLoggingListener(Action<WindowInfo> onLoggingListener)
            => logger.SetOnLoggingListener(onLoggingListener);

        public ReadOnlyCollection<WindowInfo> GetTimeline()
        {
            var log = logger.GetLog();
            aliasFilter.Filter(log);
            return log;
        }
        public ReadOnlyCollection<WindowInfo> GetTimeline(DateTime from, DateTime to)
        {
            var log = logger.GetLog(new DateRange(from, to));
            aliasFilter.Filter(log);
            return log;
        }

        public IRank<WindowInfo, TimeSpan> GetRanks()
        {
            var ranks = logger.GetRanks();

            //var enumerable = ranks.Values.Select(result => result.Item);
            //aliasFilter.Filter(enumerable);

            return ranks;
        }
        public IRank<WindowInfo, TimeSpan> GetRanks(int beginRank = 1, int endRank = 1)
            => logger.GetRanks(beginRank, endRank);

        public void Import(string path) => logger.Import(path);
        public void Export(string path) => logger.Export(path);

        public void SaveFilter()
        {
            aliasFilter.Export(pathDict["alias"]);
            ignoreFilter.Export(pathDict["ignore"]);
        }
        public Result<ResultState, string> LoadFilter()
        {
            var results = new Result<ResultState, string>(ResultState.Normal);

            try
            {
                aliasFilter.Import(pathDict["alias"]);
            }
            catch (FileNotFoundException)
            {
                results.State |= ResultState.HasProblem;
                results += $"Import Fail : Alias List '{pathDict["alias"]}'";
            }

            try
            {
                ignoreFilter.Import(pathDict["ignore"]);
            }
            catch (FileNotFoundException)
            {
                results.State |= ResultState.HasProblem;
                results += $"Import Fail : Ignore List '{pathDict["ignore"]}'";
            }

            return results;
        }

        public ReadOnlyDictionary<string, string> GetAlias() => aliasFilter.Items;
        public HashSet<string> GetIgnores() => ignoreFilter.Items;

        public int GetLogHashCode()
        {
            return logger.GetHashCode();
        }
    }
}
