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
    public class TimelineInstantUpdaterTest
    {
        [TestMethod]
        public void One()
        {
            ITimelineUpdater updater = new TimelineInstantUpdater();
            var expected = new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 120000"));

            updater.Add(expected);

            var actual = updater.One();
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void OnEnd()
        {
            var expected = new List<WindowInfoLegacy>
            {
                new WindowInfoLegacy(name: "1", date: TestUtils.MakeDate("000101")),
                new WindowInfoLegacy(name: "2", date: TestUtils.MakeDate("000102")),
                new WindowInfoLegacy(name: "3", date: TestUtils.MakeDate("000103")),
            };
            var actual = new List<WindowInfoLegacy>();
            ITimelineUpdater updater = new TimelineInstantUpdater();
            updater.SetOnEnd((WindowInfoLegacy wi) => { actual.Add(wi); });

            foreach (var item in expected) updater.Add(item);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
#endif