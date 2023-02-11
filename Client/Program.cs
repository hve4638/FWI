using System.Text;
using FWIConnection;
using FWIConnection.Message;
using System.Threading;
using CommandLine;
using System.Diagnostics;
using FWI;
using FWI.Prompt;
using System.Net.Sockets;
using System.Reflection;

namespace FWIClient
{
    static public class Program
    {
        public static Prompt? CurrentPrompt { get; set; }
        public static readonly string Version = "0.6e";
        public static readonly StandardInputStream stdIn = new();
        public static readonly FormatStandardOutputStream stdOut = new();
        public static bool VerboseMode { get; set; }
        public static IOutputStream Out { get; private set; }
        public static IInputStream In { get; private set; }
        public static IOutputStream VerboseOut
        {
            get {
                if (VerboseMode) return Out;
                else return NullOutputStream.Instance;
            }
        }
        public static ClientRunner? Runner { get; private set; }

        static Program()
        {
            Version = "0.6e";
            Out = stdOut;
            In = stdIn;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "FWIClient";
            Console.WriteLine($"FWI Client");
            Console.WriteLine($"version: {Version}");
            //ApplicationConfiguration.Initialize();
            //Application.Run(new MainForm());

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        static public void RunPromptOnRemoteConsole()
        {
            if (CurrentPrompt is null) return;
            OpenRemoteConsole();

            Task task = new(() => {
                try
                {
                    CurrentPrompt.Loop(In, Out);
                }
                catch (RemoteIODisconnectException)
                {
                    Out.WriteLine("prompt disconnected");
                }
            });
            task.Start();
        }

        static public void OpenRemoteConsole()
        {
            RemoteConsole.Open(7010);
            var server = new SimpleConnection.Server(7010);
            server.Accept();

            var onDisconnect = () => {
                In = stdIn;
                Out = stdOut;
            };

            var remoteIn = new RemoteInputStream(server);
            var remoteOut = new RemoteOutputStream(server);
            remoteIn.OnDisconnect = onDisconnect;
            remoteOut.OnDisconnect = onDisconnect;

            remoteIn.ThrowWhenDisconnect = true;

            In = remoteIn;
            Out = remoteOut;
        }

        static private void Run(Options options)
        {
            Out.WriteLine($"Connection {options.IP}:{options.Port}");
            Out.WriteLine($"Verbose : {options.Verbose}");
            Out.WriteLine($"AFK Time : {options.AFK}min");

            if (options.Target) Out.WriteLine($"Mode: Target");
            else Out.WriteLine($"Mode: Observe");
            Out.Flush();

            VerboseMode = options.Verbose;

            Out.WriteLine("연결중...");
            Out.Flush();
            var task = new Task(() => { RunClient(options); });
            task.Start();
            task.Wait();
        }

        static private void RunClient(Options options)
        {
            var client = new Client(options.IP, options.Port);
            var manager = new ClientManager(
                client : client,
                debugMode : options.DebugMode
            );
            var runner = new ClientRunner(
                options: options,
                client: client,
                manager: manager
            );
            runner.Run();

            Exit();
        }

        static public void Exit()
        {
            if (Application.MessageLoop == true) Application.Exit();
            else Environment.Exit(1);
        }
    }
}