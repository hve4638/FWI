
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI.Prompt;
using FWIConnection;
using FWIConnection.Message;
using System.Collections;

namespace FWIClient
{
    class PromptInitializer
    {
        readonly ClientManager manager;
        public PromptInitializer(Client client, ClientManager manager)
        {
            this.manager = manager;
        }

        public void Init(Prompt prompt)
        {
            prompt.Add("exit", (_) => {
                Program.Exit();
            });

            prompt.Add("debug", (args, output) => {
                if (!args.HasArg(0)) return;

                switch(args[0].ToLower())
                {
                    case "0":
                    case "f":
                        manager.DebugMode = false;
                        output.WriteLine($"Debug disabled");
                        break;

                    case "1":
                    case "t":
                        manager.DebugMode = true;
                        output.WriteLine($"Debug enabled");
                        break;
                }
            });

            prompt.Add("info", (args, output) => {
                output.WriteLine("FWIClient");
                output.WriteLine($"Version: {Program.Version}");
                output.WriteLine($"IsTarget: {manager.Sender.IsTarget}");
            });
            
            prompt.Add("echo", (args) => {
                var str = args.GetArgs();

                manager.Sender.SendEcho(str);
            });

            prompt.Add("message", (args) =>
            {
                var text = args.GetArgs();

                manager.Sender.SendMessage(text);
            });

            prompt.Add("rank", (args) =>
            {
                manager.Sender.SendRequestRank();
            });

            prompt.Add("timeline", (args) =>
            {
                manager.Sender.SendRequestTimeline();
            });

            prompt.Add("call", (args) =>
            {
                var cmd = args.GetArgs();

                manager.Sender.SendServerCall(cmd);
            });
        }
    }
}
