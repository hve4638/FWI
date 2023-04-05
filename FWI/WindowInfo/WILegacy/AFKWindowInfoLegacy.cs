using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class AFKWindowInfoLegacy : WindowInfoLegacy
    {
        public AFKWindowInfoLegacy(DateTime? date = null) : base(title: "", name: "", alias: "AFK", date: date)
        {

        }

        public override WindowInfoLegacy Copy()
        {
            return new AFKWindowInfoLegacy(date: Date);
        }
    }
}
