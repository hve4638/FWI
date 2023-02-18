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
        [DllImport("user32.dll")]
        static extern bool IsHungAppWindow(IntPtr hWnd);

        static public WindowInfo GetForeground()
        {
            var p = GetProcessForeground(out var isHungAppWindow);

            return new WindowInfo(name: p.ProcessName, title: p.MainWindowTitle, date: DateTime.Now);
        }

        static Process GetProcessForeground(out bool isHungAppWindow)
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);

            isHungAppWindow = IsHungAppWindow(hwnd);
            return Process.GetProcessById((int)pid);
        }
    }
}
