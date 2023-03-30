using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Results
{
    public class Result<T> : IEnumerable<T>
    {
        protected virtual List<T> Reasons { get; private set; }

        public Result()
        {
            Reasons = new List<T>();
        }

        public virtual void Add(Result<T> items)
        {
            foreach (T item in items) Add(item);
        }
        public virtual void Add(T item) => Reasons.Add(item);

        public IEnumerator<T> GetEnumerator() => Reasons.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        static public Result<T> operator +(Result<T> source, T item)
        {
            source.Add(item);
            return source;
        }
        static public Result<T> operator +(Result<T> source, Result<T> items)
        {
            source.Add(items);
            return source;
        }
    }
}
