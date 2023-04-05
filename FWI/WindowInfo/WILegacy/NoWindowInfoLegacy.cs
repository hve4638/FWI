using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWI
{
    public class NoWindowInfoLegacy : WindowInfoLegacy
    {
        public NoWindowInfoLegacy(DateTime? date = null) : base(title:"", name: "", alias: "__nowindow", date: date)
        {

        }

        public override WindowInfoLegacy Copy()
        {
            return new NoWindowInfoLegacy(date: Date);
        }
    }
}
