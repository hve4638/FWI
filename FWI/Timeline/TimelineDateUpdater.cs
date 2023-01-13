using FWI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public delegate DateTime DateTimeDelegate();
    public class TimelineDateUpdater : TimelineUpdater
    {
        static readonly DateTimeDelegate getDateTimeDef = () => DateTime.Now;
        DateTimeDelegate getDateTime;
        readonly Dictionary<string, WindowInfo> windowInfoDic;
        readonly Rank rank;
        DateTime begin, end;
        DateTime last;
        WindowInfo lastWI;
        OnEndDelegate onEnd;

        public TimelineDateUpdater() : this(begin: DateTime.MinValue, end: DateTime.MaxValue) 
        {

        }

        public TimelineDateUpdater(DateTime begin, DateTime end, WindowInfo initWI = null)
        {
            rank = new Rank();
            onEnd = (WindowInfo wi) => { };
            windowInfoDic = new Dictionary<string, WindowInfo>();
            getDateTime = getDateTimeDef;

            Reset(begin:begin, end:end, initWI:initWI);
        }

        public void Reset(DateTime begin, DateTime end, WindowInfo initWI = null)
        {
            this.begin = begin;
            this.end = end;
            last = DateTime.MinValue;
            lastWI = new NoWindowInfo();
            rank.Clear();
            windowInfoDic.Clear();

            if (!(initWI == null || initWI is NoWindowInfo))
            {
                if (end > initWI.Date) Add(initWI);
                else throw new TimeSeqeunceException();
            }
        }

        public void SetOnEnd(OnEndDelegate onEnd)
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
            if (lastWI is NoWindowInfo || lastWI.Date <= wi.Date) lastWI = wi;

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
            if (!(lastWI is NoWindowInfo)) time = AddRankLast(wi.Date);
            lastWI = wi;
            last = wi.Date;

            return time;
        }

        TimeSpan AddRankLast(DateTime toDate)
        {
            TimeSpan time;
            if (last >= begin) time = toDate - last; 
            else time = toDate - begin;

            if (!windowInfoDic.ContainsKey(lastWI.Name)) windowInfoDic[lastWI.Name] = lastWI;
            rank.Add(lastWI, time);

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

        bool HasNoLastWI() => (lastWI == null || lastWI is NoWindowInfo);

        public bool Empty => (rank.Count == 0);
        public bool HasOne() => (rank.Count > 0);
        public WindowInfo One()
        {
            return windowInfoDic[rank.One()];
        }

        public WindowInfo Last => lastWI;
        public DateTime BeginDate => begin;
        public DateTime EndDate => end;
    }
}
