using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FWITest.Learning
{
    [TestClass]
    public class ThreadTest
    {
        [TestMethod]
        public void TestThreadInterrupt()
        {
            var thread = new Thread(() => { });

            thread.Interrupt();
        }
    }
}
