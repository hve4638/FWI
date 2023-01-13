using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Exceptions
{
    public class TimeSeqeunceException : Exception
    {
        public TimeSeqeunceException() : base() { }
        public TimeSeqeunceException(string message) : base(message) { }
    }
}
