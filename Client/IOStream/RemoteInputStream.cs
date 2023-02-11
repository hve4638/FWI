using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FWI;

namespace FWIClient
{
    public class RemoteInputStream : RemoteIO, IInputStream
    {
        readonly SimpleConnection.Server server;
        public RemoteInputStream(SimpleConnection.Server server)
        {
            this.server = server;
            ThrowWhenDisconnect = false;
        }
        public string? Read()
        {
            if (server.Connected)
            {
                try
                {
                    return server.ReceiveOne();
                }
                catch (SocketException)
                {
                    Disconnect();
                }
            }

            return null;
        }
    }
}
