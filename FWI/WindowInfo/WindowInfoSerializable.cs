using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class WindowInfoSerializable
    {
        public bool Special { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public DateTime Date { get; set; }
    }
}
