using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Commands
{
    public delegate void CommandDelegate(CommandArgs args, IOutputStream output);
}
