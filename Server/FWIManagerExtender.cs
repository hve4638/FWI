using FWI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIServer
{
    static class FWIManagerExtender
    {
        static public string GetRankString(this FWIManager manager)
        {
            manager.Update();
            var ranks = manager.GetRanks();
            Dictionary<int, RankResult<WindowInfo>> sorted = ranks.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var str = "";
            foreach (var item in sorted)
            {
                var result = item.Value;
                var ranking = result.Ranking;
                var name = result.Item.GetAliasOrName();
                var duration = result.Duration;

                str += $"{ranking}\t|\t{name}\t|\t{duration}\n";
            }
            return str;
        }

        static public string GetTimelineString(this FWIManager manager)
        {
            var output = "";
            foreach (var item in manager.GetTimeline())
                output += $"{item.Date:yyMMdd HH:mm:ss}\t|\t{item.GetAliasOrName(),-15}\t|\t{item.Title}\t\n";
            return output;
        }

        static public void AppendPromptCommand(this FWIManager manager, Prompt prompt)
        {
            prompt.Add("timeline", (args) => {
                var output = manager.GetTimelineString();
                Console.WriteLine(output);
            });
            prompt.Add("rank", (args) => {
                var ranks = manager.GetRankString();
                Console.WriteLine(ranks);
            });
            prompt.Add("import", (args) => {
                try
                {
                    manager.Import(manager.Signiture);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"import fail : File not found exeception");
                    return;
                }

                Console.WriteLine($"import success");
            });
            prompt.Add("export", (args) => {
                manager.Export(manager.Signiture);
                Console.WriteLine($"export success");
            });
            prompt.Add("interval", (args) => {
                if (int.TryParse(args[0], out int num))
                {
                    manager.SetLoggingInterval(num);
                    Console.WriteLine($"Set LoggingInterval : {num} minutes");
                }
                else
                {
                    Console.WriteLine("Set failed");
                }
            });
            prompt.Add("save", (args) => {
                manager.SaveFilter(manager.Signiture);
            });
            prompt.Add("reload", (args) => {
                manager.LoadFilter(manager.Signiture);
            });

        }
    }
}
