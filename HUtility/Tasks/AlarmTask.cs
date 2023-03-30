using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HUtility
{
    public class AlarmTask
    {
        Task task;
        readonly bool loop;
        readonly Action action;
        CancellationTokenSource cancelTokenSource;

        public AlarmTask(Action action, bool loop = false)
        {
            task = null;
            this.action = action;
            this.loop = loop;
        }

        public void Start(TimeSpan interval, int checkInterval = 100)
        {
            cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;

            task = new Task(() => {
                var lastRun = DateTime.Now;

                while (loop && !cancelToken.IsCancellationRequested)
                {
                    Thread.Sleep(checkInterval);

                    if (DateTime.Now - lastRun > interval)
                    {
                        lastRun = DateTime.Now;

                        action();
                    }
                }
            });
            task.Start();
        }

        public void Stop() => cancelTokenSource.Cancel();
        public void Wait() => task.Wait();
    }
}
