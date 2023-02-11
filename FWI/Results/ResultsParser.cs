using FWI.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Results
{
    public class ResultsParser<S, T> where S : Enum
    {
        Dictionary<S, LinkedList<Result<S, T>>> dict;

        public ResultsParser(Results<S, T> results)
        {
            dict = new Dictionary<S, LinkedList<Result<S, T>>>();

            foreach (var result in results)
            {
                var state = result.State;

                if (!dict.ContainsKey(state))
                {
                    dict[state] = new LinkedList<Result<S, T>>();
                }

                dict[state].AddLast(result);
            }
        }
        public ResultsParser<S, T> With(S state, Action<Result<S, T>> action)
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
