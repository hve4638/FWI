using HUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWI {
    public class SingleLogger : Logger
    {
        int maximumSize;
        readonly Timeline timeline;
        readonly DateRank rank;
        readonly Dictionary<string, string> pathDict;
        public SingleLogger(int maximumSize = 50, DateTime? date = null)
        {
            timeline = new Timeline();
            rank = new DateRank();
            this.maximumSize = maximumSize;
            pathDict = new Dictionary<string, string>
            {
                ["rank"] = "fwi.logger.rank",
                ["timeline"] = "fwi.logger.timeline",
            };
        }

        public void SetPath(Dictionary<string, string> path)
        {
            foreach (var key in path.Keys) pathDict[key] = path[key];
        }

        public void SetLoggingInterval(int minutes=0) => timeline.SetInterval(minutes);
        public void SetDateTimeDelegate(Func<DateTime> dateTimeDelegate) => timeline.SetMenualDateTime(dateTimeDelegate);
        public Logger AppendWindowInfo(WindowInfo wi) => AddWI(wi);
        public Logger AddWI(WindowInfo wi)
        {
            rank.Add(wi);
            timeline.AddLog(wi);
            return this;
        }
        public Logger ClearLast(DateTime date)
        {
            rank.ClearLast(date);
            timeline.AddLog(new AFKWindowInfo(date));
            return this;
        }
        public Logger AddAFK(DateTime date)
        {
            rank.ClearLast(date);
            timeline.AddLog(new AFKWindowInfo(date));
            return this;
        }

        public void SetOnLoggingListener(Action<WindowInfo> onLoggingListener) => timeline.SetOnAddListener(onLoggingListener);

        public static SingleLogger operator +(SingleLogger logger, WindowInfo wi)
        {
            logger.AddWI(wi);
            return logger;
        }

        public bool IsFull => (timeline.Count >= maximumSize);
        public int Length => timeline.Count;
        public int Count => timeline.Count;

        public ReadOnlyCollection<WindowInfo> GetLog() => timeline.GetTimeline();
        public ReadOnlyCollection<WindowInfo> GetLog(DateTime from, DateTime to) => GetLog(new DateRange(from, to));
        public ReadOnlyCollection<WindowInfo> GetLog(DateRange range) => timeline.GetTimeline(range);

        public Dictionary<int, RankResult<WindowInfo>> GetRanks() => GetRanks(1, rank.Count);
        public Dictionary<int, RankResult<WindowInfo>> GetRanks(int beginRank = 1, int endRank = 1)
        {
            var ranks = rank.GetRanks(beginRank, endRank);

            return ranks;
        }

        public void Update(DateTime lastTime)
        {
            rank.AddLast(lastTime);
        }

        public void Import(string path)
        {
            timeline.Import(pathDict["timeline"]);
            rank.Import(pathDict["rank"]);
        }

        public void Export(string path)
        {
            timeline.Export(pathDict["timeline"]);
            rank.Export(pathDict["rank"]);
        }
        public int GetContentsHashCode()
        {
            return rank.GetContentsHash() ^ timeline.GetContentsHashCode();
        }
    }
}
