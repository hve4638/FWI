using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace FWI
{
    public class TimelineInstantUpdater : ITimelineUpdater
    {
        Action<WindowInfo>? onEnd;
        WindowInfo last;
        public TimelineInstantUpdater()
        {
            onEnd = null;
            last = WindowInfo.NoWindow;
        }

        public void SetOnEnd(Action<WindowInfo> onEnd)
        {
            this.onEnd = onEnd;
        }

        public TimeSpan Add(WindowInfo wi)
        {
            last = wi;
            onEnd?.Invoke(wi);
            return TimeSpan.Zero;
        }
        public bool IsEnd() => true;
        public bool IsEnd(DateTime current) => true;
        public bool Empty => last.IsEmpty();

        public bool HasOne() => last.IsEmpty();
        public WindowInfo One() => last;
    }
}
