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
        public static readonly string Version = "0.5d dev 3";
        static readonly FormatStandardOutputStream formatStandardOutput =  new FormatStandardOutputStream();
        public static bool VerboseMode { get; set; }
        
        static void Main(string[] args)
        {
            Console.Title = "FWIClient";
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        public static FormatStandardOutputStream Out => formatStandardOutput;
        public static IOutputStream VerboseOut
        {
            get {
                if (VerboseMode) return formatStandardOutput;
                else return formatStandardOutput;
            }
        }

        static void Run(Options options)
        {
            Console.WriteLine($"FWI Client");
            Console.WriteLine($"version: {Version}");
            Console.WriteLine($"Connection {options.IP}:{options.Port}");
            Console.WriteLine($"Verbose : {options.Verbose}");
            Console.WriteLine($"AFK Time : {options.AFK}min");

            if (options.Target) Console.WriteLine($"Mode: Target");
            else Console.WriteLine($"Mode: Observe");

            VerboseMode = options.Verbose;

            Console.WriteLine("연결중...");
            var thread = new Thread(() => { RunClient(options); });
            thread.Start();
            thread.Join();
        }

        static void RunClient(Options options)
        {
            var client = new Client(options.IP, options.Port);
            var manager = new ClientManager(client);
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