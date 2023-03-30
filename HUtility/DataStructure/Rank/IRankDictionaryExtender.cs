using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    public static class IRankDictionaryExtender
    {
        internal static void Add<T>(this RankDictionary<T, TimeSpan> rankDictionary, T key, TimeSpan value)
        {
            if (rankDictionary.Has(key))
            {
                var time = rankDictionary.Get(key) + value;
                rankDictionary.Remove(key);
                rankDictionary.Set(key, time);
            }
            else
            {
                rankDictionary.Set(key, value);
            }
        }

        internal static void Copy<T>(this RankDictionary<T, TimeSpan> src, ref RankDictionary<T, TimeSpan> dest)
        {
            dest.Clear();

            foreach ((var key, var value) in src) dest.Set(key, value);
        }
    }
}
