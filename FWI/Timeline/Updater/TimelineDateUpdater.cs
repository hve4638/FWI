using FWI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HUtility;

namespace FWI
{
    public class TimelineDateUpdater : ITimelineUpdater
    {
        static readonly Func<DateTime> getDateTimeDef = () => DateTime.Now;
        Func<DateTime> getDateTime;
        readonly Rank<WindowInfo, string, TimeSpan> rank;
        DateTime begin, end;
        DateTime last;
        WindowInfo lastWI;
        Action<WindowInfo> onEnd;

        public TimelineDateUpdater() : this(begin: DateTime.MinValue, end: DateTime.MaxValue) 
        {

        }

        public TimelineDateUpdater(DateTime begin, DateTime end, WindowInfo? initWI = null)
        {
            rank = new Rank<WindowInfo, string, TimeSpan>((wi) => wi.Name);
            onEnd = (WindowInfo wi) => { };
            getDateTime = getDateTimeDef;

            Reset(begin:begin, end:end, initWI: initWI ?? WindowInfo.NoWindow);
        }

        public void Reset(DateTime begin, DateTime end) => Reset(begin, end, WindowInfo.NoWindow);
        public void Reset(DateTime begin, DateTime end, WindowInfo initWI)
        {
            this.begin = begin;
            this.end = end;
            last = DateTime.MinValue;
            lastWI = WindowInfo.NoWindow;
            rank.Clear();

            if (!initWI.IsNoWindow())
            {
                if (end > initWI.Date) Add(initWI);
                else throw new TimeSequenceException()
                {
                    Last = end,
                    Input = initWI.Date,
                };
            }
        }

        public void SetOnEnd(Action<WindowInfo> onEnd)
        {
            this.onEnd = onEnd;
        }

        public TimeSpan Add(WindowInfo wi)
        {
            var date = wi.Date;
            var time = TimeSpan.Zero;
            if (IsEnd() || date < last) return TimeSpan.Zero;
            
            if (date <= begin) time = UpdateBeginWI(wi);
            else if (date >= end) time = UpdateLastWI();
            else if (last <= date) time = UpdateWI(wi);

            if (IsEnd()) CallOnEnd();

            return time;
        }

        TimeSpan UpdateBeginWI(WindowInfo wi)
        {
            if (lastWI.IsNoWindow() || lastWI.Date <= wi.Date) lastWI = wi;

            return TimeSpan.Zero;
        }

        TimeSpan UpdateLastWI()
        {
            var time = AddRankLast(end);
            last = end;

            return time;
        }

        TimeSpan UpdateWI(WindowInfo wi)
        {
            TimeSpan time = TimeSpan.Zero;
            if (!lastWI.IsNoWindow()) time = AddRankLast(wi.Date);
            lastWI = wi;
            last = wi.Date;

            return time;
        }

        TimeSpan AddRankLast(DateTime toDate)
        {
            TimeSpan time;
            if (last >= begin) time = toDate - last; 
            else time = toDate - begin;

            rank[lastWI] += time;

            return time;
        }

        void CallOnEnd()
        {
            if (rank.HasOne()) onEnd(One());
        }

        public bool IsEnd() => (last >= end);
        public bool IsEnd(DateTime date) => (date >= end || IsEnd());

        public TimeSpan FillLast()
        {
            if (HasNoLastWI() || IsEnd()) return TimeSpan.Zero;
            else
            {
                var wi = lastWI.Copy();
                wi.Date = EndDate;
                return Add(wi);
            }
        }

        bool HasNoLastWI() => lastWI.IsNoWindow();

        public bool Empty => (rank.Count == 0);
        public bool HasOne() => (rank.Count > 0);
        public WindowInfo One() => rank.One();

        public WindowInfo Last => lastWI;
        public DateTime BeginDate => begin;
        public DateTime EndDate => end;
    }
}
