using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace HUtility.Results
{
    public class Results<S, T> : StateResult<StateResult<S, T>>
    {
        public static Results<S, T> operator +(Results<S, T> result, Results<S, T> item)
        {
            result.Add(item);
            return result;
        }

        public static Results<S, T> operator +(Results<S, T> result, StateResult<S, T> item)
        {
            result.Add(item);
            return result;
        }
        
        public ResultsParser<S, T> Parse()
        {
            return new ResultsParser<S, T>(this);
        }
    }
}*/