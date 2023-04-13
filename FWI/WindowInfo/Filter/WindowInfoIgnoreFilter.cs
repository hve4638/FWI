using HUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class WindowInfoIgnoreFilter : IWindowInfoFilter
    {
        readonly HashSet<string> set = new HashSet<string>();

        public void Add(string name) => set.Add(name);

        public void Filter(ref WindowInfo wi)
        {
            if (set.Contains(wi.Name)) wi = WindowInfo.NoWindow;
        }
        public void Import(string filename)
        {
            using var reader = new StreamReader(filename);
            Import(reader);
        }
        public void Export(string filename)
        {
            using var writer = new StreamWriter(filename);
            Export(writer);
        }
        public void Import(StreamReader reader) => set.Reader(reader, x => x);
        public void Export(StreamWriter writer) => set.Export(writer, x => x);

        public HashSet<string> Items {
            get {
                var copy = new HashSet<string>();
                copy.UnionWith(set);
                return copy;
            }
        }
    }
}
