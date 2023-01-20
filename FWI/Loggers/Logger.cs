using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public interface Logger
    {
        int Length { get; }
        int Count { get; }
        Logger AppendWindowInfo(WindowInfo wi);
        Logger AddWI(WindowInfo wi);

        ReadOnlyCollection<WindowInfo> GetLog();
        ReadOnlyCollection<WindowInfo> GetLog(DateTime from, DateTime to);

        Dictionary<int, RankResult<WindowInfo>> GetRanks();
        Dictionary<int, RankResult<WindowInfo>> GetRanks(int beginRank, int endRank);

        void Update(DateTime time);

        void SetLoggingInterval(int minutes);
        void SetOnLoggingListener(Action<WindowInfo> listener);
        /// <summary>
        /// 해당 범위에 포함되는 WindowInfo를 가져옵니다.범위 경계에 걸치는 WindowInfo도 가져옵니다.
        /// </summary>
        ReadOnlyCollection<WindowInfo> GetLog(DateRange range);

        void Import(string filename);
        void Export(string filename);
    }
}
