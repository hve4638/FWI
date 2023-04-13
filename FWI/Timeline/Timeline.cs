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
        readonly TimelineFinder finder;
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
            lastWI = WindowInfo.NoWindow;
            lastAddedWI = WindowInfo.NoWindow;
            lastTime = DateTime.MinValue;
            timelineLog = new List<WindowInfo>();
            getCurrentDate = () => DateTime.Now;
            addListener = (WindowInfo item) => { return; };
            Range = new DateRange(DateTime.MinValue, DateTime.MinValue);
            finder = new TimelineFinder(this);
            updater = new TimelineInstantUpdater();
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
                AddWIForce(item);
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
                if (!lastWI.IsNoWindow()) dateUpdater.Add(lastWI);
            }
        }

        /// <summary>
        /// 내부 리스트에 정보를 추가. 시간 순서 등 충돌요소를 체크하지 않음.
        /// </summary>
        public void AddWIForce(WindowInfo wi)
        {
            if (IsNotLastAddedWI(wi))
            {
                lastAddedWI = wi;
                timelineLog.Add(wi);
            }
        }

        public void AddWI(WindowInfo wi)
        {
            if (wi.Date < lastTime)
            {
                throw new TimeSequenceException()
                {
                    Last = lastTime,
                    Input = wi.Date,
                };
            }
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

        public ITimelineReadOnly Slice(DateRange range)
        {
            var sliced = new TimelineSliced(this, range);

            return sliced;
        }

        public void SetMenualDateTime(Func<DateTime> dateTimeDelegate) => getCurrentDate = dateTimeDelegate;
        public DateTime GetCurrentDateTime() => getCurrentDate();

        public int Find(DateTime date) => finder.Find(date);
        public int FindNearestPast(DateTime date) => finder.FindNearest(date);
        public int FindNearestFuture(DateTime date) => finder.FindNearestPast(date);
        public int FindNearest(DateTime date) => finder.FindNearest(date);

        public int GetContentsHashCode() => throw new NotImplementedException();

        public void Import(StreamReader stream, SerializeType type) => throw new NotImplementedException();
        public void Export(StreamWriter stream, SerializeType type) => throw new NotImplementedException();

        public WindowInfo this[int index] => timelineLog[index];
        public WindowInfo this[DateTime date]
        {
            get
            {
                var index = FindNearestPast(date);
                if (index == -1) return WindowInfo.NoWindow;
                return this[index];
            }
        }
        public int Count => timelineLog.Count;
    }
}
