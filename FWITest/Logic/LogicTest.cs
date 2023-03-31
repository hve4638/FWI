using HUtility.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FWITest.Logic
{
    [TestClass]
    public class LogicTest
    {
        public DateTime GetDateFuture(DateTime date, TimeSpan interval)
        {
            TimeSpan remainder = interval - TimeSpan.FromTicks(date.TimeOfDay.Ticks % interval.Ticks);
            if (remainder == interval) remainder = TimeSpan.Zero;
            
            return date.Add(remainder);
        }
        public DateTime GetDatePast(DateTime date, TimeSpan interval)
        {
            TimeSpan remainder = interval - TimeSpan.FromTicks(date.TimeOfDay.Ticks % interval.Ticks);
            
            return date.Add(-interval + remainder);
        }
        [TestMethod]
        public void TestGetDateFuture()
        {
            DateTime date, expected, actual;
            TimeSpan interval;
            date = TestUtils.MakeDateTime("100101 123000");
            interval = new TimeSpan(1, 0, 0);
            expected = TestUtils.MakeDateTime("100101 130000");
            actual = GetDateFuture(date, interval);
            Assert.AreEqual(expected, actual);

            date = TestUtils.MakeDateTime("100101 122500");
            interval = new TimeSpan(0, 30, 0);
            expected = TestUtils.MakeDateTime("100101 123000");
            actual = GetDateFuture(date, interval);
            Assert.AreEqual(expected, actual);

            date = TestUtils.MakeDateTime("100101 123000");
            interval = new TimeSpan(0, 30, 0);
            expected = TestUtils.MakeDateTime("100101 123000");
            actual = GetDateFuture(date, interval);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetDatePast()
        {
            DateTime date, expected, actual;
            TimeSpan interval;
            date = TestUtils.MakeDateTime("100101 133000");
            interval = new TimeSpan(1, 0, 0);
            expected = TestUtils.MakeDateTime("100101 130000");
            actual = GetDatePast(date, interval);
            Assert.AreEqual(expected, actual);


            date = TestUtils.MakeDateTime("100101 123500");
            interval = new TimeSpan(0, 30, 0);
            expected = TestUtils.MakeDateTime("100101 123000");
            actual = GetDatePast(date, interval);
            Assert.AreEqual(expected, actual);


            date = TestUtils.MakeDateTime("100101 133000");
            interval = new TimeSpan(0, 30, 0);
            expected = TestUtils.MakeDateTime("100101 133000");
            actual = GetDatePast(date, interval);
            Assert.AreEqual(expected, actual);
        }
    }
}
