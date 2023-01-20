using FWIConnection.Message;
using FWIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace FWIClient
{
    class TraceForegroundWindow
    {
        static public void Trace(Action<WindowInfoCapture> onTrace, int traceInterval = 1000)
        {
            string pName = "", pTitle = "";
            try
            {
                while (true)
                {
                    Thread.Sleep(traceInterval);

                    var wi = WindowInfoCapture.GetForeground();

                    if (wi.Name != pName || wi.Title != pTitle)
                    {
                        pName = wi.Name;
                        pTitle = wi.Title;
                        onTrace(wi);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {

            }
            catch (SocketException)
            {

            }
        }
    }
}
