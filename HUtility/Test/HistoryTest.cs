#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Test
{
    [TestClass]
    public class HistoryTest
    {
        [TestMethod]
        public void AddAndGetAll()
        {
            var history = new History<int>();
            history += 1;
            history += 2;
            history += 3;

            var expected = new List<int>() { 1, 2, 3 };
            var actual = history.GetAll();

            CollectionAssert.AreEqual(expected, actual);
        } 

        [TestMethod]
        public void Count()
        {
            var history = new History<int>();
            history.Add(1);
            history.Add(2);
            history.Add(3);

            Assert.AreEqual(3, history.Count);
        }

        [TestMethod]
        public void Clear()
        {
            var history = new History<int>();
            history.Add(1);
            history.Add(2);
            history.Add(3);

            Assert.AreEqual(3, history.Count);

            history.Clear();
            Assert.AreEqual(0, history.Count);
        }

        [TestMethod]
        public void GetLast()
        {
            var history = new History<int>();
            history.Add(1);
            Assert.AreEqual(1, history.GetLast());

            history.Add(2);
            Assert.AreEqual(2, history.GetLast());

            history.Add(3);
            Assert.AreEqual(3, history.GetLast());
        }

        [TestMethod]
        public void Indexer()
        {
            var history = new History<int>();
            history.Add(1);
            history.Add(2);
            history.Add(3);

            Assert.AreEqual(1, history[0]);
            Assert.AreEqual(2, history[1]);
            Assert.AreEqual(3, history[2]);
        }
    }
}
#endif