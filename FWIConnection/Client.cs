using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

namespace FWIConnection
{
    public class Client
    {
        readonly int maximumBufferSize = 8192;
        private byte[] receiveBuffer;
        IReceiver receiver;
        bool verbose;
        readonly Socket socket;
        readonly string ipAddr;
        readonly int port;
        bool connected;

        public Client(string ipAddr, int port)
        {
            receiver = new DefaultReceiver();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ipAddr = ipAddr;
            this.port = port;
            verbose = false;
            connected = false;
        }

        void VerboseWriteLine(string value)
        {
            if (verbose) Console.WriteLine(value);
        }

        public void SetReceiver(IReceiver receiver)
        {
            this.receiver = receiver;
        }

        public bool Connect()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(ipAddr), 7000);
            try
            {
                socket.Connect(endPoint);
            }
            catch (SocketException)
            {
                return false;
            }

            connected = true;
            receiver.Connect(socket);

            return true;
        }

        public void WaitForConnect()
        {
            while (!connected)
            {

            }
        }

        public void WaitForReceive()
        {
            receiveBuffer = receiveBuffer ?? new byte[maximumBufferSize];
            try
            {
                int size = socket.Receive(receiveBuffer);
                if (size > 0) receiver.Receive(receiveBuffer, size);
            }
            catch (SocketException e)
            {
                VerboseWriteLine("Socket Disconnected");
                VerboseWriteLine("-------------------------------");
                VerboseWriteLine($"{e}");
                VerboseWriteLine("-------------------------------");
            }
        }

        public void ReceiveProgress()
        {
            receiveBuffer = receiveBuffer ?? new byte[maximumBufferSize];
            try
            {
                while (true)
                {
                    int size = socket.Receive(receiveBuffer);
                    if (size > 0) receiver.Receive(receiveBuffer, size);
                }
            }
            catch (SocketException e)
            {
                VerboseWriteLine("Socket Disconnected");
                VerboseWriteLine("-------------------------------");
                VerboseWriteLine($"{e}");
                VerboseWriteLine("-------------------------------");
            }
        }

        public void Send(in byte[] buff)
        {
            socket.Send(buff, SocketFlags.None);
        }

        public void Disconnect()
        {
            connected = false;
            try
            {
                receiver.Disconnect();
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }

        public string IP { get { return ipAddr; } }
        public int Port { get { return port; } }
        public bool IsConnected { get { return connected; } }
    }
}