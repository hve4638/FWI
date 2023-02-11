using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    class Threads
    {
        readonly LinkedList<Thread> list;
        public Threads()
        {
            list = new LinkedList<Thread>();
        }

        public void Join()
        {
            foreach (var thread in list) thread.Join();
        }

        public void Interrupt()
        {
            foreach (var thread in list) thread.Interrupt();
        }

        public void Clear()
        {
            list.Clear();
        }

        static public Threads operator +(Threads threads, Thread thread)
        {
            threads.list.AddLast(thread);
            return threads;
        }

        static public Threads operator -(Threads threads, Thread thread)
        {
            threads.list.Remove(thread);
            return threads;
        }
    }
}
