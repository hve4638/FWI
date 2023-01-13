﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;

namespace FWITest
{
    [TestClass]
    public class TimelineDateUpdaterTest
    {

        [TestMethod]
        public void TestGetOne()
        {
            var begin = new DateTime(2022, 06, 01, 12, 00, 00);
            var end = new DateTime(2022, 06, 01, 12, 01, 00);
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfo[] items = {
                new WindowInfo(title:"A", name:"a", date: new DateTime(2022,06,01, 12,00,01)),
                new WindowInfo(title:"B", name:"b", date: new DateTime(2022,06,01, 12,00,40)),
                new WindowInfo(title:"C", name:"c", date: new DateTime(2022,06,01, 12,00,50)),
            };

            foreach (var item in items) updater.Add(item);

            var expected = items[0];
            var actual = updater.One();
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void TestGetOneNoItem()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
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
        public void TestOnEndListener()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            WindowInfo[] items =
            {
                new WindowInfo(name: "1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120050")),
                new WindowInfo(name: "3", date: Date.MakeDateTime("000101 120110")),
            };

            WindowInfo[] expected =
            {
                items[0],
            };
            var actual = new List<WindowInfo>();

            TimelineUpdater updater = new TimelineDateUpdater(begin: begin, end: end);
            updater.SetOnEnd((WindowInfo wi) => { actual.Add(wi); });

            foreach (var item in items) updater.Add(item);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestFillOnEndListener()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            var item = new WindowInfo(name: "0", date: Date.MakeDateTime("000101 120005"));

            var expected = item;
            WindowInfo actual = null;

            var updater = new TimelineDateUpdater(begin: begin, end: end);
            updater.SetOnEnd((WindowInfo wi) => { actual = wi; });
            updater.Add(item);

            updater.FillLast();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetTotal()
        {
            
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfo[] array = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 115955")), 
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120030")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120115")),
            };

            var total = new TimeSpan(0, 0, 0);
            foreach (var item in array) total += updater.Add(item);

            var expected = new TimeSpan(0, 1, 0);
            Assert.AreEqual(expected, total);
        }


        [TestMethod]
        public void TestGetTime()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120010")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120030")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120055")),
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
        public void TestGetTimeWithInitWI()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120010")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120030")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120055")),
            };
            WindowInfo initWI = new WindowInfo(name: "0", date: Date.MakeDateTime("000101 115930"));

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
        public void TestFillRemainTime()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120020")),
            };
            foreach (var item in items) updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = items[1];
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestInitWI()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            var item = new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120045"));
            updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = item;
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestInitWI2()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            var initWI = new WindowInfo(name: "1", date: Date.MakeDateTime("000101 115000"));

            var updater = new TimelineDateUpdater(begin: begin, end: end, initWI: initWI);
            var item = new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120015"));
            updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = item;
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestInitWI3()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            var initWI = new WindowInfo(name: "1", date: Date.MakeDateTime("000101 120005"));

            var updater = new TimelineDateUpdater(begin: begin, end: end, initWI: initWI);
            var item = new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120015"));
            updater.Add(item);

            updater.FillLast();

            Assert.IsTrue(updater.IsEnd());

            var expected = item;
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestNoInitWI()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");

            var updater = new TimelineDateUpdater(begin: begin, end: end);
            var item = new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120045"));
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
        public void TestIsAddedFirstWI()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            var updater = new TimelineDateUpdater(begin: begin, end: end);
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120020")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120040")),
            };
            foreach (var item in items) updater.Add(item);
            updater.FillLast();

            var expected = items[0];
            var actual = updater.One();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestReset()
        {
            var begin = Date.MakeDateTime("000101 120000");
            var end = Date.MakeDateTime("000101 120100");
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120020")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120105")),
            };
            WindowInfo[] items2 = {
                new WindowInfo(name:"a", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"b", date: Date.MakeDateTime("000101 120050")),
                new WindowInfo(name:"c", date: Date.MakeDateTime("000101 120100")),
            };
            WindowInfo expected, actual;

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

        /// <summary>
        /// 실사용에 가장 가까운 방법
        /// </summary>
        [TestMethod]
        public void TestActual()
        {
            int callbackCount = 0;
            DateTime current = Date.MakeDateTime("000101 120000");
            TimeSpan interval = new TimeSpan(0, 1, 0);
            WindowInfo[] items = {
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120020")),

                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120105")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120110")),
                new WindowInfo(name:"4", date: Date.MakeDateTime("000101 120120")),
                new WindowInfo(name:"5", date: Date.MakeDateTime("000101 120155")),

                new WindowInfo(name:"6", date: Date.MakeDateTime("000101 120245")),

                new WindowInfo(name:"7", date: Date.MakeDateTime("000101 120301")),

                new WindowInfo(name:"8", date: Date.MakeDateTime("000101 120610")),
            };
            WindowInfo[] expected =
            {
                items[1],
                items[4],
                items[5],
                items[7],
            };
            var actual = new List<WindowInfo>();
            WindowInfo last = new NoWindowInfo();
            var updater = new TimelineDateUpdater(begin: current, end: current+interval);
            updater.SetOnEnd((WindowInfo wi) =>
            {
                var date = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, 0);
                updater.Reset(begin: date, end: date+interval, initWI:updater.Last);
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


    [TestClass]
    public class TimelineInstantUpdaterTest
    {
        [TestMethod]
        public void TestGetOne()
        {
            TimelineUpdater updater = new TimelineInstantUpdater();
            var expected = new WindowInfo(name: "1", date: Date.MakeDateTime("000101 120000"));

            updater.Add(expected);

            var actual = updater.One();
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void TestOnEnd()
        {
            var expected = new List<WindowInfo>
            {
                new WindowInfo(name: "1", date: Date.MakeDate("000101")),
                new WindowInfo(name: "2", date: Date.MakeDate("000102")),
                new WindowInfo(name: "3", date: Date.MakeDate("000103")),
            };
            var actual = new List<WindowInfo>();
            TimelineUpdater updater = new TimelineInstantUpdater();
            updater.SetOnEnd((WindowInfo wi) => { actual.Add(wi); });

            foreach (var item in expected) updater.Add(item);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
