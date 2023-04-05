using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI.Loggers;
using HUtility;

namespace FWI
{
    /// <summary>
    /// Timeline 로부터 Rank를 가져올때, 이미 가져온 정보를 특정 시간 단위로 캐싱하는 RankWrapper
    /// </summary>
    internal class TimelineRankCache
    {
        readonly Dictionary<DateTime, Rank<WindowInfo, string, TimeSpan>> cache;
        Timeline target;
        TimeSpan interval;

        public TimeSpan Interval
        {
            get => interval;
            set
            {
                interval = value;
            }
        }
        public TimelineRankCache(Timeline timeline)
        {
            cache = new Dictionary<DateTime, Rank<WindowInfo, string, TimeSpan>>();
            interval = new TimeSpan(1, 0, 0);

            ResetTarget(timeline);
        }

        public Rank<WindowInfo, string, TimeSpan> GetRank(DateRange range)
        {
            var alignBegin = GetNextInterval(range.Begin);
            var alignEnd = GetPreviousInterval(range.End);

            var rank = new Rank<WindowInfo, string, TimeSpan>((wi) => wi.Name);
            var date = alignBegin;
            while(date < alignEnd)
            {
                var subrank = GetRankFromCache(date);
                rank.Merge(subrank);

                date += interval;
            }

            if (alignBegin < range.Begin)
            {
                var subrank = target.ToRank(new DateRange(range.Begin, alignBegin));
                rank.Merge(subrank, (x,y) => x+y);
            }
            if (alignEnd > range.End)
            {
                var subrank = target.ToRank(new DateRange(alignEnd, range.End));
                rank.Merge(subrank);
            }

            return rank;
        }

        DateTime GetNextInterval(DateTime date)
        {
            TimeSpan remainder = Interval - TimeSpan.FromTicks(date.TimeOfDay.Ticks % Interval.Ticks);
            if (remainder == interval) remainder = TimeSpan.Zero;

            return date.Add(remainder);
        }
        DateTime GetPreviousInterval(DateTime date)
        {
            TimeSpan remainder = Interval - TimeSpan.FromTicks(date.TimeOfDay.Ticks % Interval.Ticks);

            return date.Add(-Interval + remainder);
        }

        Rank<WindowInfo, string, TimeSpan> GetRankFromCache(DateTime date)
        {
            if (!cache.ContainsKey(date)) SetNewDateCache(date);

            return cache[date];
        }
        void SetNewDateCache(DateTime dateBegin)
        {
            var rank = target.ToRank(new DateRange(dateBegin, dateBegin + Interval));

            cache[dateBegin] = rank;
        }

        public void ResetTarget(Timeline timeline)
        {
            cache.Clear();
            target = timeline;
        }
        public void ClearCache()
        {
            cache.Clear();
        }
    }
}
