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
            builder.Append(value);
        }
        public void WriteLine(string value)
        {
            builder.Append(value);
            builder.Append('\n');
        }
        public void Flush()
        {
            if (!server.Connected) Disconnect();
            else if (builder.Length > 0)
            {
                server.Send(builder.ToString());
                builder.Clear();
            }
        }
    }
}
