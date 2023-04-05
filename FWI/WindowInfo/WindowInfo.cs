using FWI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public struct WindowInfo
    {
        public WindowInfoType Special;
        public string Title;
        public string Name;
        public string Alias;
        public DateTime Date;
        public TimeSpan Duration;

        public static WindowInfo Empty => new WindowInfo()
        {
            Special = WindowInfoType.Normal,
            Title = "",
            Name = "",
            Alias = "",
            Date = DateTime.Now,
            Duration = TimeSpan.Zero,
        };
        public static WindowInfo AFK => new WindowInfo() { Special = WindowInfoType.AFK };
        public static WindowInfo NoWindow => new WindowInfo() { Special = WindowInfoType.NoWindow };
    }
}
