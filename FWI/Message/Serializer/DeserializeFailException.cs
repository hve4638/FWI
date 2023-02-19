using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Message
{
    public class DeserializeFailException : Exception
    {
        public DeserializeFailException() : base()
        {

        }

        public DeserializeFailException(string message) : base(message)
        {

        }
    }
}
