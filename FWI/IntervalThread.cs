using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FWI
{
    public class IntervalThread
    {
        Thread thread;
        readonly Action action;

        public IntervalThread(Action action)
        {
            thread = null;
            this.action = action;
        }

        public void Start(TimeSpan interval, int checkInterval = 100)
        {
            Stop();

            thread = new Thread(() => {
                var lastRun = DateTime.Now;

                while (true)
                {
                    if (DateTime.Now - lastRun > interval)
                    {
                        lastRun = DateTime.Now;

                        action();
                    }
                    Thread.Sleep(checkInterval);
                }
            });
            thread.Start();
        }

        public void Stop()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }
    }
}
