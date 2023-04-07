using HUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWI
{
    public class SingleLogger : ILogger
    {
        // Logger에 저장하는 로그의 범위
        // SingleLogger 단독으로 사용시 전체범위
        readonly DateRange loggingRange;
        readonly Timeline timeline;
        readonly TimelineRankCache rankCache;
        public SingleLogger(int maximumSize = 50, DateTime? date = null)
        {
            loggingRange = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            timeline = new Timeline();
        }

        public void SetLoggingInterval(int minutes = 0) => timeline.SetInterval(minutes);
        public void SetMenualDateTimeChanger(Func<DateTime> dateTimeDelegate) => timeline.SetMenualDateTime(dateTimeDelegate);
        public ILogger AddWI(WindowInfo wi)
        {
            timeline.AddLog(wi);
            return this;
        }
        public ILogger AddDefaultWI(DateTime date)
        {
            return AddAFK(date);
        }
        public ILogger AddAFK(DateTime date)
        {
            var wi = WindowInfo.AFK;
            wi.Date = date;
            timeline.AddLog(wi);
            return this;
        }

        public void SetOnLoggingListener(Action<WindowInfo> onLoggingListener) => timeline.SetOnAddListener(onLoggingListener);

        public int Length => timeline.Count;
        public int Count => timeline.Count;

        public ReadOnlyCollection<WindowInfo> GetTimeline() => timeline.GetAllWIs();
        public ReadOnlyCollection<WindowInfo> GetTimeline(DateRange range) => timeline.GetWIs(range);

        public IRank<WindowInfo, TimeSpan> GetRanks()
        {
            return rankCache.GetRank(timeline.Range);
        }

        public IRank<WindowInfo, TimeSpan> GetRanks(int beginRank, int endRank)
        {
            throw new NotImplementedException();
        }

        public void Update(DateTime time)
        {
            throw new NotImplementedException();
        }

        public void Import(string filename)
        {
            throw new NotImplementedException();
        }

        public void Export(string filename)
        {
            throw new NotImplementedException();
        }


        [Obsolete]
        public ReadOnlyCollection<WindowInfo> GetLog() => GetTimeline();
        [Obsolete]
        public ReadOnlyCollection<WindowInfo> GetLog(DateRange range) => GetTimeline(range);
    }
}
