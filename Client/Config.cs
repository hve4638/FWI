using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    /// <summary>
    /// Config 창에서 수정가능한 옵션들
    /// </summary>
    public struct Config
    {
        public string serverIP;
        public string serverPort;
        public string afkTime;
        public bool autoReload;
        public bool debug;
        public bool observerMode;
        public bool openConsoleWhenStartup;

        public override string ToString()
        {
            return
                $"{base.ToString}\n" +
                $"ip   : {serverIP}\n" +
                $"port : {serverPort}\n" +
                $"afk  : {afkTime}\n" +
                $"auto reload: {autoReload}\n" +
                $"observer   : {observerMode}\n" +
                $"openConsoleWhenStartup   : {openConsoleWhenStartup}\n" +
                $"debug      : {debug}\n";
        }
    }
}
