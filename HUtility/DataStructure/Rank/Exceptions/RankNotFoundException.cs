using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    public class RankNotFoundException : Exception
    {
        public RankNotFoundException() : base()
        {

        }

        public RankNotFoundException(string message) : base(message)
        {

        }
    }
}
