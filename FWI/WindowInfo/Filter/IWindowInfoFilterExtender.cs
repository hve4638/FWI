using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public static class IWindowInfoFilterExtender
    {
        public static void Filter(this IWindowInfoFilter filter, IEnumerable<WindowInfo> wis)
        {
            foreach (var item in wis)
            {
                var wi = item;
                filter.Filter(ref wi);
            }
        }
    }
}
