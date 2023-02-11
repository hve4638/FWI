using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIServer
{
    public enum ServerResultState
    {
        None,
        Normal,
        NonFatalIssue,
        FatalIssue,
        ChangeAFK,
        Info,
    }
}
