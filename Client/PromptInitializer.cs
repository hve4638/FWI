using FWIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI.Prompt;
using System.Collections;

namespace FWIClient
{
    class PromptInitializer
    {
        readonly Client client;
        readonly ClientManager manager;
        public PromptInitializer(Client client, ClientManager manager)
        {
            this.client = client;
            this.manager = manager;
        }

        public void Init(Prompt prompt)
        {
            prompt.Add("exit", (_) => {

            });

            prompt.Add("echo", (args) => {
                var bw = new ByteWriter();
                var str = args.GetArgs();

                bw.Write((short)MessageOp.Echo);
                bw.Write(str);
                client.Send(bw.ToBytes());
            });

            prompt.Add("message", (args) =>
            {
                var bw = new ByteWriter();
                var str = args.GetArgs();

                bw.Write((short)MessageOp.Message);
                bw.Write(str);
                client.Send(bw.ToBytes());
            });

            prompt.Add("rank", (args) =>
            {
                var bw = new ByteWriter();
                bw.Write((short)MessageOp.RequestRank);
                client.Send(bw.ToBytes());
            });

            prompt.Add("timeline", (args) =>
            {
                var bw = new ByteWriter();
                bw.Write((short)MessageOp.RequestTimeline);
                client.Send(bw.ToBytes());
            });

            prompt.Add("call", (args) =>
            {
                var cmd = args.GetArgs();

                var bw = new ByteWriter();
                bw.Write((short)MessageOp.ServerCall);
                bw.Write(cmd);
                client.Send(bw.ToBytes());
            });

            prompt.Add("afk", (args) =>
            {
                Program.Out.WriteLine("Send AFK");
                manager.SendAFK();
            });
        }
    }
}
