using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public interface IOutputStream
    {
        void Write(string value);
        void WriteLine(string value);
        void Flush();
    }
}
