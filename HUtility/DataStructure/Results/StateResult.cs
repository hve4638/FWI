using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Results
{
    /// <summary>
    /// State를 가진 Result
    /// </summary>
    /// <typeparam name="S">State</typeparam>
    /// <typeparam name="T">Reason</typeparam>
    public class StateResult<T, U> : IEnumerable<U> where T : Enum
    {
        public virtual T State { get; set; }
        protected virtual List<U> Reasons { get; set; }

        public StateResult() : base()
        {

        }
        public StateResult(T state) : base()
        {
            State = state;
        }

        public virtual StateResult<T, U> Add(U item)
        {
            Reasons.Add(item);
            return this;
        }
        public virtual StateResult<T, U> Add(StateResult<T, U> items)
        {
            foreach (U item in items)
            {
                Reasons.Add(item);
            }
            return this;
        }
        static public StateResult<T, U> operator +(StateResult<T, U> source, U item)
        {
            source.Add(item);
            return source;
        }
        static public StateResult<T, U> operator +(StateResult<T, U> source, StateResult<T, U> other)
        {
            source.Add(other);
            return source;
        }

        public IEnumerator<U> GetEnumerator() => Reasons.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
