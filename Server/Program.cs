using System.Text;
using FWIConnection;
using FWI;
using System.Diagnostics;
using CommandLine;
using System.Net;
using System.Net.Sockets;

namespace FWIServer
{
    static class Program
    {
        static readonly string Version = "0.1";

        static void Main(string[] args)
        {
            Console.WriteLine($"FWI Server");
            Console.WriteLine($"version: {Version}");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunServer);
        }

        static void RunServer(Options option)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FWI";
            Directory.CreateDirectory(path);

            var signiture = path + @"\" + option.Signiture;
            LinkedList<Receiver> list = new();
            FWIManager manager = new(signiture);
            Server server = new(port: option.Port)
            {
                Verbose = option.Verbose
            };

            server.SetReceiverGetter(() =>
            {
                var receiver = new Receiver(manager,
                    onConnect: (receiver) => list.AddLast(receiver),
                    onDisconnect: (receiver) => list.Remove(receiver)
                );
                receiver.SetOnVerbose(() => server.Verbose);
                return receiver;
            });

            manager.SetLoggingInterval(5);
            manager.SetOnLoggingListener((WindowInfo item) =>
            {
                if (server.Verbose)
                {
                    Console.WriteLine($"Log Added: {item}");
                    Console.Out.Flush();
                }
            });

            Thread thread = server.RunAsync();
            Prompt prompt = new();
            
            prompt.Add("verbose", (args) => {
                server.Verbose = !server.Verbose;
                Console.WriteLine($"Verbose mode: {server.Verbose}");
            });
            manager.AppendPromptCommand(prompt);
            manager.LoadFilter(manager.Signiture);

            prompt.RunAsync();
            thread.Join();
        }
    }
}