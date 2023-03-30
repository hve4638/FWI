using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FWI.Commands;
using FWIConnection;
using FWI.Message;

namespace FWIClient
{
    public class ClientRunner
    {
        readonly ClientManager manager;
        readonly Client client;
        readonly bool targetMode;
        readonly uint afkTime;

        public ClientRunner(Options options, Client client, ClientManager manager)
        {
            targetMode = options.Target;
            afkTime = (uint)(options.AFK * 60);
            this.manager = manager;
            this.client = client;
        }

        public RunnerResult Run()
        {
            RunnerResult? result = null;
            client.SetReceiver(new Receiver(manager: manager));
            InitializePrompt();

            bool wait = true;

            var task = new Task(() =>
            {
                Program.Out.WriteLine($"[D][I] 연결 시도...");
                var connectResult = TryConnect(5);
                if (connectResult)
                {
                    Program.Out.WriteLine($"[D][A] 연결됨 - {client.IP}:{client.Port}");

                    Program.CurrentForm?.SetTitle($"Connected [{client.IP}:{client.Port}]");

                    manager.Tasker.CheckAFK(afkTime);
                    manager.Tasker.BeginReceive();
                    manager.Tasker.CheckConnected(
                        onDisconnect: () => {
                            Program.Out.WriteLine("[D][A] 연결이 종료되었습니다");
                            Program.CurrentForm?.SetTitle($"Disconnected");

                            result = RunnerResult.Disconnected;
                            wait = false;
                        }
                    );

                    if (targetMode) RequestToBeTarget();

                    if (Program.ConsoleConnected())
                    {
                        Program.RunPromptOnRemoteConsole();
                    }
                }
                else
                {
                    Program.Out.WriteLine("[D][A] 연결에 실패했습니다");
                    Program.CurrentForm?.SetTitle($"Disconnected");

                    result = RunnerResult.ConnectionFailure;
                    wait = false;
                }
                Program.Out.Flush();
            });
            task.Start();

            while (wait) Thread.Sleep(300);
            manager.Close();

            return result ?? RunnerResult.NormalTerminate;
        }

        public bool TryConnect(int tryCount = 1)
        {
            var connected = false;
            var count = 0;
            while (!connected && count <= tryCount) connected = client.Connect();
            
            return connected;
        }

        public void RequestToBeTarget(int interval = 1000)
        {
            manager.RequestToBeTarget()
                .WithAccepted((str) =>
                {
                    Program.Out.WriteLine("[D][I] Target 지정됨");
                    manager.Tasker.TrackingFWI(interval);
                })
                .WithDenied((str) => {
                    Program.Out.WriteLine("[D][I] Target 요청 실패");
                    Program.Out.WriteLine($" 사유 : {str}");
                });
        }
        
        public void InitializePrompt()
        {
            var prompt = new Command();
            var promptInitializer = new PromptInitializer(client, manager);
            promptInitializer.Init(prompt);
            prompt.DefaultOutputStream = Program.Out;
            Program.CurrentPrompt = prompt;

            prompt.Add("request target", (args, output) =>
            {
                output.WriteLine("[D][I] Target 요청");
                RequestToBeTarget();
            });
        }
    }
}
