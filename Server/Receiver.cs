﻿using System;
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
            var op = (MessageOp)br.ReadShort();
            
            switch (op)
            {
                case MessageOp.UpdateWI:
                case MessageOp.UpdateCurrentWI:
                    ResponseUpdateWI(br);
                    break;
                case MessageOp.SetAFK:
                    ResponseAFK(br);
                    break;
                case MessageOp.SetNoAFK:
                    ResponseNoAFK(br);
                    break;
                case MessageOp.Message:
                    Message(br);
                    break;
                case MessageOp.Echo:
                    Send(br);
                    break;
                case MessageOp.RequestTimeline:
                    ResponseTimeline();
                    break;
                case MessageOp.RequestRank:
                    ResponseRank();
                    break;
                case MessageOp.RequestToBeTarget:
                case MessageOp.RequestPrivillegeTrace:
                    ResponseTracePrivillege(br);
                    break;
                case MessageOp.ServerCall:
                    ResponseServerCall(br);
                    break;
                default:
                    ResponseOther(br, op);
                    break;
            }
        }

        void ResponseAFK(in ByteReader br)
        {
            if (IsTarget)
            {
                br.ReadDateTime(out DateTime date);
                Out.WriteLine($"[D][A] Target Client가 AFK 상태입니다. (From {date})");

                manager.SetAFK(date);
            }
            else
            {
                VerboseOut.WriteLine($"[D][I][{ClientName}] Non-Target Client의 요청: UpdateWI (무시)");
            }
        }

        void ResponseNoAFK(in ByteReader br)
        {
            if (IsTarget)
            {
                br.ReadDateTime(out DateTime date);
                Out.WriteLine($"[D][A] Target Client가 AFK 상태가 아닙니다. (To {date})");

                manager.SetNoAFK(date);
            }
            else
            {
                VerboseOut.WriteLine($"[D][I][{ClientName}] Non-Target Client의 요청: SetNoAFK (무시)");
            }
        }

        void ResponseServerCall(in ByteReader br)
        {
            var cmd = br.ReadString();
            VerboseOut.WriteLine($"[D][A][{ClientName}] SeverCall 요청 > {cmd}");
            
            prompt.Execute(cmd, new SocketOutputStream(socket!));
        }

        void ResponseUpdateWI(in ByteReader br)
        {
            if (IsTarget)
            {
                br.ReadWI(name: out string name, title: out string title, date: out DateTime date);

                var wi = new WindowInfo(name: name, title: title, date: date);
                manager.AddWI(wi);

                VerboseOut.WriteLine($"[D][I][{ClientName}] UpdateWI : {wi.Date}\t|\t{wi.Name}\t|\t{wi.Title}");
            }
            else
            {
                VerboseOut.WriteLine($"[D][I][{ClientName}] Non-Target Client의 요청: UpdateWI (무시)");
            }
        }

        void ResponseTimeline()
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 요청 : Rank");

            var str = manager.GetTimelineAsString();
            var bw = new ByteWriter(str);
            var br = new ByteReader(bw);
            Send(br);
        }

        void ResponseRank()
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 요청 : Rank");

            var str = manager.GetRankAsString();
            var bw = new ByteWriter(str);
            var br = new ByteReader(bw);
            Send(br);
        }

        void ResponseTracePrivillege(ByteReader br)
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 요청 : To Be Target");

            bool accepted;
            var nonce = br.ReadShort();

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

            var bw = new ByteWriter();
            bw.Write((short)MessageOp.ResponsePrivillegeTrace);
            bw.Write(nonce);
            bw.Write((short)(accepted ? 1 : 0));
            socket!.Send(bw.ToBytes());
        }

        void ResponseOther(in ByteReader br, MessageOp op)
        {
            VerboseOut.WriteLine($"[D][I][{ClientName}] 처리할 수 없는 요청: {op}");
        }

        public void Message(ByteReader br)
        {
            var str = br.ReadString();
            Out.WriteLine(str);
        }

        public void Send(ByteReader br)
        {
            var str = br.ReadString();

            var bw = new ByteWriter();
            bw.Write((short)MessageOp.Message);
            bw.Write(str);
            socket!.Send(bw.ToBytes());
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