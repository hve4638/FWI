using FWI;
using FWI.Results;
using FWI.Prompt;
using CommandLine;
using System.Windows.Forms;

namespace FWIClient
{
    static public class Program
    {
        public const string PATH_CONFIG = "config.ini";
        public static readonly string Version = "0.7";
        public static FormController FormControl { get; private set; }
        private static RemoteConsoleControl ConsoleControl { get; set; }
        public static IInputStream StdIn { get; private set; }
        public static IOutputStream StdOut { get; private set; }
        public static IInputStream In { get; private set; }
        public static IOutputStream Out { get; private set; }
        public static IOutputStream VerboseOut
        {
            get {
                if (VerboseMode) return Out;
                else return NullOutputStream.Instance;
            }
        }
        public static bool AutoReload { get; set; }
        public static bool VerboseMode { get; set; }
        public static Prompt? CurrentPrompt { get; set; }
        public static ClientRunner? Runner { get; private set; }
        static Config config;

        public static bool ConsoleConnected() => ConsoleControl.Connected();
        public static bool ConsoleConnected(int id) => ConsoleControl.Connected(id);

        static Program()
        {
            FormControl = new FormController();
            ConsoleControl = new RemoteConsoleControl();

            StdIn = new StandardInputStream();
            StdOut = new FormatOutputStream(new StandardOutputStream());
            In = StdIn;
            Out = StdOut;
            AutoReload = false;

            config = new Config();

            config.serverIP = "127.0.0.1";
            config.serverPort = "7000";
            config.afkTime = "10";
            config.autoReload = true;
            config.openConsoleWhenStartup = false;
            config.debug = false;
        }

        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            var exist = config.Read(PATH_CONFIG);
            if (!exist) config.Write(PATH_CONFIG);

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        public static Config GetLastConfig() => config;
        public static Result<ResultState, string> ChangeConfig(Config config)
        {
            var result = new Result<ResultState, string>(ResultState.Normal);
            if (!int.TryParse(config.afkTime, out _))
            {
                result.State = ResultState.HasProblem;
                result += "잘못된 값: AFK Time";
            }

            if (!int.TryParse(config.serverPort, out _))
            {
                result.State = ResultState.HasProblem;
                result += "잘못된 값: port";
            }
            
            if (result.State == ResultState.Normal)
            {
                Program.config = config;
                config.Write(PATH_CONFIG);
            }

            return result;
        }

        public static void Reset()
        {
            In = StdIn;
            Out = StdOut;
        }

        static private void Run(Options options)
        {
            try
            {
                options.IP = config.serverIP;
                options.Port = int.Parse(config.serverPort);
                options.AutoReload = config.autoReload;
                options.DebugMode = config.debug;
                options.AFK = int.Parse(config.afkTime);
                options.Target = !config.observerMode;
            }
            catch (FormatException)
            {
                MessageBox.Show("Error occur while parsing config.ini");
                Exit();
                return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Error occur while parsing config.ini");
                Exit();
                return;
            }

            if (config.openConsoleWhenStartup)
            {
                var id = OpenConsole();
                while (!ConsoleConnected(id))
                {

                }
                StdOut.WriteLine("RemoteConsole 연결됨");
                Out.WriteLine("연결됨");
            }

            if (options.Target) Out.WriteLine($"Mode: Target");
            else Out.WriteLine($"Mode: Observe");
            Out.Flush();



            VerboseMode = options.Verbose;
            AutoReload = options.AutoReload;

            var form = new MainForm();
            var task = new Task(() => {
                var reload = false;
                do
                {
                    Out.WriteLine("연결중...");
                    Out.Flush();
                    var result = RunClient(options);

                    switch(result)
                    {
                        case RunnerResult.ConnectionFailure:
                            reload = AutoReload;
                            if (reload)
                            {
                                form.ShowTip("연결 실패", "재시도 중...");
                                Thread.Sleep(500);
                            }
                            break;

                        case RunnerResult.Disconnected:
                            reload = AutoReload;
                            break;

                        case RunnerResult.NormalTerminate:
                            reload = false;
                            break;
                    }
                }
                while (reload);

                if (!form.IsDisposed) form.Close();
            });
            task.Start();

            Application.Run(form);
        }

        static private RunnerResult RunClient(Options options)
        {
            var client = new FWIConnection.Client(options.IP, options.Port);
            var manager = new ClientManager(
                client : client,
                debugMode : options.DebugMode
            );

            var runner = new ClientRunner(
                options: options,
                client: client,
                manager: manager
            );
            return runner.Run();
        }

        static public void Exit()
        {
            if (Application.MessageLoop == true) Application.Exit();
            else Environment.Exit(1);
        }

        static public void RunPromptOnRemoteConsole()
        {
            if (CurrentPrompt is null) return;

            if (!ConsoleConnected())
            {
                OpenConsole(
                    onConnect: () =>
                    {
                        try
                        {
                            CurrentPrompt.Loop(In, Out);
                        }
                        catch (RemoteIODisconnectException)
                        {
                            Out.WriteLine("prompt disconnected");
                        }
                    }
                );
            }
        }

        static public int OpenConsole(Action? onConnect = null, Action? onDisconnect = null)
        {
            return ConsoleControl.Open(
                onConnect: (server) =>
                {
                    var remoteIn = new RemoteInputStream(server);
                    var remoteOut = new FormatOutputStream(new RemoteOutputStream(server));
                    remoteIn.ThrowWhenDisconnect = true;
                    In = remoteIn;
                    Out = remoteOut;
                    onConnect?.Invoke();
                },
                onDisconnect: () =>
                {
                    In = StdIn;
                    Out = StdOut;
                    onDisconnect?.Invoke();
                }
            );
        }
    }
}