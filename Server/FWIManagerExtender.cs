using FWI;
using FWI.Prompt;
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
            var last = "_";
            var output = "";
            foreach (var item in manager.GetTimeline())
            {
                var name = item.GetAliasOrName();

                if (last == name) name = "";
                else last = name;

                output += $"{item.Date:yyMMdd HH:mm:ss}\t|\t{name,-15}\t|\t{item.Title}\t\n";

            }
                
            return output;
        }

    }
}
