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
    public class AliasMap
    {
        readonly Dictionary<string, string> dict;

        public AliasMap()
        {
            dict = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            dict[key] = value;
        }

        public void Filter(ref WindowInfo wi)
        {
            if (dict.ContainsKey(wi.Name)) wi.Alias = dict[wi.Name];
        }

        public AliasMap Import(string filename)
        {
            var reader = new StreamReader(filename);
            Import(reader);
            reader.Close();
            return this;
        }
        public AliasMap Import(StreamReader reader)
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

        public AliasMap Export(string filename)
        {
            var writer = new StreamWriter(filename);
            Export(writer);
            writer.Close();
            return this;
        }
        public AliasMap Export(StreamWriter writer)
        {
            foreach(var item in dict) writer.Write($"{item.Key} : {item.Value}\n");
            return this;
        }

        public ReadOnlyDictionary<string, string> Items => new ReadOnlyDictionary<string, string>(dict);
    }
}
