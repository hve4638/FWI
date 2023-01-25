﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Results
{
    public class LazyRequestResultManager<T>
    {
        public LazyRequestResult<T> Result { get; private set; }
        public Action<T> OnAccepted { get; set; }
        public Action<T> OnDenied { get; set; }

        public LazyRequestResultManager()
        {
            Result = new LazyRequestResult<T>(this);
        }

        public virtual void Accept(T result)
        {
            OnAccepted(result);
        }
        public virtual void Deny(T result)
        {
            OnDenied(result);
        }
    }
}
