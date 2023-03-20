#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using HUtility.Testing;

namespace HUtility.Test
{
    [TestClass]
    public class DateRangeTest
    {
        [TestMethod]
        public void Equality()
        {
            var range1 = TestUtils.MakeDateRange("000101", "001231");
            var range2 = TestUtils.MakeDateRange("000101", "001231");
            Assert.AreEqual(range1, range2);
        }

        [TestMethod]
        public void IsEmpty()
        {
            var actual = TestUtils.MakeDateRange("000102", "000101");
            Assert.IsTrue(actual.IsEmpty);

            actual = TestUtils.MakeDateRange("000101", "000101");
            Assert.IsFalse(actual.IsEmpty);
        }

        [TestMethod]
        public void SameRangeIsNotEmpty()
        {
            var range = TestUtils.MakeDateRange("000101", "000101");

            Assert.IsFalse(range.IsEmpty);
        }

        [TestMethod]
        public void CompareEmptySet()
        {
            var expected = DateRange.Empty;
            var actual = TestUtils.MakeDateRange("000102", "000101");
            Assert.AreEqual(expected, actual);

            actual = TestUtils.MakeDateRange("000101", "000101");
            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void Intersection()
        {
            var range1 = TestUtils.MakeDateRange("000101", "000730");
            var range2 = TestUtils.MakeDateRange("000601", "010101");

            var expected = TestUtils.MakeDateRange("000601", "000730");
            var actual = range1 & range2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IntersectionBoundary()
        {
            var range1 = TestUtils.MakeDateRange("000101", "000630");
            var range2 = TestUtils.MakeDateRange("000630", "001231");

            var expected = TestUtils.MakeDateRange("000630", "000630");
            var actual = range1 & range2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IntersectionEmpty()
        {
            var range1 = TestUtils.MakeDateRange("000101", "010101");
            var range2 = TestUtils.MakeDateRange("010102", "020101");

            var expected = DateRange.Empty;
            var actual = range1 & range2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IntersectionSubset()
        {
            var range1 = TestUtils.MakeDateRange("000101", "001231");
            var range2 = TestUtils.MakeDateRange("000301", "000601");

            var expected = TestUtils.MakeDateRange("000301", "000601");
            var actual = range1 & range2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ContainsDateTime()
        {
            var range = TestUtils.MakeDateRange("000101", "000630");

            {
                var dates = new List<DateTime>
                {
                    TestUtils.MakeDate("000401"),
                    TestUtils.MakeDate("000101"),
                    TestUtils.MakeDate("000630")
                };
                CustomAssert.AllTrue(dates, (date) => range.Contains(date));
            }
            {
                var date = TestUtils.MakeDate("001231");
                Assert.IsFalse(range.Contains(date), "범위 외");
            }
        }

        [TestMethod]
        public void EmptyContainsDateTime()
        {
            var range = DateRange.Empty;
            var dates = new List<DateTime>
            {
                TestUtils.MakeDate("000101"),
                TestUtils.MakeDate("000105"),
                DateTime.MinValue,
                DateTime.MaxValue,
            };

            CustomAssert.AllFalse(dates, (date) => range.Contains(date));
        }

        [TestMethod]
        public void ContainsDateRange()
        {
            DateRange range = TestUtils.MakeDateRange("000101", "000630");

            {
                var subranges = new List<DateRange>
                {
                    TestUtils.MakeDateRange("000201", "000401"), //범위 내
                    TestUtils.MakeDateRange("000101", "000401"), //왼쪽 경계 일치
                    TestUtils.MakeDateRange("000401", "000630"), //오른쪽 경계 일치
                    TestUtils.MakeDateRange("000101", "000630"), //완전 일치
                };
                CustomAssert.AllTrue(subranges, (subrange) => range.Contains(subrange));
            }
            {
                var subranges = new List<DateRange>
                {
                    TestUtils.MakeDateRange("000701", "000901"),
                };
                CustomAssert.AllFalse(subranges, (subrange) => range.Contains(subrange));
            }
        }

        [TestMethod]
        public void CompareDate()
        {
            var range = TestUtils.MakeDateRange("220101", "221231");

            var beforeDate = TestUtils.MakeDate("191231");
            var afterDate = TestUtils.MakeDate("230601");

            {
                var results = new List<bool>
                {
                    range > beforeDate, // 범위 이전 일치
                    range < afterDate  // 범위 이후 일치
                };
                CustomAssert.AllTrue(results);
            }
            {
                var results = new List<bool>
                {
                    range < beforeDate, // 범위 이전 불일치
                    range > afterDate   // 범위 이후 불일치
                };
                CustomAssert.AllFalse(results);
            }
        }
    }

}

#endif