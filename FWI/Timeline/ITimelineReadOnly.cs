using HUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace FWI
{
    public interface ITimelineReadOnly
    {
        WindowInfo? this[DateTime date] { get; }

        ITimelineReadOnly Slice(DateRange range);
        ITimelineReadOnly Slice(DateTime start, DateTime end);

        void Export(StreamWriter stream, SerializeType type);
    }
}
