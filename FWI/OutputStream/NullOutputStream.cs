using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class NullOutputStream : IOutputStream
    {
        public void Write(string value)
        {
        }

        public void WriteLine(string value)
        {
        }

        public void Flush()
        {
        }
    }
}
