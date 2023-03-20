#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUtility.Testing;

namespace FWI.Test
{

    [TestClass]
    public class TimelineHashTest
    {
        [TestMethod]
        public void ContentsHashEqual()
        {
            var timeline = new Timeline();
            var timeline2 = new Timeline();

            Assert.AreNotEqual(timeline.GetHashCode(), timeline2.GetHashCode());
            Assert.AreEqual(timeline.GetContentsHashCode(), timeline2.GetContentsHashCode());
        }

        [TestMethod]
        public void ContentsHashNotEqual()
        {
            var timeline = new Timeline();
            var timeline2 = new Timeline();
            timeline2.AddLog(new WindowInfo(name: "1", date: TestUtils.MakeDateTime("000101 120000")));
            timeline2.AddLog(new WindowInfo(name: "2", date: TestUtils.MakeDateTime("000101 120005")));

            Assert.AreNotEqual(timeline.GetHashCode(), timeline2.GetHashCode());
            Assert.AreNotEqual(timeline.GetContentsHashCode(), timeline2.GetContentsHashCode());
        }
    }

}
#endif