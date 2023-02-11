using FWI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FWIClient
{
    static class WICapture
    {
        [DllImport("user32")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        static public WindowInfo GetForeground()
        {
            var p = GetProcessForeground();

            return new WindowInfo(name: p.ProcessName, title: p.MainWindowTitle, date: DateTime.Now);
        }

        static Process GetProcessForeground()
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);
            return Process.GetProcessById((int)pid);
        }
    }
}
