using FWI.Prompt;
using FWI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;
using FWI.Results;

namespace FWIServer
{
    internal class PromptInitializer
    {
        readonly ServerManager serverManager;
        readonly FWIManager fwiManager;
        readonly LinkedList<Receiver> sessions;
        public PromptInitializer(FWIManager fwiManager, ServerManager serverManager, LinkedList<Receiver> sessions)
        {
            this.fwiManager = fwiManager;
            this.serverManager = serverManager;
            this.sessions = sessions;
        }

        public void Initialize(Prompt prompt)
        {
            prompt.Add("load", (_, output) => {
                fwiManager.Import(fwiManager.Signiture);
                output.WriteLine($"[D][I] logger load success");
            });
            prompt.Add("save", (_, output) => {
                fwiManager.Export(fwiManager.Signiture);
                output.WriteLine($"[D][I] logger save success");
            });
            prompt.Add("interval", (args, output) => {
                if (int.TryParse(args[0], out int num))
                {
                    fwiManager.SetLoggingInterval(num);
                    output.WriteLine($"[D][I] Set loggingInterval : {num} minutes");
                }
                else
                {
                    output.WriteLine("set failed");
                }
            });
            prompt.Add("reload", (args, output) => {
                var results = fwiManager.LoadFilter();
                if (results.State == ResultState.Normal)
                {
                    output.WriteLine("[D][I] Load config successful");
                }
                else if (results.State == ResultState.HasProblem)
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
                    case "timeline":
                        {
                            var timeline = serverManager.GetTimelineAsString();
                            output.WriteLine(timeline);
                            break;
                        }

                    case "rank":
                        {
                            var ranks = serverManager.GetRankAsString();
                            output.WriteLine(ranks);
                            break;
                        }

                    case "group":
                        break;

                    case "ignore":
                        {
                            var ignoreSet = fwiManager.GetIgnore();

                            output.WriteLine($"ignore list:");
                            foreach (var value in ignoreSet)
                                output.WriteLine($"  - {value}");
                            break;
                        }

                    case "alias":
                        {
                            var aliasDict = fwiManager.GetAlias();

                            output.WriteLine($"alias:");
                            foreach (var (key, value) in aliasDict)
                                output.WriteLine($"  {key}: {value}");
                            break;
                        }

                    case "current":
                    case "lastwi":
                        output.WriteLine($"last WI: {serverManager.LastWI}");
                        break;

                    case "history":
                        {
                            var list = serverManager.GetHistory();

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
            prompt.Add("client", "show client");
            prompt.Add("lastwi", "show lastwi");
            prompt.Add("timeline", "show timeline");
            prompt.Add("rank", "show rank");

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
