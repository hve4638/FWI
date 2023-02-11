using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public class Tasks
    {
        readonly LinkedList<Task> list;
        readonly CancellationTokenSource tokenSource;
        public CancellationToken Token => tokenSource.Token;

        public Tasks()
        {
            list = new LinkedList<Task>();
            tokenSource = new CancellationTokenSource();
        }

        public void Wait()
        {
            foreach (var task in list) task.Wait();
        }

        public void Cancel()
        {
            tokenSource.Cancel();
        }

        public void Clear() => list.Clear();
    }
}
