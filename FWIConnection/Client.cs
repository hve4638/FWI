using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace FWIConnection
{
    public class Client
    {
        readonly int maximumBufferSize = 8192;
        private byte[] receiveBuffer;
        IReceiver receiver;
        readonly Socket socket;
        public bool Verbose { get; set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public bool Connected => socket.Connected;

        public Client(string ipAddr, int port)
        {
            receiver = new DefaultReceiver();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IP = ipAddr;
            Port = port;
            Verbose = false;
        }

        void VerboseWriteLine(string value)
        {
            if (Verbose) Console.WriteLine(value);
        }

        public void SetReceiver(IReceiver receiver)
        {
            this.receiver = receiver;
        }

        public bool Connect()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            try
            {
                socket.Connect(endPoint);
            }
            catch (SocketException)
            {
                return false;
            }

            receiver.Connect(socket);

            return true;
        }

        public void WaitForConnect()
        {
            while (!Connected)
            {
                Thread.Sleep(100);
            }
        }
        public void WaitForReceive() => BeginReceive();

        public void BeginReceive()
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

        public void WaitForReceiveLoop()
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

    }
}