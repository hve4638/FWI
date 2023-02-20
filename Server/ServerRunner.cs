using FWI;
using FWI.Prompt;
using FWIConnection;

namespace FWIServer
{
    class ServerRunner
    {
        readonly FWIManager fwiManager;
        readonly ServerManager serverManager;
        readonly Prompt prompt;
        readonly Server server;
        readonly LinkedList<Receiver> sessionList;
        readonly Dictionary<string, string> pathDict;
        readonly int interval;
        readonly int port;
        readonly bool verbose;
        readonly string signiture;

        public ServerRunner(Options options, Dictionary<string, string> pathDict)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FWI";
            
            Directory.CreateDirectory(path);

#pragma warning disable CS8629
            port = (int)options.Port;
            interval = (int)options.Interval;
            verbose = (bool)options.Verbose;
            signiture = options.Signiture!;
#pragma warning restore CS8629

            fwiManager = new(signiture: signiture);
            serverManager = new(manager: fwiManager);
            sessionList = new();
            prompt = new();
            server = new(port: port)
            {
                Verbose = verbose
            };
            this.pathDict = pathDict;
        }

        public void Run()
        {
            InitializeManager();
            RunPrompt();
            RunChecker();

            Thread thread = server.RunAsync();
            thread.Join();
        }

        public void InitializeManager()
        {
            fwiManager.SetPath(pathDict);
            fwiManager.LoadFilter();

            server.SetReceiverGetter(() =>
            {
                var receiver = new Receiver(
                    manager: serverManager,
                    prompt: prompt,
                    onConnect: (receiver) => sessionList.AddLast(receiver),
                    onDisconnect: (receiver) => sessionList.Remove(receiver)
                );
                receiver.SetOnVerbose(() => server.Verbose);
                return receiver;
            });

            fwiManager.SetLoggingInterval(interval);
            fwiManager.SetOnLoggingListener((WindowInfo item) =>
            {
                if (server.Verbose)
                {
                    Program.Out.WriteLine($"[D][A] Log Added: {item.Name.PadCenter(25)} | {item.Title.Truncate(40)}");
                }
            });
        }

        public Task RunPrompt()
        {
            var promptInitializer = new PromptInitializer(
                fwiManager: fwiManager,
                serverManager : serverManager,
                sessions: sessionList
            );;
            promptInitializer.Initialize(prompt);

            prompt.Add("verbose", (args, output) => {
                var arg = args.GetArg(0, "").ToLower();
                switch (arg)
                {
                    case "T":
                    case "TRUE":
                    case "1":
                        server.Verbose = true;
                        Program.VerboseMode = true;
                        break;

                    case "F":
                    case "FALSE":
                    case "0":
                        server.Verbose = false;
                        Program.VerboseMode = false;
                        break;

                    case "":
                        break;
                }

                output.WriteLine($"Current verbose mode: {server.Verbose}");

            });
            prompt.DefaultOutputStream = Program.Out;

            return prompt.LoopAsync(Program.In, Program.Out);
        }

        public void RunChecker()
        {
            var alertChecker = new IntervalThread(
                () => {
                    if (!serverManager.HasTarget)
                    {
                        Program.Out.WriteLine("[D][A] 현재 Target 클라이언트가 없습니다.");
                    }
                }
            );
            alertChecker.Start(new TimeSpan(0,5,0), 5000);
        }
    }
}
