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
    internal class WindowInfoAliasFilter : IWindowInfoFilter
    {
        readonly Dictionary<string, string> dict = new Dictionary<string, string>();

        public void Filter(ref WindowInfo wi)
        {
            if (dict.ContainsKey(wi.Name)) wi.Alias = dict[wi.Name];
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
        public void Import(StreamReader reader) => dict.ImportKV(reader, x => x, x => x);
        public void Export(StreamWriter writer) => dict.ExportKV(writer, x => x, x => x);

        public string this[string index]
        {
            get => dict[index];
            set => dict[index] = value;
        }
        public ReadOnlyDictionary<string, string> Items => new ReadOnlyDictionary<string, string>(dict);
    }
}
