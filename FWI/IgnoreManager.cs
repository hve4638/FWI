using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class IgnoreMap
    {
        readonly HashSet<string> set;

        public IgnoreMap()
        {
            set = new HashSet<string>();
        }

        public void Add(string name)
        {
            set.Add(name);
        }

        public bool Contains(WindowInfoLegacy wi) => Contains(wi.Name);
        public bool Contains(string name) => set.Contains(name);

        public IgnoreMap Import(string filename)
        {
            var reader = new StreamReader(filename);
            Import(reader);
            reader.Close();
            return this;
        }
        public IgnoreMap Import(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var name = reader.ReadLine().Trim();
                if (name.Length > 0) Add(name);
            }
            return this;
        }

        public IgnoreMap Export(string filename)
        {
            var writer = new StreamWriter(filename);
            Export(writer);
            writer.Close();
            return this;
        }
        public IgnoreMap Export(StreamWriter writer)
        {
            foreach (var item in set) writer.Write($"{item}\n");
            return this;
        }

        public HashSet<string> Items {
            get {
                var copy = new HashSet<string>();
                copy.UnionWith(set);
                return copy;
            }
        }
    }
}
