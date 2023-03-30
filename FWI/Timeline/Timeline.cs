#nullable enable
using FWI.Exceptions;
using HUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class Timeline : ITimeline
    {
        static readonly int initContentHash = "timelineHash".GetHashCode();
        Func<DateTime> getCurrentDate;
        readonly List<WindowInfo> timelineLog;
        ITimelineUpdater updater;
        TimeSpan interval;
        DateTime lastTime;
        WindowInfo lastWI;
        WindowInfo lastAddedWI;
        Action<WindowInfo> addListener;
        public DateRange Range { get; private set; }
        public DateTime LastDateTime { get; private set; }
        public DateTime CurrentDateTime => getCurrentDate();


        public Timeline()
        {
            lastWI = new NoWindowInfo();
            lastAddedWI = new NoWindowInfo();
            lastTime = DateTime.MinValue;
            timelineLog = new List<WindowInfo>();
            getCurrentDate = ()=>DateTime.Now;
            addListener = (WindowInfo item) => { return; };
            Range = new DateRange(DateTime.MinValue, DateTime.MinValue);

            SetInterval(0);
        }

        public Timeline SetInterval(int minutes = 0)
        {
            if (minutes == 0)
            {
                interval = TimeSpan.Zero;
                updater = new TimelineInstantUpdater();
            }
            else
            {
                interval = new TimeSpan(0, minutes, 0);
                updater = new TimelineDateUpdater(begin: DateTime.MinValue, end: DateTime.MaxValue);
            }

            updater.SetOnEnd((WindowInfo item) =>
            {
                AddLogForce(item);
                ResetUpdater(getCurrentDate());
                addListener(item);
            });

            ResetUpdater(getCurrentDate());
            return this;
        }

        public void SetOnAddListener(Action<WindowInfo> onAddListener)
        {
            addListener = onAddListener;
        }

        private void ResetUpdater(DateTime current)
        {
            if (updater is TimelineDateUpdater)
            {
                var date = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, 0);
                var dateUpdater = updater as TimelineDateUpdater;
                var initWI = dateUpdater!.Last;

                dateUpdater.Reset(begin: date, end: date + interval, initWI: initWI);
                if (IsValidWI(lastWI)) dateUpdater.Add(lastWI);
            }
        }

        static bool IsValidWI(WindowInfo wi) => !(wi == null || wi is NoWindowInfo);

        // 내부 리스트에 정보를 추가. 시간 순서 등 충돌요소를 체크하지 않음.
        public void AddLogForce(WindowInfo wi)
        {
            if (IsNotLastAddedWI(wi))
            {
                lastAddedWI = wi;
                timelineLog.Add(wi);
            }
        }

        public void AddWI(WindowInfo wi) => AddLog(wi);
        public void AddWIs(WindowInfo[] wis) => AddLog(Array.AsReadOnly(wis));
        public void AddLog(WindowInfo[] log) => AddLog(Array.AsReadOnly(log));
        public void AddLog(List<WindowInfo> log) => AddLog(log.AsReadOnly());
        public void AddLog(ReadOnlyCollection<WindowInfo> items)
        {
            foreach(var wi in items) AddLog(wi);
        }
        public void AddLog(WindowInfo wi)
        {
            if (wi.Date < lastTime) throw new TimeSequenceException()
                {
                    Last = lastTime,
                    Input = wi.Date,
                };
            else if (IsLastWI(wi)) return;
            else
            {
                lastTime = wi.Date;
                lastWI = wi;
                updater.Add(wi);
            }
        }

        bool IsLastWI(WindowInfo wi) => (wi.Title == lastWI.Title && wi.Name == lastWI.Name);
        bool IsNotLastAddedWI(WindowInfo wi) => (wi.Title != lastAddedWI.Title || wi.Name != lastAddedWI.Name);


        public ReadOnlyCollection<WindowInfo> GetAllWIs()
        {
            UpdateDate();
            return timelineLog.AsReadOnly();
        }
        public ReadOnlyCollection<WindowInfo> GetWIs(DateRange range)
        {
            var list = GetAllWIs();

            return HUtils.CutWindowInfoList(list, range);
        }
        [Obsolete]
        public ReadOnlyCollection<WindowInfo> GetTimeline()
        {
            UpdateDate();
            return timelineLog.AsReadOnly();
        }
        [Obsolete]
        public ReadOnlyCollection<WindowInfo> GetTimeline(DateRange range)
        {
            var list = GetTimeline();

            return HUtils.CutWindowInfoList(list, range);
        }

        public void UpdateDate() => UpdateDate(GetCurrentDateTime());
        void UpdateDate(DateTime current)
        {
            LastDateTime = current;

            if (updater is TimelineDateUpdater && updater.IsEnd(current))
            {
                var dateUpdater = updater as TimelineDateUpdater;
                dateUpdater!.FillLast();
            }
        }

        public ITimelineReadOnly Slice(DateTime begin, DateTime end) => Slice(new DateRange(begin, end));
        public ITimelineReadOnly Slice(DateRange range)
        {
            var sliced = new TimelineSliced(this, range);

            return sliced;
        }

        public void Import(string filePath)
        {
            using var sr = new StreamReader(filePath);
            Import(sr);
        }
        public void Import(StreamReader reader)
        {
            WindowInfo item;
            string line;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                item = WindowInfo.Decode(line);
                AddLogForce(item);
            }
        }

        public void Export(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            Export(writer);
        }
        public void Export(StreamWriter writer)
        {
            foreach (WindowInfo wi in GetAllWIs()) writer.WriteLine(wi.Encoding());
        }

        public void SetMenualDateTime(Func<DateTime> dateTimeDelegate)
        {
            getCurrentDate = dateTimeDelegate;
        }

        public DateTime GetCurrentDateTime() => getCurrentDate();
        public int Count => timelineLog.Count;

        public WindowInfo this[DateTime date]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Find(DateTime date)
        {
            if (WICount == 0) return -1;

            var index = FindNoisy(date);
            var wi = this[index];

            if (wi.Date == date) return index;
            else return -1;
        }
        public int FindNearestPast(DateTime date)
        {
            if (WICount == 0) return -1;

            var index = FindNoisy(date);
            WindowInfo wi;
            wi = this[index];

            if (wi.Date == date || wi.Date < date) return index;
            else if (wi.Date > date && index - 1 >= 0) return index - 1;
            else return -1;
        }
        public int FindNearestFuture(DateTime date)
        {
            if (WICount == 0) return -1;

            var index = FindNoisy(date);
            WindowInfo wi;
            wi = this[index];

            if (wi.Date == date || wi.Date > date) return index;
            else if (wi.Date < date && index + 1 < WICount) return index + 1;
            else return -1;
        }
        public int FindNearest(DateTime date)
        {
            if (WICount == 0) return -1;

            var index = FindNoisy(date);
            int index2;
            WindowInfo wi, wi2;
            wi = this[index];

            if (wi.Date == date) return index;
            else if (wi.Date < date) index2 = index + 1;
            else index2 = index - 1;

            if (index2 < 0 || index2 >= WICount) return index;
            if (index > index2) (index, index2) = (index2, index);

            wi = this[index];
            wi2 = this[index2];
            double dif1 = Math.Abs((date - wi.Date).TotalMilliseconds);
            double dif2 = Math.Abs((date - wi2.Date).TotalMilliseconds);

            if (dif1 <= dif2) return index;
            else return index2;
        }

        int FindNoisy(DateTime date)
        {
            int min = 0;
            int max = WICount - 1;
            int result = 0;

            while (min <= max)
            {
                result = (min + max) / 2;

                if (this[result].Date == date) break;
                else if (this[result].Date < date) min = result + 1;
                else max = result - 1;
            }

            if (result < 0) result = 0;
            else if (result >= WICount) result = WICount - 1;

            return result;
        }

        public int GetContentsHashCode()
        {
            int hash = initContentHash;
            foreach(var item in timelineLog) hash ^= item.GetContentsHashCode();

            return hash;
        }

        public void Import(StreamWriter stream, SerializeType type)
        {
            throw new NotImplementedException();
        }

        public void Export(StreamWriter stream, SerializeType type)
        {
            throw new NotImplementedException();
        }

        public WindowInfo this[int index]
        {
            get
            {
                return timelineLog[index];
            }
        }
        public int WICount => timelineLog.Count;
    }
}
