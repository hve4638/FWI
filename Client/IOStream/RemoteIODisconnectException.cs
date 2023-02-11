using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public class RemoteIODisconnectException : Exception
    {
        public RemoteIODisconnectException() : base() { }
        public RemoteIODisconnectException(string message) : base(message) { }
    }
}
