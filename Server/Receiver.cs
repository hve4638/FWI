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
using System.Xml.Linq;

namespace FWIServer
{
    internal class Receiver : IReceiver
    {
        FWIManager manager;
        IPEndPoint? client;
        Socket? socket;
        Func<bool>? onVerbose;
        Action<Receiver>? onConnect;
        Action<Receiver>? onDisconnect;
        bool CanTrace { get; set; }

        public Receiver(FWIManager manager, Action<Receiver>? onConnect = null, Action<Receiver>? onDisconnect = null)
        {
            this.manager = manager;
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
                CanTrace = CanTrace,
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

            Console.WriteLine($"Connect: {client?.Address}:{client?.Port}");
            onConnect?.Invoke(this);
        }
        public void Disconnect()
        {
            if (CanTrace)
            {
                CanTrace = false;
                manager.TraceCount--;
            }
            
            Console.WriteLine($"Disconnect: {client?.Address}:{client?.Port}");
            onDisconnect?.Invoke(this);
        }
        public void Receive(in byte[] buf, int size)
        {
            var br = new ByteReader(buf, size);
            var op = (MessageOp)br.ReadShort();

            var verbose = onVerbose?.Invoke() ?? false;
            if (verbose) Console.WriteLine($"[Response] {op}");
            switch (op)
            {
                case MessageOp.UpdateCurrentWI:
                    if (CanTrace) ResponseUpdateCurrentWI(br);
                    else if (verbose) Console.WriteLine($"Ignore UpdateCurrentWI");
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
                case MessageOp.RequestPrivillegeTrace:
                    ResponseTracePrivillege(br);
                    break;

                default:
                    break;
            }
        }


        void ResponseUpdateCurrentWI(ByteReader br)
        {
            WindowInfoMessage.Parse(br, name: out string name, title: out string title, date: out DateTime date);

            var wi = new WindowInfo(name: name, title: title, date: date);
            manager.AddWI(wi);

            var verbose = onVerbose?.Invoke() ?? false;
            if (verbose) Console.WriteLine($"response WI : {wi.Name} {wi.Title} {wi.Date}");
        }

        void ResponseTimeline()
        {
            var str = manager.GetTimelineString();
            var bw = new ByteWriter(str);
            var br = new ByteReader(bw);
            Send(br);
        }

        void ResponseRank()
        {
            var str = manager.GetRankString();
            var bw = new ByteWriter(str);
            var br = new ByteReader(bw);
            Send(br);
        }

        void ResponseTracePrivillege(ByteReader br)
        {
            bool accepted;
            var nonce = br.ReadShort();

            if (manager.TraceCount == 0)
            {
                CanTrace = true;
                manager.TraceCount++;
                accepted = true;
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

        public void Message(ByteReader br)
        {
            var str = br.ReadString();
            Console.WriteLine(str);
        }

        public void Send(ByteReader br)
        {
            var str = br.ReadString();

            var bw = new ByteWriter();
            bw.Write((short)MessageOp.Message);
            bw.Write(str);
            socket!.Send(bw.ToBytes());
        }

        public string GetRank()
        {
            var ranks = manager.GetRanks();
            Dictionary<int, RankResult<WindowInfo>> sorted = ranks.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var str = "";
            foreach (var item in sorted)
            {
                var result = item.Value;
                var ranking = result.Ranking;
                var name = result.Item.Name;
                var duration = result.Duration;

                str += $"{ranking}\t|\t{name}\t|\t{duration}\n";
            }
            return str;
        }

        public string GetTimeline()
        {
            var output = "";
            int num = 1;
            foreach (var item in manager.GetTimeline())
                output += $"{num++}. {item.Title}\t|\t{item.Name}\t|\t{item.Date}\n";
            return output;
        }
    }

    public class ReceiverInfo
    {
        public string? Addr { get; set; }
        public int Port { get; set; }
        public bool CanTrace { get; set; }
    }
}