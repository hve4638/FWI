using FWIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FWI.Message;
using FWI.Results;

namespace FWIClient
{
    internal class Receiver : IReceiver
    {
        readonly ClientManager manager;
        public Receiver(ClientManager manager) {
            this.manager = manager;
        }

        public void Connect(Socket socket)
        {
            manager.Sender.Connected = true;
        }
        public void Disconnect()
        {
            manager.Sender.Connected = false;
        }
        public void Receive(in byte[] buf, int size)
        {
            var br = new ByteReader(buf, size);
            var op = (MessageOp)br.PeekShort();

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
        static void ResponseMessage(ByteReader br)
        {
            var message = TextMessage.Deserialize(br);

            Program.Out.WriteLine(message.Text);
            Program.Out.Flush();
        }
        void ResponseToBeTarget(ByteReader br)
        {
            var message = ResponseToBeTargetMessage.Deserialize(br);
            var nonce = message.Id;
            var accepted = message.Accepted;

            manager.ResponseToBeTarget(nonce, accepted);
        }

    }
}
