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
using FWI.Message;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace FWIClient
{
    public class ClientManager
    {
        private readonly Client client;
        public ClientTask Tasker { get; set; }
        public ClientSender Sender { get; private set; }
        readonly ToBeTargetManager toBeTargetManager;
        public bool Closed { get; private set; }
        public bool DebugMode {
            get => Sender.DebugMode;
            set { Sender.DebugMode = value; }
        }

        public ClientManager(Client client, bool debugMode = false)
        {
            this.client = client;
            Sender = new ClientSender(client);
            DebugMode = debugMode;
            toBeTargetManager = new();
            Closed = false;

            Tasker = new(client: client, sender: Sender);
        }
        
        public RequestResult<string> RequestToBeTarget()
        {
            var DeniedResult = (string message) => new RequestResult<string>(RequestResultState.Denied, message);

            if (!Sender.Connected) return DeniedResult("내부 요청 거절 - 연결되지 않음");
            else if (Sender.IsTarget) return DeniedResult("내부 요청 거절 - 중복 요청 (권한 존재)");
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

        public void Close() {
            if (!Closed)
            {
                Closed = true;
                client.Disconnect();
                Tasker.CancelAll();
            }
        }
    }
}
