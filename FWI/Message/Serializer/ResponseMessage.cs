using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Message
{
    public class ResponseMessage
    {
        public bool Failed { get; set; }

        public ResponseMessage()
        {
            Failed = false;
        }
    }
}
