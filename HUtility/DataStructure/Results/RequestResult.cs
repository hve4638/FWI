using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace HUtility.Results
{
    public class RequestResult<T>
    {
        readonly protected RequestResultState state;
        readonly protected T result;

        public RequestResult(RequestResultState state, T result)
        {
            this.state = state;
            this.result = result;
        }

        public virtual RequestResult<T> WithAccepted(Action<T> onAccepted)
        {
            if (state == RequestResultState.Accepted)
                onAccepted(result);
            return this;
        }
        public virtual RequestResult<T> WithDenied(Action<T> onDenied)
        {
            if (state == RequestResultState.Denied)
                onDenied(result);
            return this;
        }
    }
}
*/