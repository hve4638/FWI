using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FWI.Prompt;
using FWIConnection;
using FWIConnection.Message;

namespace FWIClient
{
    public class ClientRunner
    {
        readonly ClientManager manager;
        readonly Client client;
        readonly bool targetMode;
        readonly uint afkTime;
        Threads threads;

        public ClientRunner(Options options, Client client, ClientManager manager)
        {
            threads = new Threads();
            targetMode = options.Target;
            afkTime = (uint)(options.AFK * 60);
            this.manager = manager;
            this.client = client;
        }

        public void Run()
        {
            threads.Clear();
            client.SetReceiver(new Receiver(manager: manager));

            var prompt = new Prompt();
            InitializePrompt(prompt);
            Program.CurrentPrompt = prompt;

            var connectResult = TryConnect(5);
            if (connectResult)
            {
                Program.Out.WriteLine($"[D][I] Connect: {client.IP}:{client.Port}");
                Program.Out.Flush();

                manager.CheckAFKAsync(afkTime);
                
                Program.RunPromptOnRemoteConsole();

                if (targetMode) prompt.Execute("rt");
                try
                {
                    client.ReceiveProgress();
                }
                finally
                {
                    threads.Interrupt();
                }
            }
            else
            {
                Program.Out.WriteLine("[D][I] 연결에 실패했습니다");
            }
            Program.Out.Flush();
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
            var thread = new Thread(
                () => TraceForegroundWindow.Trace(
                    onTrace: (wi) =>
                    {
                        if (manager.IsAFK) return;

                        manager.Sender.SendWI(wi);
                    },
                    traceInterval: interval
                )
            );

            manager.RequestToBeTarget()
                .WithAccepted((str) =>
                {
                    Program.Out.WriteLine("[D][I] Target 지정됨");
                    thread.Start();
                })
                .WithDenied((str) => {
                    Program.Out.WriteLine("[D][I] Target 요청 실패");
                    Program.Out.WriteLine($" 사유 : {str}");
                });

            threads += thread;
        }

        public void InitializePrompt(Prompt prompt)
        {
            var promptInitializer = new PromptInitializer(client, manager);
            promptInitializer.Init(prompt);
            prompt.DefaultOutputStream = Program.Out;

            prompt.Add("rt", (args, output) =>
            {
                output.WriteLine("[D][I] Target 요청");
                RequestToBeTarget();
            });
        }

        public void RunAFKChecker()
        {
            var thread = new Thread(() => {
                bool afk = false;
                int noAFKTime = 0;
                int sleepTime = 750;

                while (true)
                {
                    var current = AFKChecker.GetLastInputTime();

                    if (afk)
                    {
                        if (current == 0) noAFKTime += sleepTime;
                        else noAFKTime = 0;

                        if (noAFKTime >= 2000)
                        {
                            Program.Out.WriteLine($"[D][I] No longer AFK");
                            manager.Sender.SendNoAFK(DateTime.Now);
                            afk = false;
                        }
                    }
                    else if (!manager.IsAFK && current >= afkTime)
                    {
                        Program.Out.WriteLine($"[D][I] Now AFK");
                        var now = DateTime.Now;
                        var from = now - new TimeSpan(0, 0, (int)afkTime);
                        manager.Sender.SendAFK(from, now);
                        afk = true;
                        noAFKTime = 0;
                    }
                    Thread.Sleep(sleepTime);
                }
            });
            thread.Start();

            threads += thread;
        }

        public void RunReceiver()
        {

        }
    }
}
