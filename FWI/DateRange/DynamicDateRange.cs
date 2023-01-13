using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    internal class DynamicDateRange : DateRange
    {
        public DynamicDateRange(DateTime begin, DateTime end) : base(begin, end)
        {

        }

        public new DateTime Begin
        {
            get { return begin; }
            set { begin = value; }
        }
        public new DateTime End
        {
            get { return end; }
            set { end = value; }
        }
    }
}
