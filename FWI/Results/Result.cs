using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Results
{
    public class Result<T> : IEnumerable<T>
    {
        private readonly List<T> list;

        public Result()
        {
            list = new List<T>();
        }

        public virtual void Add(T item)
        {
            list.Add(item);
        }

        public virtual void Add(Result<T> items)
        {
            foreach (T item in items)
            {
                list.Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

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

    public class Result<S, T> : Result<T> where S : Enum
    {
        public virtual S State { get; set; }

        public Result() : base()
        {

        }
        public Result(S state) : base()
        {
            State = state;
        }

        static public Result<S, T> operator +(Result<S, T> source, T item)
        {
            source.Add(item);
            return source;
        }

        static public Result<S, T> operator +(Result<S, T> source, Result<S, T> other)
        {
            source.Add(other);
            return source;
        }

        static public Result<S, T> operator +(Result<S, T> source, Result<T> other)
        {
            source.Add(other);
            return source;
        }
    }
}
