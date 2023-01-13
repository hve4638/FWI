using FWIConnection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TestAPP
{
    internal class Program
    {
        [DllImport("user32")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        const int nChars = 256;
        public static void Main(string[] args)
        {
            while (true)
            {
                var p = GetActiveProcessFileName();
                if (p != null)
                {
                    Console.WriteLine($"PTITLE: {p.MainWindowTitle} PNAME:{p.ProcessName}");
                }
                Thread.Sleep(1000);
            }
        }
        static Process GetActiveProcessFileName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            return Process.GetProcessById((int)pid);
        }
    }
}