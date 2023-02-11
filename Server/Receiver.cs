using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;
using FWIConnection.Message;
using FWI;
using FWI.Prompt;
using System.Xml.Linq;
using YamlDotNet.Core.Tokens;
using FWI.Results;

namespace FWIServer
{
    internal class Receiver : IReceiver
    {
        ServerManager manager;
        Prompt prompt;
        IPEndPoint? client;
        Socket? socket;
        Func<bool>? onVerbose;
        Action<Receiver>? onConnect;
        Action<Receiver>? onDisconnect;
        
        public bool IsTarget { get; private set; }

        public Receiver(ServerManager manager, Prompt prompt, Action<Receiver>? onConnect = null, Action<Receiver>? onDisconnect = null)
        {
            this.manager = manager;
            this.prompt = prompt;
            client = null;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
        }

        public ReceiverInfo Info()
        {
            return new ReceiverInfo
            {
                Addr = client?.Address.ToString() ?? "N/A",
                Port = client?.Port ?? 0,
                Target = IsTarget,
            };
        }

        public void SetOnVerbose(Func<bool> onVerbose)
        {
            this.onVerbose = onVerbose;
        }

        public void Connect(Socket socket)
        {
            client = socket.RemoteEndPoint as IPEndPoint;
            this.socket = socket;

            Program.Out.WriteLine($"[D][I] Connect: {ClientName}");
            onConnect?.Invoke(this);
        }
        public void Disconnect()
        {
            if (IsTarget)
            {
                IsTarget = false;
                manager.ResetTarget(DateTime.Now);
                Out.WriteLine($"[D][A] Target Client 가 지정 해제되었습니다 : {ClientName}");
            }
            
            Out.WriteLine($"[D][I] Disconnect: {ClientName}");
            onDisconnect?.Invoke(this);
        }
        public void Receive(in byte[] buf, int size)
        {
            var br = new ByteReader(buf, size);
            var op = (MessageOp)br.PeekShort();

            try
            {
                switch (op)
                {
                    case MessageOp.UpdateWI:
                        ResponseUpdateWI(br);
                        break;
                    case MessageOp.SetAFK:
                        ResponseAFK(br);
                        break;
                    case MessageOp.SetNoAFK:
                        ResponseNoAFK(br);
                        break;
                    case MessageOp.Message:
                        ResponseMessage(br);
                        break;
                    case MessageOp.Echo:
                        ResponseEcho(br);
                        break;
                    case MessageOp.RequestTimeline:
                        ResponseTimeline();
                        break;
                    case MessageOp.RequestRank:
                        ResponseRank();
                        break;
                    case MessageOp.RequestToBeTarget:
                        ResponseToBeTarget(br);
                        break;
                    case MessageOp.ServerCall:
                        ResponseServerCall(br);
                        break;
                    default:
                    case MessageOp.UpdateCurrentWI:
                    case MessageOp.RequestPrivillegeTrace:
                        ResponseOther(br, op);
                        break;
                }
            }
            catch(DeserializeFailException e)
            {
                Out.WriteLine($"[D][W] Receiver에서 Deserialize에 실패했습니다. (OP: {op})");
                Out.WriteLine($"----------------------");
                Out.WriteLine($"");
                Out.WriteLine(e.ToString());
                Out.WriteLine(e.StackTrace);
                Out.WriteLine($"----------------------");
            }
            catch(ThreadInterruptedException e)
            {
                throw e;
            }
            catch(Exception e)
            {
                Out.WriteLine($"[D][W] Receiver에서 예외가 발생했습니다");
                Out.WriteLine($"----------------------");
                Out.WriteLine(e.ToString());
                Out.WriteLine(e.StackTrace);
                Out.WriteLine($"----------------------");
            }
        }

        void ResponseUpdateWI(in ByteReader br)
        {
            if (IsTarget)
            {
                var message = UpdateWIMessage.Deserialize(br);
                var name = message.Name;
                var title = message.Title;
                var date = message.Date;

                var wi = new WindowInfo(name: name, title: title, date: date);
                var results = manager.AddWI(wi);

                foreach(var result in results)
                {
                    switch(result.State)
                    {
                        case ServerResultState.Normal:
                            VerboseOut.WriteLine($"[D][I][{ClientName}] UpdateWI : {wi.Date:yyMMdd HH:mm:ss}  |  {wi.Name.PadCenter(28)}  |\t{wi.Title.Truncate(40)}");
                            break;
                        case ServerResultState.FatalIssue:
                        case ServerResultState.NonFatalIssue:
                            VerboseOut.WriteLine($"[D][I][{ClientName}] UpdateWI 실패 {wi.Name}");
                            break;
                    }

                    foreach (var item in result) VerboseOut.WriteLine($"  {item}");
                }
            }
            else
            {
                VerboseOut.WriteLine($"[D][I][{ClientName}] Non-Target Client의 요청: UpdateWI (무시)");
            }
        }

        void ResponseAFK(in ByteReader br)
        {
            if (IsTarget)
            {
                var message = AFKMessage.Deserialize(br);
                var date = message.FromDate;
                var results = manager.SetAFK(date);

                results.Parse()
                    .With(ServerResultState.ChangeAFK, (result) =>
                    {
                        Out.WriteLine($"[D][A] Target Client가 AFK 상태입니다. (From {date})");
                        foreach (var item in result) VerboseOut.WriteLine($"  {item}");
                    })
                    .With(ServerResultState.NonFatalIssue, (result) =>
                    {
                        Out.WriteLine($"[D][A] ResponseAFK 처리 중 Non-Fatal Issue가 발생했습니다.");
                        foreach (var item in result) Out.WriteLine($"  {item}");
                    })
                    .With(ServerResultState.FatalIssue, (result) =>
                    {
                        Out.WriteLine($"[D][W] ResponseAFK 처리 중 심각한 문제를 발견했습니다.");
                        foreach (var item in result) Out.WriteLine($"  {item}");
                    });
            }
            else
            {
                VerboseOut.WriteLine($"[D][I][{ClientName}] Non-Target Client의 요청: RequestAFK (무시)");
            }
        }

        void ResponseNoAFK(in ByteReader br)
        {
            if (IsTarget)
            {
                var message = NoAFKMessage.Deserialize(br);
                var date = message.FromDate;

                var results = manager.SetNoAFK(date);
                results.Parse()
                    .With(ServerResultState.Normal, (result) =>
                    {
                        Out.WriteLine($"[D][A] Target Client가 AFK 상태가 아닙니다. (To {date})");
                        foreach (var detail in result) VerboseOut.WriteLine($"  {detail}");
                    })
                    .With(ServerResultState.NonFatalIssue, (result) =>
                    {
                        Out.WriteLine($"[D][A] ResponseNoAFK 처리 중 문제가 발생했습니다.");
                        foreach (var detail in result) Out.WriteLine($"  {detail}");
                    })
                    .With(ServerResultState.FatalIssue, (result) =>
                    {
                        Out.WriteLine($"[D][W] NoAFK 처리 중 심각한 문제가 발생했습니다.");
                        foreach (var detail in result) Out.WriteLine($"  {detail}");
                    })
                    .With(ServerResultState.Info, (result) =>
                    {
                        foreach (var detail in result) Out.WriteLine($"  {detail}");
                    });
            }
            else
            {
                VerboseOut.WriteLine($"[D][I][{ClientName}] Non-Target Client의 요청: SetNoAFK (무시)");
            }
        }

        void ResponseMessage(in ByteReader br)
        {
            var message = TextMessage.Deserialize(br);
            var text = message.Text;

            Out.WriteLine($"{ClientName}> {text}");
        }

        void ResponseEcho(in ByteReader br)
        {
            var message = EchoMessage.Deserialize(br);
            var text = message.Text;

            SendMessage(text);
        }

        void ResponseServerCall(in ByteReader br)
        {
            var message = ServerCallMessage.Deserialize(br);
            var cmd = message.Command;
            VerboseOut.WriteLine($"[D][I][{ClientName}] SeverCall 요청 > {cmd}");
            
            prompt.Execute(cmd, new SocketOutputStream(socket!));
        }

        void ResponseTimeline()
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 요청 : Timeline");

            var text = manager.GetTimelineAsString();
            SendMessage(text);
        }

        void ResponseRank()
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 요청 : Rank");

            var text = manager.GetRankAsString();
            SendMessage(text);
        }

        void ResponseToBeTarget(ByteReader br)
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 요청 : ToBeTarget");

            bool accepted;
            var receiveMessage = RequestToBeTargetMessage.Deserialize(br);
            var nonce = receiveMessage.Id;

            if (!manager.HasTarget)
            {
                IsTarget = true;
                manager.SetTarget();
                accepted = true;

                Out.WriteLine($"[D][A] Target Client가 지정되었습니다 : {ClientName}");
            }
            else
            {
                accepted = false;
            }

            var message = new ResponseToBeTargetMessage()
            {
                Id = nonce,
                Accepted = accepted
            };

            Send(message);
        }

        void ResponseOther(in ByteReader br, MessageOp op)
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 처리할 수 없는 요청: {op}");
        }

        public void SendMessage(string text)
        {
            var message = new TextMessage()
            {
                Text = text,
            };

            Send(message);
        }

        public void Send(ISerializableMessage serializableMessage)
        {
            var bytes = serializableMessage.Serialize();

            socket!.Send(bytes);
        }

        string ClientName => $"{client?.Address}:{client?.Port}";

        static IOutputStream Out => Program.Out;
        IOutputStream VerboseOut
        {
            get
            {
                var verbose = onVerbose?.Invoke() ?? false;
                if (verbose) return Out;
                else return NullOutputStream.Instance;
            }
        }
    }

    public class ReceiverInfo
    {
        public string? Addr { get; set; }
        public int Port { get; set; }
        public bool Target { get; set; }
        public bool CanTrace => Target;
    }
}