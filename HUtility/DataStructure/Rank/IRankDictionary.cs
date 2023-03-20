using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    internal interface IRankDictionary<T> : IEnumerable<(T, TimeSpan)>
    {
        bool Remove(T item);
        bool Has(T item);
        void Set(T key, TimeSpan value);
        TimeSpan Get(T key);
        void Clear();
    }
}
