using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FWIConnection
{
    public interface IReceiver
    {
        void Receive(in byte[] buf, int size);
        void Connect(Socket socket);
        void Disconnect();
    }

    public class DefaultReceiver : IReceiver
    {
        public void Receive(in byte[] buff, int size)
        {
            string data = Encoding.UTF8.GetString(buff, 0, size);
            Console.WriteLine("Receive : '{0}' (size:{1})", data, size);
        }

        public void Connect(Socket socket)
        {
            IPEndPoint ipPoint = socket.RemoteEndPoint as IPEndPoint;
            string ipAddr = ipPoint.Address.ToString();
            string port = ipPoint.Port.ToString();

            Console.WriteLine("{0}:{1} Connected ", ipAddr, port);
        }

        public void Disconnect()
        {
            Console.WriteLine("Client Disconnected ");
        }
    }
}
