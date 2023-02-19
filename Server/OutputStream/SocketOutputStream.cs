using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FWI;
using FWIConnection;
using FWI.Message;

namespace FWIServer
{
    class SocketOutputStream : IOutputStream
    {
        readonly Socket socket;
        readonly StringBuilder builder;
        public SocketOutputStream(Socket socket)
        {
            this.socket = socket;
            builder = new StringBuilder();
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
            if (builder.Length > 0)
            {
                var message = new TextMessage()
                {
                    Text = builder.ToString()
                };
                var bytes = message.Serialize();

                builder.Clear();
                socket.Send(bytes);
            }
        }
    }
}
