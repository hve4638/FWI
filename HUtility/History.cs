using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    public class History<T>
    {
        readonly LinkedList<T> linked;
        public int MaxHistory { get; set; }
        public int Count => linked.Count;

        public History()
        {
            linked = new LinkedList<T>();
            MaxHistory = 100;
        }

        public void Add(T item)
        {
            linked.AddLast(item);
            while (linked.Count >= MaxHistory) linked.RemoveFirst();
        }

        public void Clear()
        {
            linked.Clear();
        }

        public List<T> GetAll()
        {
            var list = new List<T>();
            foreach (var item in linked) list.Add(item);
            return list;
        }

        public T GetLast()
        {
            return linked.Last();
        }

        static public History<T> operator +(History<T> history, T item)
        {
            history.Add(item);

            return history;
        }

        public T this[int index]
        {
            get
            {
                if (Count > index)
                {
                    var node = linked.First;
                    for (int i = 0; i < index; i++) node = node.Next;

                    return node.Value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
    }
}
