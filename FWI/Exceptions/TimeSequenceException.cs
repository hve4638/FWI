using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Exceptions
{
    public class TimeSequenceException : Exception
    {
        public TimeSequenceException() : base()
        {
            Last = DateTime.MinValue;
            Input = DateTime.MinValue;
        }
        public TimeSequenceException(string message) : base(message)
        {
            Last = DateTime.MinValue;
            Input = DateTime.MinValue;
        }

        public DateTime Last { get; set; }
        public DateTime Input { get; set; }
    }
}
