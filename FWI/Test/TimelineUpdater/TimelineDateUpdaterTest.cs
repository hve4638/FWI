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
    public class TimelineDateUpdaterTest
    {
        [TestMethod]
        public void One()
        {
            var begin = new DateTime(2022, 06, 01, 12, 00, 00);
            var end = new DateTime(2022, 06, 01, 12, 01, 00);
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(title:"A", name:"a", date: new DateTime(2022,06,01, 12,00,01)),
                new WindowInfoLegacy(title:"B", name:"b", date: new DateTime(2022,06,01, 12,00,40)),
                new WindowInfoLegacy(title:"C", name:"c", date: new DateTime(2022,06,01, 12,00,50)),
            };

            foreach (var item in items) updater.Add(item);

            var expected = items[0];
            var actual = updater.One();
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void OneNoItem()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);

            try
            {
                updater.One();
            }
            catch (RankNotFoundException)
            {
                return;
            }
            Assert.Fail();
        }


        [TestMethod]
        public void OnEndListener()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            WindowInfoLegacy[] items =
            {
                new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120050")),
                new WindowInfoLegacy(name: "3", date: TestUtils.MakeDateTime("000101 120110")),
            };

            WindowInfoLegacy[] expected =
            {
                items[0],
            };
            var actual = new List<WindowInfoLegacy>();

            ITimelineUpdater updater = new TimelineDateUpdater(begin: begin, end: end);
            updater.SetOnEnd((WindowInfoLegacy wi) => { actual.Add(wi); });

            foreach (var item in items) updater.Add(item);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FillOnEndListener()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var item = new WindowInfoLegacy(name: "0", date: TestUtils.MakeDateTime("000101 120005"));

            var expected = item;
            WindowInfoLegacy actual = null;

            var updater = new TimelineDateUpdater(begin: begin, end: end);
            updater.SetOnEnd((WindowInfoLegacy wi) => { actual = wi; });
            updater.Add(item);

            updater.FillLast();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetTotal()
        {

            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfoLegacy[] array = {
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 115955")),
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120030")),
                new WindowInfoLegacy(name:"3", date: TestUtils.MakeDateTime("000101 120115")),
            };

            var total = new TimeSpan(0, 0, 0);
            foreach (var item in array) total += updater.Add(item);

            var expected = new TimeSpan(0, 1, 0);
            Assert.AreEqual(expected, total);
        }


        [TestMethod]
        public void GetTime()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120010")),
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120030")),
                new WindowInfoLegacy(name:"3", date: TestUtils.MakeDateTime("000101 120055")),
            };

            TimeSpan expected, actual;
            var updater = new TimelineDateUpdater(begin: begin, end: end);

            expected = new TimeSpan(0, 0, 0);
            actual = updater.Add(items[0]);
            Assert.AreEqual(expected, actual);

            expected = new TimeSpan(0, 0, 20);
            actual = updater.Add(items[1]);
            Assert.AreEqual(expected, actual);

            expected = new TimeSpan(0, 0, 25);
            actual = updater.Add(items[2]);
            Assert.AreEqual(expected, actual);

            expected = new TimeSpan(0, 0, 5);
            actual = updater.FillLast();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetTimeWithInitWI()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120010")),
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120030")),
                new WindowInfoLegacy(name:"3", date: TestUtils.MakeDateTime("000101 120055")),
            };
            WindowInfoLegacy initWI = new WindowInfoLegacy(name: "0", date: TestUtils.MakeDateTime("000101 115930"));

            TimeSpan expected, actual;
            var updater = new TimelineDateUpdater(begin: begin, end: end, initWI: initWI);

            expected = new TimeSpan(0, 0, 10);
            actual = updater.Add(items[0]);
            Assert.AreEqual(expected, actual);

            expected = new TimeSpan(0, 0, 20);
            actual = updater.Add(items[1]);
            Assert.AreEqual(expected, actual);

            expected = new TimeSpan(0, 0, 25);
            actual = updater.Add(items[2]);
            Assert.AreEqual(expected, actual);

            expected = new TimeSpan(0, 0, 5);
            actual = updater.FillLast();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// FillLast()로 마지막 넣은값으로 나머지 시간을 모두 채우기
        /// </summary>
        [TestMethod]
        public void FillRemainTime()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120020")),
            };
            foreach (var item in items) updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = items[1];
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InitWI()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            var item = new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120045"));
            updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = item;
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InitWI2()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var initWI = new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 115000"));

            var updater = new TimelineDateUpdater(begin: begin, end: end, initWI: initWI);
            var item = new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120015"));
            updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = item;
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InitWI3()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var initWI = new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 120005"));

            var updater = new TimelineDateUpdater(begin: begin, end: end, initWI: initWI);
            var item = new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120015"));
            updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = item;
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NoInitWI()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");

            var updater = new TimelineDateUpdater(begin: begin, end: end);
            var item = new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120045"));
            updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = item;
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// 같은 windowInfo가 여러번 들어와도 최초로 들어온 date를 가진 windowInfo를 리턴함
        /// </summary>
        [TestMethod]
        public void IsAddedFirstWI()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120020")),
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120040")),
            };
            foreach (var item in items) updater.Add(item);
            updater.FillLast();

            var expected = items[0];
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Reset()
        {
            var begin = TestUtils.MakeDateTime("000101 120000");
            var end = TestUtils.MakeDateTime("000101 120100");
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120020")),
                new WindowInfoLegacy(name:"3", date: TestUtils.MakeDateTime("000101 120105")),
            };
            WindowInfoLegacy[] items2 = {
                new WindowInfoLegacy(name:"a", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name:"b", date: TestUtils.MakeDateTime("000101 120050")),
                new WindowInfoLegacy(name:"c", date: TestUtils.MakeDateTime("000101 120100")),
            };
            WindowInfoLegacy expected, actual;

            var updater = new TimelineDateUpdater(begin: begin, end: end);
            foreach (var item in items) updater.Add(item);
            expected = items[1];
            actual = updater.One();
            Assert.AreEqual(expected, actual, "first");

            updater.Reset(begin: begin, end: end);
            foreach (var item in items2) updater.Add(item);
            expected = items2[0];
            actual = updater.One();
            Assert.AreEqual(expected, actual, "second");
        }

        /// 실사용에 가장 가까운 방법
        [TestMethod]
        public void Actual()
        {
            int callbackCount = 0;
            DateTime current = TestUtils.MakeDateTime("000101 120000");
            TimeSpan interval = new TimeSpan(0, 1, 0);
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name:"0", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name:"1", date: TestUtils.MakeDateTime("000101 120020")),

                new WindowInfoLegacy(name:"2", date: TestUtils.MakeDateTime("000101 120105")),
                new WindowInfoLegacy(name:"3", date: TestUtils.MakeDateTime("000101 120110")),
                new WindowInfoLegacy(name:"4", date: TestUtils.MakeDateTime("000101 120120")),
                new WindowInfoLegacy(name:"5", date: TestUtils.MakeDateTime("000101 120155")),

                new WindowInfoLegacy(name:"6", date: TestUtils.MakeDateTime("000101 120245")),

                new WindowInfoLegacy(name:"7", date: TestUtils.MakeDateTime("000101 120301")),

                new WindowInfoLegacy(name:"8", date: TestUtils.MakeDateTime("000101 120610")),
            };
            WindowInfoLegacy[] expected =
            {
                items[1],
                items[4],
                items[5],
                items[7],
            };
            var actual = new List<WindowInfoLegacy>();
            WindowInfoLegacy last = new NoWindowInfoLegacy();
            var updater = new TimelineDateUpdater(begin: current, end: current + interval);
            updater.SetOnEnd((WindowInfoLegacy wi) =>
            {
                var date = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, 0);
                updater.Reset(begin: date, end: date + interval, initWI: updater.Last);
                updater.Add(last);

                actual.Add(wi);
                callbackCount++;
            });

            foreach (var item in items)
            {
                last = item;
                current = item.Date;
                updater.Add(item);
            }

            Assert.AreEqual(expected.Length, callbackCount);
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
#endif