using System;
using System.Collections.Generic;
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
    internal class ClientRunner
    {
        readonly ClientManager manager;
        readonly Client client;
        readonly bool tryTraced;
        readonly uint afkTime;
        Threads threads;

        public ClientRunner(Options options, Client client, ClientManager manager)
        {
            threads = new Threads();
            tryTraced = options.Trace;
            afkTime = 5 * 60;
            this.manager = manager;
            this.client = client;
        }

        public void Run()
        {
            threads.Clear();
            client.SetReceiver(new Receiver(manager: manager));

            if (TryConnect(5))
            {
                Program.Out.WriteLine($"[D][I] Connect: {client.IP}:{client.Port}");

                Thread? traceThread = null;
                if (tryTraced) RequestToBeTarget();
                RunAFKChecker();

                RunPrompt();
                try
                {
                    client.ReceiveProgress();
                }
                finally
                {
                    traceThread?.Interrupt();
                    threads.Interrupt();
                }
            }
            else
            {
                Program.Out.WriteLine("[D][I] 연결에 실패했습니다");
            }
        }

        public bool TryConnect(int tryCount = 1)
        {
            var connected = false;
            var count = 0;
            while (!connected && count <= tryCount)
            {
                connected = client.Connect();
            }
            
            return connected;
        }

        public void RequestToBeTarget(int interval = 1000)
        {
            var thread = new Thread(
                () => TraceForegroundWindow.Trace(
                    onTrace: (wi) =>
                    {
                        var bw = new ByteWriter();
                        bw.Write((short)MessageOp.UpdateCurrentWI);
                        bw.WriteWI(name: wi.Name, title: wi.Title, date: wi.Date);
                        
                        client.Send(bw.ToBytes());
                    },
                    traceInterval: interval
                )
            );

            manager.RequestPrivillegeTrace()
                .WithAccepted(() =>
                {
                    thread.Start();
                });

            threads += thread;
        }

        public void RunPrompt()
        {
            var prompt = new Prompt();
            var promptInitializer = new PromptInitializer(client, manager);
            promptInitializer.Init(prompt);

            threads += prompt.LoopAsync();
        }

        public void RunAFKChecker()
        {
            var thread = new Thread(() => {
                bool afk = false;
                while (true)
                {
                    var current = AFKChecker.GetLastInputTime();

                    if (!afk && current >= afkTime)
                    {
                        Program.Out.WriteLine($"[D][I] Now AFK");
                        manager.SendAFK();
                        afk = true;
                    }
                    else if (afk && current < afkTime)
                    {
                        Program.Out.WriteLine($"[D][I] No longer AFK");
                        manager.SendNoAFK();
                        afk = false;
                    }
                }
            });
            thread.Start();

            threads += thread;
        }
    }
}
