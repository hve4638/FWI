using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class NullOutputStream : IOutputStream
    {
        private static readonly Lazy<NullOutputStream> lazy = new Lazy<NullOutputStream>(() => new NullOutputStream());
        public static NullOutputStream Instance { get { return lazy.Value; } }
        private NullOutputStream() { }
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
