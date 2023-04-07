using HUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public interface ILogger
    {
        ILogger AddWI(WindowInfo wi);
        ILogger AddDefaultWI(DateTime date);

        void SetPath(string path);

        ReadOnlyCollection<WindowInfo> GetLog();
        /// <summary>
        /// 해당 범위에 포함되는 WindowInfo를 가져옵니다.범위 경계에 걸치는 WindowInfo도 가져옵니다.
        /// </summary>
        ReadOnlyCollection<WindowInfo> GetLog(DateRange range);

        [Obsolete]
        ReadOnlyCollection<WindowInfo> GetTimeline();
        /// <summary>
        /// 해당 범위에 포함되는 WindowInfo를 가져옵니다.범위 경계에 걸치는 WindowInfo도 가져옵니다.
        /// </summary>
        [Obsolete]
        ReadOnlyCollection<WindowInfo> GetTimeline(DateRange range);

        IRank<WindowInfo, TimeSpan> GetRanks();
        IRank<WindowInfo, TimeSpan> GetRanks(int beginRank, int endRank);

        void Update(DateTime time);
        void SetLoggingInterval(int minutes);
        void SetOnLoggingListener(Action<WindowInfo> listener);


        void Import(string path);
        void Export(string path);
    }
}
