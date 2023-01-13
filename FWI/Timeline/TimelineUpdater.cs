using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public delegate void OnEndDelegate(WindowInfo item);
    public interface TimelineUpdater
    {
        void SetOnEnd(OnEndDelegate onEnd);
        TimeSpan Add(WindowInfo wi);
        bool IsEnd(DateTime current);
        bool IsEnd();
        bool Empty { get; }
        bool HasOne();
        WindowInfo One();
    }
}
