using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    public static class HashExtender
    {
        public static void Export<T>(this HashSet<T> hashSet, StreamWriter writer, Func<T, string> converter)
        {
            foreach (var item in hashSet)
            {
                var value = converter(item);
                writer.WriteLine(value);
            }
        }

        public static void Reader<T>(this HashSet<T> hashSet, StreamReader reader, Func<string, T> converter)
        {
            while (!reader.EndOfStream)
            {
                var sValue = reader.ReadLine().Trim();
                if (sValue.Length > 0)
                {
                    var value = converter(sValue);
                    hashSet.Add(value);
                }
            }
        }
    }
}
