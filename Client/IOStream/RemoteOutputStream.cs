using FWI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public class RemoteOutputStream : RemoteIO, IOutputStream
    {
        readonly SimpleConnection.Server server;
        readonly StringBuilder builder;

        public RemoteOutputStream(SimpleConnection.Server server)
        {
            this.server = server;
            builder = new StringBuilder();
            ThrowWhenDisconnect = false;
        }
        public void Write(string value)
        {
            if (!server.Connected) Disconnect();
            else if(value.Length > 0)
            {
                server.Send(value);
                builder.Clear();
            }
        }
        public void WriteLine(string value)
        {
            Write($"{value}\n");
        }
        public void Flush()
        {

        }
    }
}
