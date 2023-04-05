using HUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Loggers
{
    public static class RankWIExtender
    {
        public static Rank<WindowInfo, string, TimeSpan> ToRank(this Timeline timeline, DateRange range)
        {
            var wis = timeline.GetWIs(range);
            var rank = new Rank<WindowInfo, string, TimeSpan>((wi) => wi.Name);
            WindowInfo previousWI = WindowInfo.NoWindow;
            foreach (var wi in wis)
            {
                if (previousWI != null) rank[previousWI] += (wi.Date - previousWI.Date);

                previousWI = wi;
            }
            
            if (previousWI != null) rank[previousWI] += (range.End - previousWI.Date);
            return rank;
        }
    }
}
