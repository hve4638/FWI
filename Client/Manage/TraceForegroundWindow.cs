using FWIConnection.Message;
using FWIConnection;
using FWI;
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
        string lastName;
        string lastTitle;
        public TraceForegroundWindow()
        {
            lastName = "";
            lastTitle = "";
        }
        
        public WindowInfo TrackingFWI()
        {
            while(true)
            {
                var wi = WICapture.GetForeground();

                if (wi.Name != lastName || wi.Title != lastTitle)
                {
                    lastName = wi.Name;
                    lastTitle = wi.Title;
                    return wi;
                }
            }
        }

        static public void Trace(Action<WindowInfo> onTrace, int traceInterval = 1000)
        {
            string pName = "", pTitle = "";
            try
            {
                while (true)
                {
                    Thread.Sleep(traceInterval);

                    var wi = WICapture.GetForeground();

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
