using FWIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public class ClientTask
    {
        readonly CancellationTokenManager tokens;
        readonly Client client;
        ClientSender Sender { get; set; }

        public ClientTask(Client client, ClientSender sender)
        {
            tokens = new CancellationTokenManager();
            this.client = client;
            Sender = sender;
        }
        public void CancelAll() => tokens.Clear();

        public Task BeginReceive()
        {
            var cancelToken = tokens.MakeNewToken("begin-receive");

            var task = new Task(() =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    client.WaitForReceive();
                }
            });
            task.Start();

            return task;
        }

        public Task TrackingFWI(int interval)
        {
            var cancelToken = tokens.MakeNewToken("tracking-fwi");

            var task = new Task(() => {
                var tracker = new FWITracker();
                while (!cancelToken.IsCancellationRequested)
                {
                    var wi = tracker.Track();
                    if (!Sender.IsAFK)
                    {
                        Sender.SendWI(wi);
                    }

                    Thread.Sleep(interval);
                }
            });
            task.Start();

            return task;
        }

        public Task CheckAFK(uint afkTime)
        {
            var cancelToken = tokens.MakeNewToken("check-afk");

            var task = new Task(() => {
                bool afk = false;
                int noAFKTime = 0;
                int sleepTime = 750;

                while (!cancelToken.IsCancellationRequested)
                {
                    var current = AFKChecker.GetLastInputTime();

                    if (afk)
                    {
                        if (current == 0) noAFKTime += sleepTime;
                        else noAFKTime = 0;

                        if (noAFKTime >= 2000)
                        {
                            Program.Out.WriteLine($"[D][I] No longer AFK");
                            Sender.SendNoAFK(DateTime.Now);
                            afk = false;
                        }
                    }
                    else if (!Sender.IsAFK && current >= afkTime)
                    {
                        Program.Out.WriteLine($"[D][I] Now AFK");
                        var now = DateTime.Now;
                        var from = now - new TimeSpan(0, 0, (int)afkTime);
                        Sender.SendAFK(from, now);
                        afk = true;
                        noAFKTime = 0;
                    }
                    Thread.Sleep(sleepTime);
                }
            }, cancelToken.Token);
            task.Start();

            return task;
        }

        public Task CheckConnected(Action onDisconnect)
        {
            var cancelToken = tokens.MakeNewToken("check-connected");

            var task = new Task(() => {
                while (!cancelToken.IsCancellationRequested)
                {
                    if (!client.Connected)
                    {
                        onDisconnect();
                        break;
                    }

                    Thread.Sleep(100);
                }
            });
            task.Start();

            return task;
        }
    }
}
