using FWIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FWIConnection.Message;
using FWI.Results;

namespace FWIClient
{
    internal class Receiver : IReceiver
    {
        readonly ClientManager manager;
        Socket? socket = null;
        public Receiver(ClientManager manager) {
            this.manager = manager;
        }

        public void Connect(Socket socket)
        {
            this.socket = socket;
            manager.Connected = true;
        }
        public void Disconnect()
        {
            manager.Connected = false;
        }
        public void Receive(in byte[] buf, int size)
        {
            var br = new ByteReader(buf, size);
            var op = (MessageOp)br.ReadShort();

            switch (op)
            {
                case MessageOp.Message:
                    ResponseMessage(br);
                    break;
                case MessageOp.ResponseToBeTarget:
                case MessageOp.ResponsePrivillegeTrace:
                    ResponseToBeTarget(br);
                    break;
                default:
                    break;
            }
        }
        void ResponseToBeTarget(ByteReader br)
        {
            var nonce = br.ReadShort();
            var accepted = (br.ReadShort() == 1);
            
            manager.ResponseToBeTarget(nonce, accepted);
        }

        static void ResponseMessage(ByteReader br)
        {
            var str = br.ReadString();
            Program.Out.WriteLine(str);
        }
    }
}
