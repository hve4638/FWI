using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using FWIConnection;
using FWI.Results;
using System.Security.Policy;
using FWI;
using FWIConnection.Message;
using System.Threading;

namespace FWIClient
{
    public class ClientManager
    {
        private readonly Client client;
        private CancellationTokenSource? afkCancelToken;
        private CancellationTokenSource? receiveCancelToken;
        public ClientSender Sender { get; private set; }
        readonly ToBeTargetManager toBeTargetManager;
        public bool IsAFK
        {
            get => Sender.IsAFK;
            set { Sender.IsAFK = value; }
        }
        public bool DebugMode {
            get => Sender.DebugMode;
            set { Sender.DebugMode = value; }
        }

        public ClientManager(Client client, bool debugMode = false)
        {
            this.client = client;
            Sender = new ClientSender(client);
            DebugMode = debugMode;
            IsAFK = false;
            toBeTargetManager = new();
        }

        public RequestResult<string> RequestToBeTarget()
        {
            var DeniedResult = (string message) => new RequestResult<string>(RequestResultState.Denied, message);

            if (!Sender.Connected) return DeniedResult("내부 요청 거절 - 연결되지 않음");
            else if (Sender.IsTarget) return DeniedResult("내부 요청 거절 - 권한이 존재");
            else
            {
                toBeTargetManager.Reset();
                toBeTargetManager.Request();

                var message = new RequestToBeTargetMessage()
                {
                    Id = toBeTargetManager.Id,
                };
                Sender.Send(message);
            }

            return toBeTargetManager.Result;
        }

        public Result<ResultState, string> ResponseToBeTarget(short nonce, bool accepted)
        {
            var results = new Result<ResultState, string>();

            var manager = toBeTargetManager;
            if (manager.Progress != ToBeTargetState.Requested) results += "요청되지 않음";
            else if (manager.Id != nonce) results += "잘못된 ID";
            else if (accepted)
            {
                results.State |= ResultState.Normal;
                Sender.IsTarget = true;

                manager.Accept("Request Accepted");
            }
            else
            {
                results.State |= ResultState.HasProblem;
                results += "요청 거부됨";

                manager.Deny("Request Denied");
            }

            return results;
        }

        public Task ReceiveFromServerAsync()
        {
            var cancelToken = new CancellationTokenSource();

            receiveCancelToken?.Cancel();
            receiveCancelToken = cancelToken;

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

        public Task CheckAFKAsync(uint afkTime)
        {
            var cancelToken = new CancellationTokenSource();

            afkCancelToken?.Cancel();
            afkCancelToken = cancelToken;

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
                    else if (!IsAFK && current >= afkTime)
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
    }
}
