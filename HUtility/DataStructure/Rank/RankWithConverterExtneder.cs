using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    public static class RankWithConverterExtneder
    {
        public static void Merge<K, T>(this Rank<K, T, int> rank, Rank<K, T, int> other)
        {
            foreach (var (key, value) in other) rank[key] += value;
        }

        public static void Merge<K, T>(this Rank<K, T, TimeSpan> rank, Rank<K, T, TimeSpan> other)
        {
            foreach (var (key, value) in other) rank[key] += value;
        }
    }
}
