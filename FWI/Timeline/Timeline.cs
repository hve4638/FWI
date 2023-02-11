using FWI.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class Timeline
    {
        static readonly int initContentHash = "timelineHash".GetHashCode();
        DateTimeDelegate getCurrentDate;
        readonly List<WindowInfo> timelineLog;
        TimelineUpdater updater;
        TimeSpan interval;
        DateTime lastTime;
        WindowInfo lastWI;
        WindowInfo lastAddedWI;
        Action<WindowInfo> addListener;

        public Timeline()
        {
            lastWI = new NoWindowInfo();
            lastAddedWI = new NoWindowInfo();
            lastTime = DateTime.MinValue;
            timelineLog = new List<WindowInfo>();
            getCurrentDate = ()=>DateTime.Now;
            addListener = (WindowInfo item) => { return; };

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
                var initWI = dateUpdater.Last;

                dateUpdater.Reset(begin: date, end: date + interval, initWI: initWI);
                if (IsValidWI(lastWI)) dateUpdater.Add(lastWI);
            }
        }

        static bool IsValidWI(WindowInfo wi) => !(wi == null || wi is NoWindowInfo);

        /// <summary>
        /// 내부 리스트에 정보를 추가합니다. 시간 순서 등 충돌요소를 체크하지 않습니다.
        /// </summary>
        public void AddLogForce(WindowInfo wi)
        {
            if (IsNotLastAddedWI(wi))
            {
                lastAddedWI = wi;
                timelineLog.Add(wi);
            }
        }

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
        bool IsNotLastWI(WindowInfo wi) => !IsLastWI(wi);
        bool IsNotLastAddedWI(WindowInfo wi) => (wi.Title != lastAddedWI.Title || wi.Name != lastAddedWI.Name);


        public ReadOnlyCollection<WindowInfo> GetTimeline()
        {
            UpdateDate();
            return timelineLog.AsReadOnly();
        }
        public ReadOnlyCollection<WindowInfo> GetTimeline(DateRange range)
        {
            var list = GetTimeline();

            return HUtils.CutWindowInfoList(list, range);
        }

        public void UpdateDate() => UpdateDate(GetCurrentDateTime());
        void UpdateDate(DateTime current)
        {
            if (updater is TimelineDateUpdater && updater.IsEnd(current))
            {
                var dateUpdater = updater as TimelineDateUpdater;
                dateUpdater.FillLast();
            }
        }

        public void Import(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            Import(sr);
            sr.Close();
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
            StreamWriter writer = new StreamWriter(filePath);
            Export(writer);
            writer.Close();
        }
        public void Export(StreamWriter writer)
        {
            foreach (WindowInfo wi in GetTimeline()) writer.WriteLine(wi.Encoding());
        }

        public void SetDateTimeDelegate(DateTimeDelegate dateTimeDelegate)
        {
            getCurrentDate = dateTimeDelegate;
        }

        public DateTime GetCurrentDateTime() => getCurrentDate();
        public int Count => timelineLog.Count;
        
        public int GetContentsHashCode()
        {
            int hash = initContentHash;
            foreach(var item in timelineLog) hash ^= item.GetContentsHashCode();

            return hash;
        }
    }

}
