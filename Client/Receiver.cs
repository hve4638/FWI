using FWIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FWIConnection.Message;

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
            manager.ServerSocket = socket;
        }
        public void Disconnect()
        {

        }
        public void Receive(in byte[] buf, int size)
        {
            var br = new ByteReader(buf, size);
            var op = (MessageOp)br.ReadShort();

            switch (op)
            {
                case MessageOp.Message:
                    socket!.ResponseMessage(br);
                    break;
                case MessageOp.Echo:
                    socket!.ResponseEcho(br);
                    break;
                case MessageOp.ResponsePrivillegeTrace:
                    ResponsePrivillegeTrace(br);
                    break;
                default:
                    break;
            }
        }
        public void ResponsePrivillegeTrace(ByteReader br)
        {
            var nonce = br.ReadShort();
            var accepted = (br.ReadShort() == 1);

            manager.ElevateUpdatePrivillege(nonce, accepted);

            if (accepted) Program.Out.WriteLine("[D][A] Target 지정됨");
            else
            {
                Program.Out.WriteLine($"[D][A] Target 거부됨");
                Program.Out.WriteLine($"[D][I] Observer Mode로 전환합니다");
            }
        }
    }

    static class SocketResponseExtender
    {
        static public void ResponseEcho(this Socket socket, ByteReader br)
        {
            var str = br.ReadString();
            Program.Out.WriteLine(str);

            var bw = new ByteWriter();
            bw.Write((short)MessageOp.Message);
            socket.Send(bw.ToBytes());
        }

        static public void ResponseMessage(this Socket socket, ByteReader br)
        {
            var str = br.ReadString();
            Program.Out.WriteLine(str);
        }

    }
}
