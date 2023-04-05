using FWI;
using FWI.Exceptions;
using FWI.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HUtility;

namespace FWIServer
{
    class ServerManager
    {
        readonly FWIManager manager;
        WindowInfoLegacy lastWI = new NoWindowInfoLegacy();
        readonly History<WindowInfoLegacy> history;
        bool hasTarget;
        bool isAFK;

        public bool HasTarget => hasTarget;
        public ServerManager(FWIManager manager)
        {
            this.manager = manager;
            history = new History<WindowInfoLegacy>();
            hasTarget = false;
            isAFK = false;
        }
        public WindowInfoLegacy LastWI => (history.Count == 0 ? new NoWindowInfoLegacy() : history.GetLast());

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

        public Results<ServerResultState, string> AddWI(WindowInfoLegacy wi)
        {
            var results = new Results<ServerResultState, string>();
            if (wi is NoWindowInfoLegacy && wi is AFKWindowInfoLegacy)
            {
                var result = new Result<ServerResultState, string>(ServerResultState.NonFatalIssue);
                result += "처리할 수 없는 WindowInfo";
                result += $"Item: {wi}";
                result += "처리 - 무시됨";

                results += result;
            }
            else
            {
                if (isAFK)
                {
                    var result = new Result<ServerResultState, string>(ServerResultState.ChangeAFK);
                    result += $"AFK가 해제되었습니다. ({wi.Date})";

                    results += result;
                }

                lastWI = wi;
                isAFK = false;

                try
                {
                    var result = manager.AddWI(wi);
                    history.Add(wi);

                    if (result.State == ResultState.Normal)
                    {
                        results += new Result<ServerResultState, string>(ServerResultState.Normal);
                    }
                }
                catch(TimeSequenceException e)
                {
                    var result = new Result<ServerResultState, string>(ServerResultState.NonFatalIssue);
                    result += "TimeSeqeunce 충돌";
                    result += $"Last - {e.Last}";
                    result += $"Input - {e.Input}";
                    result += "처리 - 무시됨";

                    results += result;
                }
            }


            return results;
        }

        public Results<ServerResultState, string> SetAFK(DateTime date)
        {
            var results = new Results<ServerResultState, string>();
            
            if (isAFK)
            {
                var result = new Result<ServerResultState, string>(ServerResultState.NonFatalIssue);
                result += "AFK 중복";
                result += "처리 - 무시됨";

                results += result;
            }
            else
            {
                isAFK = true;

                try
                {
                    manager.AddEmpty(date);
                    history.Add(new AFKWindowInfoLegacy(date));

                    results += new Result<ServerResultState, string>(ServerResultState.Normal);
                    results += new Result<ServerResultState, string>(ServerResultState.ChangeAFK);
                }
                catch (TimeSequenceException e)
                {
                    results += SetAFK(e.Last + new TimeSpan(0,0,1));

                    var result = new Result<ServerResultState, string>(ServerResultState.NonFatalIssue);
                    result += "TimeSeqeunce 충돌";
                    result += $"Last - {e.Last}";
                    result += $"Input - {e.Input}";
                    result += "처리 - 충돌 시간 이후로 AFK 시간 조정";

                    results += result;
                }
            }

            return results;
        }

        public Results<ServerResultState, string> SetNoAFK(DateTime date)
        {
            var results = new Results<ServerResultState, string>();

            if (isAFK)
            {
                isAFK = false;

                if (lastWI is null)
                {
                    var result = new Result<ServerResultState, string>
                    {
                        State = ServerResultState.Info
                    };
                    result += "이전 로그가 없음";

                    results += result;
                }
                else
                {
                    var wi = lastWI.Copy();
                    wi.Date = date;

                    try
                    {
                        results += AddWI(wi);
                    }
                    catch (TimeSequenceException e)
                    {
                        var result = new Result<ServerResultState, string>(ServerResultState.NonFatalIssue);
                        result += "TimeSeqeunce 충돌";
                        result += $"Last - {e.Last}";
                        result += $"Input - {e.Input}";
                        result += "처리 - AFK 해제, 추가 로그 기록 무시";

                        results += result;
                    }
                }
            }
            else
            {
                var result = new Result<ServerResultState, string>(ServerResultState.NonFatalIssue);
                result += "AFK 상태가 아님";
                result += "처리 - 무시됨";

                results += result;
            }

            return results;
        }

        public List<WindowInfoLegacy> GetHistory()
        {
            return history.GetAll();
        }

        public string GetRankAsString()
        {
            manager.Update();
            var ranks = manager.GetRanks();
            Dictionary<int, RankResult<WindowInfoLegacy>> sorted = ranks.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var str = "";
            foreach (var item in sorted)
            {
                var result = item.Value;
                var ranking = result.Ranking;
                var name = result.Item.GetAliasOrName();
                var duration = result.Duration;

                str += $"{ranking}\t|{name.PadCenter(30)}|\t{duration}\n";
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

                output += $"{item.Date:yyMMdd HH:mm:ss}\t|{name.PadCenter(30)}|\t{item.Title}\t\n";
            }
            return output;
        }
    }
}
