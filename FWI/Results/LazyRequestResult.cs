using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Results
{
    public class LazyRequestResult<T> : RequestResult<T>
    {
        LazyRequestResultManager<T> manager;

        public LazyRequestResult(LazyRequestResultManager<T> manager) : base(RequestResultState.None, default)
        {
            this.manager = manager;
        }

        public override RequestResult<T> WithAccepted(Action<T> onAccepted)
        {
            manager.OnAccepted += onAccepted;
            return this;
        }
        public override RequestResult<T> WithDenied(Action<T> onDenied)
        {
            manager.OnDenied += onDenied;
            return this;
        }
    }
}
