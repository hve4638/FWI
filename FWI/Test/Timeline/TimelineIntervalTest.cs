#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HUtility.Testing;
using System.Collections.Generic;

namespace FWI.Test
{

    [TestClass]
    public class TimelineIntervalTest
    {
        Timeline GetIntervalTimeline(Func<DateTime> dateTimeDelegate)
        {
            var timeline = new Timeline();
            timeline.SetMenualDateTime(dateTimeDelegate);
            timeline.SetInterval(minutes: 1);

            return timeline;
        }

        [TestMethod]
        public void GetAllWIs()
        {
            DateTime current = TestUtils.MakeDateTime("000101 120000");
            var timeline = GetIntervalTimeline(() => current);

            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"0", date: TestUtils.MakeDateTime("000101 120000")), // [0] 40s V
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120040")), // [0] 10s
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120050")), // [0] 10s
            };

            current = TestUtils.MakeDateTime("000101 120000");
            timeline.AddLog(items);

            current = TestUtils.MakeDateTime("000101 120130");

            var expected = new List<WindowInfoLegacy>() { items[0] };
            var actual = timeline.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAllWIs2()
        {
            DateTime current = TestUtils.MakeDateTime("000101 120000");
            var timeline = GetIntervalTimeline(() => current);

            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name: "0", date: TestUtils.MakeDateTime("000101 120000")), // [0] 10s
                new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 120010")), // [0] 35s V
                new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120045")), // [0] 15s
                new WindowInfoLegacy(name: "3", date: TestUtils.MakeDateTime("000101 120110")), // [1] 60s V
            };

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }
            current = TestUtils.MakeDateTime("000101 120300");

            var expected = new List<WindowInfoLegacy>() { items[1], items[3] };
            var actual = timeline.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }

        // Interval 기간 내에 확정되지 않은 요소를 무시하는지 확인
        [TestMethod]
        public void IgnoreBeforeGivenTime()
        {
            var current = TestUtils.MakeDateTime("000101 120000");
            var timeline = GetIntervalTimeline(() => current);

            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"0", date: TestUtils.MakeDateTime("000101 120001")), // [0] 40s V
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120040")), // [0] 20s 
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120105")), // [1] 5s  
                new WindowInfoLegacy(name:"3", date: TestUtils.MakeDateTime("000101 120150")), // [1] 45s 
            };

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }

            var expected = new List<WindowInfoLegacy>() { items[0] };
            var actual = timeline.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }

        // 한 Period동안 아무값도 들어오지 않았을때 적절히 작동하는지
        [TestMethod]
        public void SkipAPeriod()
        {
            var current = TestUtils.MakeDateTime("000101 120000");
            var timeline = GetIntervalTimeline(() => current);

            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"0", date: TestUtils.MakeDateTime("000101 120000")), // [0] 40s V
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120040")), // [0] 20s+60s+50s V
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120150")), // [2] 10s 
            };

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }
            current = TestUtils.MakeDateTime("000101 120200");

            var expected = new List<WindowInfoLegacy>() { items[0], items[1] };
            var actual = timeline.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IgnoreDuplicate()
        {
            var current = TestUtils.MakeDateTime("000101 120000");
            var timeline = GetIntervalTimeline(() => current);

            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120000")), // [0] 40s V 
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120040")), // [0] 20s+5s
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120105")), // [1] 55s V       (but duplicate)
            };

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }
            current = TestUtils.MakeDateTime("000101 120200");

            var expected = new List<WindowInfoLegacy>() { items[0] };
            var actual = timeline.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }
    }

}

#endif