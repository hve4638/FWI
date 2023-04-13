using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    /// <summary>
    /// WindowInfo 기록 타임라인
    /// </summary>
    public interface ITimeline : ITimelineReadOnly
    {
        void AddWI(WindowInfo wi);
        void Import(StreamReader stream, SerializeType type);
    }
}
