using FWI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWIServer
{
    class ServerManager
    {
        readonly FWIManager manager;
        WindowInfo lastWI = new NoWindowInfo();
        readonly History<WindowInfo> history;
        bool hasTarget;
        bool isAFK;

        public bool HasTarget => hasTarget;
        public ServerManager(FWIManager manager)
        {
            this.manager = manager;
            history = new History<WindowInfo>();
            hasTarget = false;
            isAFK = false;
        }
        public WindowInfo LastWI => (history.Count == 0 ? new NoWindowInfo() : history.GetLast());

        public void SetTarget(Receiver? receiver = null)
        {
            hasTarget = true;
        }

        public void ResetTarget(DateTime? date = null)
        {
            if (hasTarget)
            {
                hasTarget = false;
                SetAFK(date ?? DateTime.Now);
            }
        }

        public void AddWI(WindowInfo wi)
        {
            if (wi is NoWindowInfo) return;
            else if (wi is AFKWindowInfo) SetAFK(wi.Date);
            else
            {
                if (isAFK) Program.VerboseOut.WriteLine("[D][Verbose] AFK가 해제되었습니다. (AddWI에 의해)");

                lastWI = wi;
                isAFK = false;
                manager.AddWI(wi);
                history.Add(wi);
            }
        }

        public void SetAFK(DateTime date)
        {
            if (isAFK) return;

            isAFK = true;
            manager.AddEmpty(date);
            history.Add(new AFKWindowInfo(date));
        }
        public void SetNoAFK(DateTime date)
        {
            if (!isAFK) return;

            isAFK = false;
            if (lastWI is not null)
            {
                var wi = lastWI.Copy();
                wi.Date = date;
                AddWI(wi);
            }
        }

        public List<WindowInfo> GetHistory()
        {
            return history.GetAll();
        }

        public string GetRankAsString()
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

        public string GetTimelineAsString()
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
