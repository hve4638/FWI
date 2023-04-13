using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    public static class DictionaryExtender
    {
        public static void ExportKV<T, U>(this Dictionary<T, U> dict, StreamWriter writer
            , Func<T, string> keyConverter, Func<U, string> valueConverter)
        {
            foreach (var item in dict)
            {
                var sKey = keyConverter(item.Key);
                var sValue = valueConverter(item.Value);
                writer.WriteLine($"{sKey} : {sValue}");
            }
        }

        public static int ImportKV<T, U>(this Dictionary<T, U> dict, StreamReader reader
            , Func<string, T> keyConverter, Func<string, U> valueConverter)
        {
            int fail = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var splited = line.Split(':');

                if (splited.Length == 2)
                {
                    var sKey = splited[0].Trim();
                    var sValue = splited[1].Trim();
                    var key = keyConverter(sKey);
                    var value = valueConverter(sValue);

                    dict[key] = value;
                }
                else
                {
                    fail++;
                }
            }

            return fail;
        }
    }
}
