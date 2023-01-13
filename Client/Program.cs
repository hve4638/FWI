using System.Text;
using FWIConnection;
using FWIConnection.Message;
using System.Threading;
using CommandLine;
using System.Diagnostics;

namespace FWIClient
{
    public class Program
    {
        public readonly string Version = "1.0";

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        static void Run(Options options)
        {
            Console.WriteLine($"Connection {options.IP}:{options.Port}");
            Console.WriteLine($"Verbose : {options.Verbose}");
            options.Trace = true;
            if (options.Trace) Console.WriteLine($"Trace Mode");
            else Console.WriteLine($"Show Infomation Mode");

            RunClient(options);
        }

        static void RunClient(Options options)
        {
            var manager = new ClientManager();
            var client = new Client(options.IP, options.Port);
            client.SetReceiver(new Receiver(manager: manager));


            var connected = client.Connect();
            if (!connected)
            {
                Console.WriteLine("Connect Fail");
                return;
            }

            var threadPrompt = StartPrompt(client);
            client.WaitForConnection();

            if (options.Trace)
            {
                manager.RequestPrivillegeTrace()
                    .WithAccepted(() => {
                        StartTrace(client);
                    });
            }

            client.ReceiveProgress();


            threadPrompt.Join();
        }

        static public Thread StartTrace(Client client)
        {
            var thread = new Thread(
                () => TraceForegroundWindow.Trace(
                    onTrace: (wi) =>
                    {
                        var bw = new ByteWriter();
                        WindowInfoMessage.Make(bw, wi.Name, wi.Title, wi.Date);

                        client.Send(bw.ToBytes());
                    },
                    traceInterval: 1000
                )
            );
            thread.Start();
            return thread;
        }

        static public Thread StartPrompt(Client client)
        {
            var prompt = new PromptClient(client);
            var thread = new Thread(prompt.Run);
            thread.Start();
            return thread;
        }

        public class Options
        {
            [Option('p', "port", Required = false, Default = 7000, HelpText = "Destination Port")]
            public int Port { get; set; }

            [Option('d', "dest", Default = "127.0.0.1", HelpText = "Destination IP Address")]
            public string? IP { get; set; }

            [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }

            [Option('b', "background", Default = false, HelpText = "Running on background.")]
            public bool Background { get; set; }

            [Option('t', "trace", Default = false, HelpText = "TraceMode")]
            public bool Trace { get; set; }
        }
    }
}