using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Message
{
    public class SerializeFailException : Exception
    {
        public SerializeFailException() : base()
        {

        }

        public SerializeFailException(string message) : base(message)
        {

        }
    }
}
