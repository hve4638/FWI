using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using FWI;
using System.IO;
using System.Linq;

namespace FWITest
{
    [TestClass]
    public class WindowInfoTest
    {
        [TestMethod]
        public void TestHashNotEqual()
        {
            var wi = new WindowInfo(name: "1");
            var wi2 = new WindowInfo(name: "1");

            Assert.AreNotEqual(wi.GetHashCode(), wi2.GetHashCode());
        }

        [TestMethod]
        public void TestEqual()
        {
            var wi = new WindowInfo(name: "1", date: Date.MakeDate("000101"));
            var wi2 = new WindowInfo(name: "1", date: Date.MakeDate("000101"));
            var wi3 = new WindowInfo(name: "2", date: Date.MakeDate("000101"));

            Assert.AreEqual(wi, wi2);
            Assert.AreNotEqual(wi, wi3);
        }

        [TestMethod]
        public void TestContentsHashEqual()
        {
            var wi = new WindowInfo(name: "1", date: Date.MakeDate("000101"));
            var wi2 = new WindowInfo(name: "1", date: Date.MakeDate("000101"));
            var wi3 = new WindowInfo(name: "5", date: Date.MakeDate("000101"));

            Assert.AreEqual(wi.GetContentsHashCode(), wi2.GetContentsHashCode());
            Assert.AreNotEqual(wi.GetContentsHashCode(), wi3.GetContentsHashCode());
        }

        [TestMethod]
        public void TestNoWIHash()
        {
            var wi = new NoWindowInfo();
            var wi2 = new NoWindowInfo();

            Assert.AreEqual(wi, wi2);
        }

        [TestMethod]
        public void TestNoWIContentsHash()
        {
            var wi = new NoWindowInfo();
            var wi2 = new NoWindowInfo();

            Assert.AreEqual(wi.GetContentsHashCode(), wi2.GetContentsHashCode());
        }
    }

    [TestClass]
    public class WindowInfoEncodingTest
    {
        [TestMethod]
        public void TestEncodeDecode()
        {
            DateTime date = new DateTime(2000, 1, 1, 12, 30, 12);
            WindowInfo wi = new WindowInfo(title: "hello", name: "hello.exe", alias: "hi", date: date);
            string encoded = wi.Encoding();
            WindowInfo decodedWi = WindowInfo.Decode(encoded);

            Assert.IsTrue(wi == decodedWi);
        }
        [TestMethod]
        public void TestValidEqualOperator()
        {
            DateTime date = new DateTime(2000, 1, 1, 12, 30, 12);
            WindowInfo wi1 = new WindowInfo(title: "hello", name: "hello.exe", alias: "hi", date: date);
            WindowInfo wi2 = new WindowInfo(title: "hello", name: "hello.exe", alias: "hi", date: date);

            Assert.AreEqual(wi1, wi2);
        }
        [TestMethod]
        public void TestMapContains()
        {
            Dictionary<WindowInfo, bool> dict = new Dictionary<WindowInfo, bool>();
            WindowInfo w1, w2;
            var date = new DateTime();
            w1 = new WindowInfo(title: "hello", name: "hello.exe", alias: "hi", date: date);
            w2 = new WindowInfo(title: "hello", name: "hello.exe", alias: "hi", date: date);

            dict.Add(w1, false);

            Assert.IsTrue(dict.ContainsKey(w1), "a");
            Assert.IsFalse(dict.ContainsKey(w2), "b");
        }

        [TestMethod]
        public void TestEndodeSplitWord()
        {
            DateTime date = Date.MakeDateTime("000101 123000");
            WindowInfo wi = new WindowInfo(name: "Split|Split", alias: "hi", date: date);
            string encoded = wi.Encoding();
            WindowInfo decodedWi = WindowInfo.Decode(encoded);

            Assert.IsTrue(wi == decodedWi);
        }
    }
}
