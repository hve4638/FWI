using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;

namespace FWIConnection
{
    public class Server
    {
        readonly int maximumBufferSize = 8192;
        Func<IReceiver> getReceiver;
        readonly int port;
        public bool Verbose { get; set; }

        public Server(int port, bool verbose = false)
        {
            getReceiver = () => new DefaultReceiver();
            this.port = port;
            Verbose = verbose;
        }

        public void SetReceiverGetter(Func<IReceiver> getReceiver)
        {
            this.getReceiver = getReceiver;
        }

        void VerboseWriteLine(string value)
        {
            if (Verbose) Console.WriteLine(value);
        }

        public Thread RunAsync()
        {
            Thread thread = new Thread(Run);
            thread.Start();

            return thread;
        }

        public void Run()
        {
            var connector = Listen();

            while(true) connector.Accept();
        }

        SocketConnector Listen(int backlog = 5)
        {
            var connnector = new SocketConnector(
                port : port, backlog : backlog,
                onEstablish : Connect
            );

            return connnector;
        }
        void Connect(Socket socket)
        {
            var receiver = getReceiver();
            receiver.Connect(socket);
            VerboseWriteLine("Socket Connected");

            try
            {
                byte[] buff = new byte[maximumBufferSize];
                while (true)
                {
                    int size = socket.Receive(buff);

                    if (size == 0) break;
                    else receiver.Receive(buff, size);
                }
            }
            catch (SocketException e)
            {
                VerboseWriteLine("Socket Disconnected");
                VerboseWriteLine("-------------------------------");
                VerboseWriteLine($"{e}");
                VerboseWriteLine("-------------------------------");
            }
            finally
            {
                receiver.Disconnect();
                socket.Close();
            }
        }

        class SocketConnector
        {
            readonly Socket socket;
            readonly Action<Socket> onEstablish;

            public SocketConnector(int port, int backlog, Action<Socket> onEstablish)
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ep);
                socket.Listen(backlog);

                this.onEstablish = onEstablish;
            }

            public Socket Accept()
            {
                Socket clientSocket = socket.Accept();
                Thread thread = new Thread(() =>
                {
                    onEstablish(clientSocket);
                });
                thread.Start();

                return clientSocket;
            }

            public void Close()
            {
                socket.Close();
            }
        }
    }
}