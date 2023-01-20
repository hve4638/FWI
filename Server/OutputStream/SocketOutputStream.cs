using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FWI;
using FWIConnection;

namespace FWIServer
{
    class SocketOutputStream : IOutputStream
    {
        readonly Socket socket;
        readonly List<string> list;
        public SocketOutputStream(Socket socket)
        {
            this.socket = socket;
            list = new List<string>();
        }
        public void Write(string value)
        {
            list.Add(value);
        }
        public void WriteLine(string value)
        {
            list.Add(value);
            list.Add("\n");
        }
        public void Flush()
        {
            if (list.Count > 0)
            {
                var bw = new ByteWriter();
                bw.Write((short)MessageOp.Message);

                foreach (string str in list)
                {
                    bw.Write(str);
                }

                list.Clear();
                socket.Send(bw.ToBytes());
            }
        }
    }
}
