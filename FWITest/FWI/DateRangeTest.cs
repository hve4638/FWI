using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;
using System.Text.RegularExpressions;

namespace FWITest.FWI
{
    [TestClass]
    public class DateRangeTest
    {
        [TestMethod]
        public void TestEquals()
        {
            var d1 = MakeRange("000101", "001231");
            var d2 = MakeRange("000101", "001231");
            Assert.AreEqual(d1, d2);
        }

        [TestMethod]
        public void TestEqualsEmptySet()
        {
            var actual = MakeRange("000102", "000101");
            Assert.IsTrue(actual.IsEmpty);

            actual = MakeRange("000101", "000101");
            Assert.IsFalse(actual.IsEmpty);
        }

        [TestMethod]
        public void TestCompareEmptySet()
        {
            var expected = DateRange.Empty;
            var actual = MakeRange("000102", "000101");
            Assert.AreEqual(expected, actual);

            actual = MakeRange("000101", "000101");
            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void TestIntersection()
        {
            var d1 = MakeRange("000101", "000730");
            var d2 = MakeRange("000601", "010101");

            var expected = MakeRange("000601", "000730");
            var actual = d1 & d2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIntersectionBoundary()
        {
            var expected = MakeRange("000630", "000630");
            var actual = MakeRange("000101", "000630") & MakeRange("000630", "001231");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIntersectionEmpty()
        {
            var d1 = MakeRange("000101", "010101");
            var d2 = MakeRange("010102", "020101");

            var expected = DateRange.Empty;
            var actual = d1 & d2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIntersectionSubset()
        {
            var d1 = MakeRange("000101", "001231");
            var d2 = MakeRange("000301", "000601");

            var expected = MakeRange("000301", "000601");
            var actual = d1 & d2;
            Assert.AreEqual(expected, actual);
        }


        DateRange MakeRange(string begin, string end)
        {
            var beginDate = MakeDate(begin);
            var endDate = MakeDate(end);
            return new DateRange(beginDate, endDate);
        }

        DateTime MakeDate(string format) => DateTime.ParseExact(format, "yyMMdd", null);

        [TestMethod]
        public void TestContainsDateTime()
        {
            var dr = MakeRange("000101", "000630");

            Assert.IsTrue(dr.Contains(MakeDate("000401")), "범위 내");
            Assert.IsTrue(dr.Contains(MakeDate("000101")), "범위 내 경계 (1)");
            Assert.IsTrue(dr.Contains(MakeDate("000630")), "범위 내 경계 (2)");

            Assert.IsFalse(dr.Contains(MakeDate("001231")), "범위 외");
        }

        [TestMethod]
        public void TestContainsDateTimeEmpty()
        {
            var dr = DateRange.Empty;

            Assert.IsFalse(dr.Contains(MakeDate("000101")));
            Assert.IsFalse(dr.Contains(MakeDate("000105")));
        }

        [TestMethod]
        public void TestContainsDateRange()
        {
            DateRange dr = MakeRange("000101", "000630");
            DateRange d;

            d = MakeRange("000201", "000401");
            Assert.IsTrue(dr.Contains(d), "범위 내");

            d = MakeRange("000101", "000401");
            Assert.IsTrue(dr.Contains(d), "범위 내 경계 (1)");
            d = MakeRange("000401", "000630");
            Assert.IsTrue(dr.Contains(d), "범위 내 경계 (2)");

            d = MakeRange("000701", "000901");
            Assert.IsFalse(dr.Contains(d), "범위 외");

            d = MakeRange("000101", "000630");
            Assert.IsTrue(dr.Contains(d), "일치");
        }

        [TestMethod]
        public void TestCompareDateTime() {
            var dr = MakeRange("220101", "221231");

            Assert.IsTrue(dr > MakeDate("191231"), "1");
            Assert.IsFalse(dr < MakeDate("191231"), "2");

            Assert.IsTrue(dr < MakeDate("230601"), "3");
            Assert.IsFalse(dr > MakeDate("230601"), "4");
        }
    }

    [TestClass]
    public class DateRankHashTest
    {
        [TestMethod]
        public void TestContentsHash()
        {
            var rank = new DateRank();
            var rank2 = new DateRank();

            Assert.AreEqual(rank.GetContentsHash(), rank2.GetContentsHash());
        }

        [TestMethod]
        public void TestContentsHashNotEqual()
        {
            var rank = new DateRank();
            var rank2 = new DateRank();
            rank2.Add(new NoWindowInfo(date: Date.MakeDateTime("000101 120000")));
            rank2.AddLast(Date.MakeDateTime("000101 120010"));

            Assert.AreNotEqual(rank.GetContentsHash(), rank2.GetContentsHash());
        }

        [TestMethod]
        public void TestContentsHashEqual()
        {
            var rank = new DateRank();
            var rank2 = new DateRank();
            rank.Add(new NoWindowInfo(date: Date.MakeDateTime("000101 120000")));
            rank.AddLast(Date.MakeDateTime("000101 120010"));
            rank2.Add(new NoWindowInfo(date: Date.MakeDateTime("000101 120000")));
            rank2.AddLast(Date.MakeDateTime("000101 120010"));

            Assert.AreEqual(rank.GetContentsHash(), rank2.GetContentsHash());
        }
    }
}
