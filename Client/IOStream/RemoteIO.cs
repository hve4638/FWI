using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public class RemoteIO
    {
        public bool ThrowWhenDisconnect { get; set; }
        public Action? OnDisconnect { get; set; }

        public virtual void Disconnect()
        {
            OnDisconnect?.Invoke();
            OnDisconnect = null;

            if (ThrowWhenDisconnect) throw new RemoteIODisconnectException();
        }
    }
}
