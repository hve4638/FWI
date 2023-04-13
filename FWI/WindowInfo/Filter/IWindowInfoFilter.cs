using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public interface IWindowInfoFilter
    {
        void Filter(ref WindowInfo wi);
    }
}
