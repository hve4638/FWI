using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWIClient
{
    public class PromptClient
    {
        readonly Client client;
        public PromptClient(Client client)
        {
            this.client = client;
        }

        public void Run()
        {
            string[]? cmdArray;
            string cmd;
            ArraySegment<string> args;
            while (true)
            {
                Console.Write("> ");
                cmdArray = Console.ReadLine()?.Split(" ");
                if (cmdArray == null || cmdArray.Length == 0) continue;
                else
                {
                    cmd = cmdArray[0];
                    args = new ArraySegment<string>(cmdArray, 1, cmdArray.Length-1);
                }

                switch (cmd)
                {
                    case "exit":
                        return;

                    case "echo":
                        Echo(args);
                        break;

                    case "message":
                        Message(args);
                        break;

                    case "rank":
                        RequestRank();
                        break;

                    case "timeline":
                        RequestTimeline();
                        break;
                }
            }
        }

        void Echo(ArraySegment<string> args)
        {
            var bw = new ByteWriter();
            var str = JoinString(args);

            bw.Write((short)MessageOp.Echo);
            bw.Write(str);
            client.Send(bw.ToBytes());
        }

        void Message(ArraySegment<string> args)
        {
            var bw = new ByteWriter();
            var str = JoinString(args);

            bw.Write((short)MessageOp.Message);
            bw.Write(str);
            client.Send(bw.ToBytes());
        }

        void RequestRank()
        {
            var bw = new ByteWriter();
            bw.Write((short)MessageOp.RequestRank);
            client.Send(bw.ToBytes());
        }

        void RequestTimeline()
        {
            var bw = new ByteWriter();
            bw.Write((short)MessageOp.RequestTimeline);
            client.Send(bw.ToBytes());
        }

        static string JoinString(ArraySegment<string> args)
        {
            var str = new StringBuilder();
            foreach (var arg in args) str.Append(arg);
            return str.ToString();
        }
    }
}
