using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    [Obsolete]
    public class AliasMapLegacy
    {
        readonly Dictionary<string, string> dict;

        public AliasMapLegacy()
        {
            dict = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            dict[key] = value;
        }

        public void Filter(ref WindowInfoLegacy wi)
        {
            if (dict.ContainsKey(wi.Name)) wi.Alias = dict[wi.Name];
        }
        public void Filter(IEnumerable<WindowInfoLegacy> enumerable)
        {
            foreach (var item in enumerable)
            {
                var wi = item;
                Filter(ref wi);
            }
        }

        public AliasMapLegacy Import(string filename)
        {
            var reader = new StreamReader(filename);
            Import(reader);
            reader.Close();
            return this;
        }
        public AliasMapLegacy Import(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var splited = line.Split(':');

                if (splited.Length == 2)
                {
                    var key = splited[0].Trim();
                    var value = splited[1].Trim();

                    dict[key] = value;
                }
            }

            return this;
        }

        public AliasMapLegacy Export(string filename)
        {
            var writer = new StreamWriter(filename);
            Export(writer);
            writer.Close();
            return this;
        }
        public AliasMapLegacy Export(StreamWriter writer)
        {
            foreach(var item in dict) writer.Write($"{item.Key} : {item.Value}\n");
            return this;
        }

        public ReadOnlyDictionary<string, string> Items => new ReadOnlyDictionary<string, string>(dict);
    }
}
