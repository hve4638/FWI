#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUtility;
using HUtility.Testing;
using FWI.Loggers;

namespace FWI.Test
{
    [TestClass]
    public class TimelineRankCacheTest
    {
        [TestMethod]
        public void GetCache()
        {
            var timeline = new Timeline();
            var rankCache = new TimelineRankCache(timeline);
            rankCache.Interval = new TimeSpan(0, 30, 0);
            WindowInfo[] wis =
            {
                new WindowInfo(name: "A", date: TestUtils.MakeDateTime("220101 120000")), // 20min
                new WindowInfo(name: "B", date: TestUtils.MakeDateTime("220101 122000")), // 30min
                new WindowInfo(name: "C", date: TestUtils.MakeDateTime("220101 125000")), // 10min
                new WindowInfo(name: "D", date: TestUtils.MakeDateTime("220101 130000")),
            };
            timeline.AddWIs(wis);

            var range = TestUtils.MakeDateTimeRange("220101 120000", "220101 130000");
            
            var expected = timeline.ToRank(range);
            var actual = rankCache.GetRank(range);
            var a = expected.GetRank(1);
            var b = actual.GetRank(1);
            Assert.AreEqual(expected.GetRank(1), actual.GetRank(1));
            Assert.AreEqual(expected.GetRank(2), actual.GetRank(2));
            Assert.AreEqual(expected.GetRank(3), actual.GetRank(3));
            Assert.AreEqual(expected.GetRank(4), actual.GetRank(4));
            Assert.AreEqual(expected.GetRank(5), actual.GetRank(5));
        }
    }
}
#endif