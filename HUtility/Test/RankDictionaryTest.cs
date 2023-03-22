#if TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HUtility.Test
{
    [TestClass]
    public class RankDictionaryTest
    {
        [TestMethod]
        public void GetAndSet()
        {
            IRankDictionary<int, TimeSpan> rankDict = new RankDictionary<int, TimeSpan>();

            rankDict.Set(10, new TimeSpan(0, 10, 0));
            rankDict.Set(20, new TimeSpan(0, 20, 0));
            rankDict.Set(30, new TimeSpan(0, 30, 0));

            Assert.AreEqual(new TimeSpan(0, 10, 0), rankDict.Get(10));
            Assert.AreEqual(new TimeSpan(0, 20, 0), rankDict.Get(20));
            Assert.AreEqual(new TimeSpan(0, 30, 0), rankDict.Get(30));
        }

        [TestMethod]
        public void Has()
        {
            IRankDictionary<int, TimeSpan> rankDict = new RankDictionary<int, TimeSpan>();

            Assert.IsFalse(rankDict.Has(10));
            Assert.IsFalse(rankDict.Has(20));
            Assert.IsFalse(rankDict.Has(30));
            rankDict.Set(10, new TimeSpan(0, 10, 0));
            rankDict.Set(20, new TimeSpan(0, 20, 0));
            rankDict.Set(30, new TimeSpan(0, 30, 0));

            Assert.IsTrue(rankDict.Has(10));
            Assert.IsTrue(rankDict.Has(20));
            Assert.IsTrue(rankDict.Has(30));
        }

        [TestMethod]
        public void Remove()
        {
            IRankDictionary<int, TimeSpan> rankDict = new RankDictionary<int, TimeSpan>();

            rankDict.Set(10, new TimeSpan(0, 10, 0));
            rankDict.Set(20, new TimeSpan(0, 20, 0));
            rankDict.Set(30, new TimeSpan(0, 30, 0));
            rankDict.Remove(10);
            rankDict.Remove(20);
            rankDict.Remove(30);

            Assert.IsFalse(rankDict.Has(10));
            Assert.IsFalse(rankDict.Has(20));
            Assert.IsFalse(rankDict.Has(30));
        }

        [TestMethod]
        public void Clear()
        {
            IRankDictionary<int, TimeSpan> rankDict = new RankDictionary<int, TimeSpan>();

            rankDict.Set(10, new TimeSpan(0, 10, 0));
            rankDict.Set(20, new TimeSpan(0, 20, 0));
            rankDict.Set(30, new TimeSpan(0, 30, 0));
            rankDict.Clear();

            Assert.IsFalse(rankDict.Has(10));
            Assert.IsFalse(rankDict.Has(20));
            Assert.IsFalse(rankDict.Has(30));
        }

        [TestMethod]
        public void Add()
        {
            IRankDictionary<int, TimeSpan> rankDict = new RankDictionary<int, TimeSpan>();
            rankDict.Add(10, new TimeSpan(0, 10, 0));
            rankDict.Add(20, new TimeSpan(0, 20, 0));
            rankDict.Add(30, new TimeSpan(0, 30, 0));
            rankDict.Add(10, new TimeSpan(0, 20, 0));
            rankDict.Add(20, new TimeSpan(0, 20, 0));
            rankDict.Add(30, new TimeSpan(0, 20, 0));

            Assert.AreEqual(new TimeSpan(0, 30, 0), rankDict.Get(10));
            Assert.AreEqual(new TimeSpan(0, 40, 0), rankDict.Get(20));
            Assert.AreEqual(new TimeSpan(0, 50, 0), rankDict.Get(30));
        }

        [TestMethod]
        public void Copy()
        {
            IRankDictionary<int, TimeSpan> original = new RankDictionary<int, TimeSpan>();
            IRankDictionary<int, TimeSpan> copied = new RankDictionary<int, TimeSpan>();

            original.Set(10, new TimeSpan(0, 10, 0));
            original.Set(20, new TimeSpan(0, 20, 0));
            original.Set(30, new TimeSpan(0, 30, 0));
            original.Copy(ref copied);

            Assert.AreEqual(new TimeSpan(0, 10, 0), copied.Get(10));
            Assert.AreEqual(new TimeSpan(0, 20, 0), copied.Get(20));
            Assert.AreEqual(new TimeSpan(0, 30, 0), copied.Get(30));
        }
    }
}



#endif