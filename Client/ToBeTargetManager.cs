using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;
using FWI.Results;

namespace FWIClient
{
    public class ToBeTargetManager
    {
        LazyRequestResultManager<string>? manager;
        public ToBeTargetState Progress { get; private set; }
        public short Id { get; private set; }
        public RequestResult<string> Result => manager!.Result;

        public void Request()
        {
            Reset();
            manager = new();
            Id = Nonce();
            Progress = ToBeTargetState.Requested;
        }

        public void Reset()
        {
            Deny("요청이 만료됨");
            Progress = ToBeTargetState.None;
        }

        public virtual void Accept(string message)
        {
            if (Progress == ToBeTargetState.Requested)
            {
                Progress = ToBeTargetState.Responsed;
                manager?.Accept(message);
            }
        }

        public virtual void Deny(string message)
        {
            if (Progress == ToBeTargetState.Requested)
            {
                Progress = ToBeTargetState.Responsed;
                manager?.Deny(message);
            }
        }

        static short Nonce()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            return (short)rand.Next(0, short.MaxValue);
        }

    }

    public enum ToBeTargetState
    {
        None = 0,
        Requested = 1,
        Responsed = 2,
    }
    public enum PrivilegeElevation
    {
        None = 0,
        Requested = 1,
        Accepted = 2,
        Denied = 3,
        Canceled = 4
    }
}
