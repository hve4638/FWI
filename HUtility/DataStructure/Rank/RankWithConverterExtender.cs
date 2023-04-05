using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    public static class RankWithConverterExtender
    {
        public static void Merge<K, T>(this Rank<K, T, int> rank, Rank<K, T, int> other)
        {
            rank.Merge(other, (x, y) => x + y);
        }

        public static void Merge<K, T>(this Rank<K, T, TimeSpan> rank, Rank<K, T, TimeSpan> other)
        {
            rank.Merge(other, (x, y) => x + y);
        }
    }
}
