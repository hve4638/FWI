﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public interface ITimeline : ITimelineReadOnly
    {
        void AddLog(WindowInfo wi);
        void AddLog(WindowInfo[] log);
        void AddLog(List<WindowInfo> log);
        void AddLog(ReadOnlyCollection<WindowInfo> log);

        void Import(StreamWriter stream, SerializeType type);
    }
}
