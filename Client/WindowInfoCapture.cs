using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FWIClient
{
    internal class WindowInfoCapture
    {
        [DllImport("user32")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        readonly string name;
        readonly string title;
        readonly DateTime date;
        private WindowInfoCapture(string name, string title, DateTime? date = null)
        {
            this.name = name;
            this.title = title;
            this.date = date ?? DateTime.Now;
        }

        static public WindowInfoCapture GetForeground()
        {
            var p = GetProcessForeground();
            return new WindowInfoCapture(p.ProcessName, p.MainWindowTitle);
        }

        static Process GetProcessForeground()
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);
            return Process.GetProcessById((int)pid);
        }

        public string Name => name;
        public string Title => title;
        public DateTime Date => date;
        override public string ToString() => $"<WindowInfoCapture('{name}','{Title}')>";
    }
}
