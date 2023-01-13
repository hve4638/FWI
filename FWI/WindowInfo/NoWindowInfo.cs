using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWI
{
    public class NoWindowInfo : WindowInfo
    {
        public NoWindowInfo(DateTime? date = null) : base(title:"", name: "", alias: "__nowindow", date: date)
        {

        }

        public override WindowInfo Copy()
        {
            return this;
        }
    }
}
