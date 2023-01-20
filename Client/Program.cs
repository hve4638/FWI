using System.Text;
using FWIConnection;
using FWIConnection.Message;
using System.Threading;
using CommandLine;
using System.Diagnostics;
using FWI;
using FWI.Prompt;
using System.Net.Sockets;

namespace FWIClient
{
    static public class Program
    {
        static readonly FormatStandardOutputStream formatStandardOutput =  new FormatStandardOutputStream();
        public static FormatStandardOutputStream Out => formatStandardOutput;
        public static readonly string Version = "0.5b";

        static void Main(string[] args)
        {
            Console.Title = "FWIClient";
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        static void Run(Options options)
        {
            Console.WriteLine($"FWI Client");
            Console.WriteLine($"version: {Version}");
            Console.WriteLine($"Connection {options.IP}:{options.Port}");
            Console.WriteLine($"Verbose : {options.Verbose}");

            if (options.Trace) Console.WriteLine($"Mode: Target");
            else Console.WriteLine($"Mode: Observe");

            Console.WriteLine("연결중...");
            var thread = new Thread(() => { RunClient(options); });
            thread.Start();
            thread.Join();
        }

        static void RunClient(Options options)
        {
            var manager = new ClientManager();
            var client = new Client(options.IP, options.Port);
            var runner = new ClientRunner(
                options: options,
                client: client,
                manager: manager
            );
            runner.Run();

            if (Application.MessageLoop == true) Application.Exit();
            else Environment.Exit(1);
        }
    }
}