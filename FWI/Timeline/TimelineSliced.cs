﻿using HUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace FWI
{
    public class TimelineSliced : ITimelineReadOnly, IEnumerable<WindowInfo>
    {
        readonly ITimelineReadOnly original;
        public DateRange Range { get; private set; }

        public int Count => throw new NotImplementedException();

        public TimelineSliced(ITimeline original, DateRange range)
        {
            this.original = original;
            Range = range;
        }

        public WindowInfo this[DateTime date]
        {
            get
            {
                if (Range.Contains(date)) return original[date];
                else throw new IndexOutOfRangeException();
            }
        }

        public ITimelineReadOnly Slice(DateTime begin, DateTime end) => Slice(new DateRange(begin, end));
        public ITimelineReadOnly Slice(DateRange range)
        {
            var newRange = Range & range;
            
            return original.Slice(newRange);
        }

        public void Export(StreamWriter stream, SerializeType type) => original.Export(stream, type);

        public IEnumerator<WindowInfo> GetEnumerator()
        {
            var count = 0;
            for (int i = 2; i <= 6 && i < count; i++)
            {
                yield return WindowInfo.NoWindow;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
