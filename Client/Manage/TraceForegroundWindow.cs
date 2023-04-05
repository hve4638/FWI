using FWI.Message;
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
    class FWITracker
    {
        string lastName;
        string lastTitle;
        public FWITracker()
        {
            lastName = "";
            lastTitle = "";
        }

        public WindowInfoLegacy Track(int interval = 100)
        {
            while (true)
            {
                var wi = WICapture.GetForeground();

                if (wi.Name != lastName || wi.Title != lastTitle)
                {
                    lastName = wi.Name;
                    lastTitle = wi.Title;
                    return wi;
                }
                Thread.Sleep(interval);
            }
        }

        public WindowInfoLegacy Tracking(Func<WindowInfoLegacy, WindowInfoLegacy> filter)
        {
            while (true)
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

        static public void TrackingAsync(Action<WindowInfoLegacy> onTrace, int traceInterval = 1000)
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
