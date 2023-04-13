using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    [Obsolete]
    public class IgnoreMapLegacy
    {
        readonly HashSet<string> set;

        public IgnoreMapLegacy()
        {
            set = new HashSet<string>();
        }

        public void Add(string name)
        {
            set.Add(name);
        }

        public bool Contains(string name) => set.Contains(name);

        public IgnoreMapLegacy Import(string filename)
        {
            var reader = new StreamReader(filename);
            Import(reader);
            reader.Close();
            return this;
        }
        public IgnoreMapLegacy Import(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var name = reader.ReadLine().Trim();
                if (name.Length > 0) Add(name);
            }
            return this;
        }

        public IgnoreMapLegacy Export(string filename)
        {
            var writer = new StreamWriter(filename);
            Export(writer);
            writer.Close();
            return this;
        }
        public IgnoreMapLegacy Export(StreamWriter writer)
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
