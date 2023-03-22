using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    internal interface IRankDictionary<T, S> : IEnumerable<(T, S)>
    {
        bool Remove(T item);
        bool Has(T item);
        void Set(T key, S value);
        S Get(T key);
        void Clear();
    }
}
