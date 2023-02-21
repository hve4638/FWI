using FWI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public static class RemoteConsole
    {
        public static void Open(int port)
        {
            Process process = new();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = @".\FWIConsole.exe";
            process.StartInfo.Arguments = $"127.0.0.1 {port}";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
        }
    }
}
