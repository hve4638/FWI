using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    class PrivillegeElevationResult
    {
        public PrivilegeElevation Progress { get; set; }
        public short Nonce { get; set; }
        public Action? OnAccepted { get; private set; }
        public Action<string>? OnDenied { get; private set; }

        public virtual PrivillegeElevationResult WithAccepted(Action onAccepted)
        {
            OnAccepted = onAccepted;
            return this;
        }
        public virtual PrivillegeElevationResult WithDenied(Action<string> onDenied)
        {
            OnDenied = onDenied;
            return this;
        }

        public virtual void Accept()
        {
            Progress = PrivilegeElevation.Accepted;
            OnAccepted?.Invoke();
            Reset();
        }

        public virtual void Deny(string message)
        {
            Progress = PrivilegeElevation.Denied;
            OnDenied?.Invoke(message);
            Reset();
        }

        void Reset()
        {
            OnAccepted = null;
            OnDenied = null;
        }
    }

    class MustDeniedPrivillegeElevationResult : PrivillegeElevationResult
    {
        public string message;
        public MustDeniedPrivillegeElevationResult(string message = "")
        {
            this.message = message;
        }
        public override PrivillegeElevationResult WithDenied(Action<string> onDenied)
        {
            base.WithDenied(onDenied);
            Deny(message);

            return this;
        }
    }

    enum PrivilegeElevation
    {
        None = 0,
        Requested = 1,
        Accepted = 2,
        Denied = 3,
        Canceled = 4
    }
}
