using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class StandardInputStream : IInputStream
    {
        public string Read()
        {
            return Console.ReadLine();
        }
    }
}
