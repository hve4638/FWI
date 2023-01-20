using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class AFKWindowInfo : WindowInfo
    {
        public AFKWindowInfo(DateTime? date = null) : base(title: "", name: "", alias: "AFK", date: date)
        {

        }

        public override WindowInfo Copy()
        {
            return new AFKWindowInfo(date: Date);
        }
    }
}
