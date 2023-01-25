using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using FWIConnection;
using FWI.Results;
using System.Security.Policy;
using FWIConnection.Message;
using FWI;

namespace FWIClient
{
    internal class ClientManager
    {
        readonly Client client;
        readonly ToBeTargetManager toBeTargetManager;
        public bool Connected { get; set; }
        public bool IsTarget { get; set; }
        public bool IsAFK { get; private set; }

        public ClientManager(Client client)
        {
            this.client = client;
            toBeTargetManager = new();
            Connected = false;
            IsTarget = false;
            IsAFK = false;
        }

        public RequestResult<string> RequestToBeTarget()
        {
            var DeniedResult = (string message) => new RequestResult<string>(RequestResultState.Denied, message);

            if (!Connected) return DeniedResult("내부 요청 거절 - 연결되지 않음");
            else if (IsTarget) return DeniedResult("내부 요청 거절 - 권한이 존재");
            else
            {
                toBeTargetManager.Reset();
                toBeTargetManager.Request();

                var bw = new ByteWriter();
                bw.Write((short)MessageOp.RequestToBeTarget);
                bw.Write(toBeTargetManager.Id);
                client.Send(bw.ToBytes());
            }

            return toBeTargetManager.Result;
        }

        public Results<string> ResponseToBeTarget(short nonce, bool accepted)
        {
            var results = new Results<string>();

            var manager = toBeTargetManager;
            if (manager.Progress != ToBeTargetState.Requested)
            {
                results.State = ResultState.None;
                results += "요청되지 않음";
            }
            else if (manager.Id != nonce)
            {
                results.State = ResultState.None;
                results += "잘못된 ID";
            }
            else if (accepted)
            {
                results.State = ResultState.Normal;
                IsTarget = true;

                manager.Accept("Request Accepted");
            }
            else
            {
                results.State = ResultState.HasProblem;
                results += "요청 거부됨";

                manager.Deny("Request Denied");
            }

            return results;
        }

        public void SendWI(WindowInfo wi)
        {
            if (!Connected) return;
            else if (!IsTarget) return;
            else
            {
                IsAFK = false;

                var bw = new ByteWriter();
                bw.Write((short)MessageOp.UpdateCurrentWI);
                bw.WriteWI(name: wi.Name, title: wi.Title, date: wi.Date);

                client.Send(bw.ToBytes());
            }
        }

        public void SendAFK(DateTime date)
        {
            if (!Connected) return;
            else if (!IsTarget) return;
            else if (IsAFK) return;
            else
            {
                var bw = new ByteWriter();
                bw.Write((short)MessageOp.SetAFK);
                bw.WriteDateTime(date);

                client.Send(bw.ToBytes());
            }
        }

        public void SendNoAFK(DateTime date)
        {
            if (!Connected) return;
            else if (!IsTarget) return;
            else if (!IsAFK) return;
            else
            {
                var bw = new ByteWriter();
                bw.Write((short)MessageOp.SetNoAFK);
                bw.WriteDateTime(date);

                client.Send(bw.ToBytes());
            }
        }
    }
}
