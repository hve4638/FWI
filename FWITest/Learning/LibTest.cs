using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;
using System.Security.Claims;

namespace FWITest.Learning
{
    [TestClass]
    public class LibTest
    {
        [TestMethod]
        public void TestSplitPathSlash()
        {
            AssertSplitPath("", "", "");
            AssertSplitPath("", "name", "name");
            AssertSplitPath("dir", "name", "dir/name");
            AssertSplitPath(@"C:\dir", "name", "C:/dir/name");
        }

        [TestMethod]
        public void TestSplitPathBackSlash()
        {
            AssertSplitPath(@"dir", "name", @"dir\name");
            AssertSplitPath(@"C:\dir", "name", @"C:\dir\name");
        }

        void AssertSplitPath(string expectedDirectory, string expectedName, string fullPath)
        {
            string dir, name;
            (dir, name) = HUtils.SplitPath(fullPath);

            try
            {
                Assert.AreEqual(expectedDirectory, dir);
                Assert.AreEqual(expectedName, name);
            }
            catch (AssertFailedException)
            {
                Assert.Fail($"expected \"{expectedDirectory}\", \"{expectedName}\", but \"{dir}\", \"{name}\"");
            }
        }


        [TestMethod]
        public void TestEncodeDecodeVertical()
        {
            AssertEncodeCorrectly("");
            AssertEncodeCorrectly("name title");
            AssertEncodeCorrectly("name|title");
            AssertEncodeCorrectly("|name|title|");
            AssertEncodeCorrectly("name|||");
            AssertEncodeCorrectly("name|<split>||");
        }

        /// <summary>
        /// 버그 테스트, 해당 테스트가 실패하면 버그가 수정된 것
        /// </summary>
        [TestMethod]
        public void TestFailEncodeDecodeVertical()
        {
            try
            {
                AssertEncodeCorrectly("name|\\<__split>||");
            }
            catch (AssertFailedException)
            {
                return;
            }

            Assert.Fail();
        }

        void AssertEncodeCorrectly(string plain)
        {
            var encoded = HUtils.EncodeVertical(plain);
            var decoded = HUtils.DecodeVertical(encoded);

            Assert.AreEqual(plain, decoded, $"Plain: {plain}");
        }

        [TestMethod]
        public void TestMakeDate()
        {
            DateTime expected = new DateTime(2000, 06, 18);
            DateTime actual = Date.MakeDate("000618");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMakeDateTime()
        {
            DateTime expected = new DateTime(2000, 06, 18, 12, 30, 00);
            DateTime actual = Date.MakeDateTime("000618 123000");

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestStringSplit()
        {
            string str = "a|b|c|";
            string[] split = str.Split('|');
            Assert.AreEqual(4, split.Length);
        }

        [TestMethod]
        public void TestDateCompare()
        {
            DateTime date1 = new DateTime(2000, 6, 10);
            DateTime date2 = new DateTime(2000, 6, 3);

            Assert.IsTrue(date1 > date2);
        }

        [TestMethod]
        public void TestDateTimeEquals() {
            DateTime date1 = new DateTime(2000, 1, 1);
            DateTime date2 = new DateTime(2000, 1, 1);
            Assert.AreEqual(date1, date2);
        }

        [TestMethod]
        public void TestDateTimeParseExact() {
            DateTime expected, actual;
            expected = new DateTime(2000, 12, 13);
            actual = DateTime.ParseExact("001213", "yyMMdd", null);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2000, 12, 13);
            actual = DateTime.ParseExact("00/12/13", "yy/MM/dd", null);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2000, 12, 13);
            actual = DateTime.ParseExact("00 12 13", "yy MM dd", null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAssertCollection() {
            int[] arr = { 1, 2, 3 };
            List<int> list = new List<int>{ 1,2,3 };

            CollectionAssert.AreEqual(arr, list.AsReadOnly());
        }

        [TestMethod]
        public void TestEnumerableReverse()
        {
            int[] arr = { 1, 3, 2, 4, 5 };
            int[] reversed = Enumerable.Reverse(arr).ToArray();

            int[] expected = { 5, 4, 2, 3, 1 };
            CollectionAssert.AreEqual(expected, reversed);
        }

        [TestMethod]
        public void TestDateTimeAddNegative()
        {
            var dt = new DateTime(2022,11,27, 12,30,13);
            var expected = new DateTime(2022,11,27, 12,30,12);
            var actual = dt.AddSeconds(-1);

            Assert.AreEqual(expected, actual);
        }
    }
}
