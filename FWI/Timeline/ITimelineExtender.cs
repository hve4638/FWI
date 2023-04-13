using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public static class ITimelineExtender
    {

        public static void AddWIs(ITimeline timeline, WindowInfo[] wis)
        {
            foreach(var wi in wis) timeline.AddWI(wi);
        }
        public static void AddWIs(ITimeline timeline, List<WindowInfo> wis)
        {
            foreach (var wi in wis) timeline.AddWI(wi);
        }
        public static void AddWIs(ITimeline timeline, ReadOnlyCollection<WindowInfo> wis)
        {
            foreach (var wi in wis) timeline.AddWI(wi);
        }

        public static void Import(ITimeline timeline, string filePath)
        {
            using var reader = new StreamReader(filePath);
            timeline.Import(reader, SerializeType.Normal);
        }
        public static void Export(ITimeline timeline, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            timeline.Export(writer, SerializeType.Normal);
        }
    }
}
