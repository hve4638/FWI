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
        const string FILENAME_RANK = "fwi.rank.{0}.json";
        const string FILENAME_TIMELINE = "fwi.timeline.{0}.json";
        int maximumSize;
        readonly Timeline timeline;
        readonly DateRank rank;
        public SingleLogger(int maximumSize = 50, DateTime? date = null)
        {
            timeline = new Timeline();
            rank = new DateRank();
            this.maximumSize = maximumSize;
        }

        public void SetLoggingInterval(int minutes=0) => timeline.SetInterval(minutes);
        public void SetDateTimeDelegate(DateTimeDelegate dateTimeDelegate) => timeline.SetDateTimeDelegate(dateTimeDelegate);
        public Logger AddWI(WindowInfo wi) => AppendWindowInfo(wi);
        public Logger AppendWindowInfo(WindowInfo wi)
        {
            rank.Add(wi);
            timeline.AddLog(wi);
            return this;
        }

        public void SetOnLoggingListener(Action<WindowInfo> onLoggingListener) => timeline.SetOnAddListener(onLoggingListener);

        public static SingleLogger operator +(SingleLogger logger, WindowInfo wi)
        {
            logger.AppendWindowInfo(wi);
            return logger;
        }

        public bool IsFull => (timeline.Count >= maximumSize);
        public int Length => timeline.Count;
        public int Count => timeline.Count;

        public ReadOnlyCollection<WindowInfo> GetLog() => timeline.GetTimeline();
        public ReadOnlyCollection<WindowInfo> GetLog(DateTime from, DateTime to) => GetLog(new DateRange(from, to));
        public ReadOnlyCollection<WindowInfo> GetLog(DateRange range) => timeline.GetTimeline(range);

        public void Import(string path)
        {
            string dir, name;
            (dir, name) = HUtils.SplitPath(path);
            var timelinePath = FileNameFilter(FILENAME_TIMELINE, name, dir);
            var rankPath = FileNameFilter(FILENAME_RANK, name, dir);

            timeline.Import(timelinePath);
            rank.Import(rankPath);
        }

        public void Export(string path)
        {
            string dir, name;
            (dir, name) = HUtils.SplitPath(path);
            var timelinePath = FileNameFilter(FILENAME_TIMELINE, name, dir);
            var rankPath = FileNameFilter(FILENAME_RANK, name, dir);

            timeline.Export(timelinePath);
            rank.Export(rankPath, name);
        }

        void ExecuteFileIOSafe(Action execute, Action onCatchException = null)
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

        string FileNameFilter(string format, string name, string directory = "")
        {
            var formatted = string.Format(format, name);
            if (directory == "") return formatted;
            else return directory + @"\" + formatted;
        }

        public int GetContentsHashCode()
        {
            return rank.GetContentsHash() ^ timeline.GetContentsHashCode();
        }
    }
}
