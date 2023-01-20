using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Results
{
    public class Results<T> : IEnumerable<T>
    {
        readonly List<T> results;

        public ResultState State { get; set; }

        public Results()
        {
            State = ResultState.Normal;
            results = new List<T>();
        }


        public void Add(T item)
        {
            results.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return results.GetEnumerator();
        }

        static public Results<T> operator +(Results<T> result, T item) 
        {
            result.Add(item);
            return result;
        }

        public bool IsNormal => State == ResultState.Normal;
        public bool HasProblem => State == ResultState.HasProblem;
    }
}
