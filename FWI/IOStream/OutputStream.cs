using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class StandardOutputStream : IOutputStream
    {
        public void Write(string value)
        {
            Console.Write(value);
        }

        public void WriteLine(string value)
        {
            Console.WriteLine($"\r{value}");
        }

        public void Flush()
        {
            Console.Out.Flush();
        }
    }
}
