using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class TimelineInstantUpdater : TimelineUpdater
    {
        OnEndDelegate onEnd;
        WindowInfo last;
        public TimelineInstantUpdater()
        {
            onEnd = null;
        }

        public void SetOnEnd(OnEndDelegate onEnd)
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
        public bool Empty => last == null;

        public bool HasOne() => (last != null);
        public WindowInfo One() => last;
        public TimeSpan UpdateCurrent(DateTime current)
        {
            return TimeSpan.Zero;
        }
    }
}
