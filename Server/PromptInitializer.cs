using FWI.Prompt;
using FWI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace FWIServer
{
    internal class PromptInitializer
    {
        readonly FWIManager manager;
        readonly LinkedList<Receiver> sessions;
        public PromptInitializer(FWIManager manager, LinkedList<Receiver> sessions)
        {
            this.manager = manager;
            this.sessions = sessions;
        }

        public void Initialize(Prompt prompt)
        {
            prompt.Add("timeline", () => {
                var output = manager.GetTimelineString();
                prompt.Out.WriteLine(output);
            });
            prompt.Add("rank", () => {
                var ranks = manager.GetRankString();
                prompt.Out.WriteLine(ranks);
            });
            prompt.Add("import", (_, output) => {
                manager.Import(manager.Signiture);
                output.WriteLine($"[D][I] import success");
            });
            prompt.Add("export", (_, output) => {
                manager.Export(manager.Signiture);
                output.WriteLine($"[D][I] export success");
            });
            prompt.Add("interval", (args, output) => {
                if (int.TryParse(args[0], out int num))
                {
                    manager.SetLoggingInterval(num);
                    output.WriteLine($"[D][I] Set LoggingInterval : {num} minutes");
                }
                else
                {
                    output.WriteLine("Set failed");
                }
            });
            prompt.Add("save", () => { manager.SaveFilter(); });
            prompt.Add("reload", (args, output) => {
                var results = manager.LoadFilter();
                if (results.IsNormal)
                {
                    output.WriteLine("[D][I] Load successful");
                }
                else if (results.HasProblem)
                {
                    output.WriteLine("[D][W] Something went wrong while loading");
                    foreach (var result in results) output.WriteLine($"  {result}");
                }

            });
            prompt.Add("show", (args, output) =>
            {
                if (args.Count == 0) return;

                switch(args[0])
                {
                    case "rank":
                        {
                            var ranks = manager.GetRankString();
                            output.WriteLine(ranks);
                            break;
                        }

                    case "group":
                        break;

                    case "ignore":
                        {
                            var ignoreSet = manager.GetIgnore();

                            output.WriteLine($"ignore list:");
                            foreach (var value in ignoreSet)
                                output.WriteLine($"  - {value}");
                            break;
                        }

                    case "alias":
                        {
                            var aliasDict = manager.GetAlias();

                            output.WriteLine($"alias:");
                            foreach (var (key, value) in aliasDict)
                                output.WriteLine($"  {key}: {value}");
                            break;
                        }

                    case "current":
                    case "lastwi":
                        output.WriteLine($"last WI: {manager.LastWI}");
                        break;

                    case "history":
                        {
                            var list = manager.History;

                            output.WriteLine($"history");
                            foreach (var item in list)
                            {
                                output.WriteLine($"- {item}");
                            }
                            break;
                        }
                        
                    case "client":
                        {
                            output.WriteLine($"Client List:");
                            foreach (var session in sessions)
                            {
                                var info = session.Info();

                                output.Write($"- [{info.Addr}:{info.Port}] ");
                                if (info.CanTrace) output.Write($"Target");
                                else output.Write($"Observer");

                                output.WriteLine("");
                            }
                            break;
                        }
                }
            });
            prompt.Add("client", (args, output) => {
                output.WriteLine($"Client List:");
                foreach (var session in sessions)
                {
                    var info = session.Info();

                    output.Write($"- [{info.Addr}:{info.Port}] ");
                    if (info.CanTrace) output.Write($"Target");
                    else output.Write($"Observer");
                }
            });

            prompt.Add("uptime", (_, output) =>
            {
                var elapsed = Program.Elapsed;
                var h = (int)elapsed.TotalHours;
                var m = elapsed.Minutes;
                var s = elapsed.Seconds;
                output.WriteLine($"uptime: {h:00}:{m:00}:{s:00}");
            });
        }
    }
}
