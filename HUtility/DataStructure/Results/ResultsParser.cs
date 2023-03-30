using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace HUtility.Results
{
    /// <summary>
    /// Results.Parse()의 결과
    /// 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ResultsParser<S, T> where S : Enum
    {
        Dictionary<S, LinkedList<StateResult<S, T>>> dict;

        public ResultsParser(Results<S, T> results)
        {
            dict = new Dictionary<S, LinkedList<StateResult<S, T>>>();

            foreach (var result in results)
            {
                var state = result.State;

                if (!dict.ContainsKey(state))
                {
                    dict[state] = new LinkedList<StateResult<S, T>>();
                }

                dict[state].AddLast(result);
            }
        }
        public ResultsParser<S, T> With(S state, Action<StateResult<S, T>> action)
        {
            if (dict.ContainsKey(state))
            {
                foreach (var result in dict[state])
                {
                    action(result);
                }
            }

            return this;
        }
    }
}
*/