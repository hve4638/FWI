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
    public class RankTest
    {
        [TestMethod]
        public void SetAndGet()
        {
            var rank = new Rank<string, int>();

            // 최초 정의가 되어있지 않다면 해당 자료형의 default리턴
            rank["A"] += 10;
            rank["A"] += 20;
            rank["A"] += 30;
            rank["B"] += 40;
            rank["B"] += 50;
            rank["B"] += 60;

            Assert.AreEqual(60, rank["A"]);
            Assert.AreEqual(150, rank["B"]);
        }

        [TestMethod]
        public void HasOne()
        {
            var rank = new Rank<string, int>();
            Assert.IsFalse(rank.HasOne());

            rank["A"] = 10;
            Assert.IsTrue(rank.HasOne());
        }

        [TestMethod]
        public void One()
        {
            var rank = new Rank<string, int>();
            Assert.IsFalse(rank.HasOne());

            rank["A"] = 20;
            rank["B"] = 10;
            Assert.AreEqual("A", rank.One());

            rank["B"] = 20;
            rank["A"] = 10;
            Assert.AreEqual("B", rank.One());
        }

        [TestMethod]
        public void TryGetRank()
        {
            var rank = new Rank<string, int>();
            Assert.IsFalse(rank.HasOne());

            rank["A"] = 10;
            rank["B"] = 20;
            rank["C"] = 30;
            rank["D"] = 40;
            rank["E"] = 50;

            string name;
            rank.TryGetRank(1, out name);
            Assert.AreEqual("A", name);
            rank.TryGetRank(2, out name);
            Assert.AreEqual("B", name);
            rank.TryGetRank(3, out name);
            Assert.AreEqual("C", name);
            rank.TryGetRank(4, out name);
            Assert.AreEqual("D", name);
            rank.TryGetRank(5, out name);
            Assert.AreEqual("E", name);

            var isSuccess = rank.TryGetRank(6, out name);
            Assert.IsFalse(isSuccess);
        }

        [TestMethod]
        public void GetRank()
        {
            var rank = new Rank<string, int>();
            Assert.IsFalse(rank.HasOne());

            rank["A"] = 10;
            rank["B"] = 20;
            rank["C"] = 30;
            rank["D"] = 40;
            rank["E"] = 50;

            Assert.AreEqual("A", rank.GetRank(1));
            Assert.AreEqual("B", rank.GetRank(2));
            Assert.AreEqual("C", rank.GetRank(3));
            Assert.AreEqual("D", rank.GetRank(4));
            Assert.AreEqual("E", rank.GetRank(5));
        }

        [TestMethod]
        public void Count()
        {
            var rank = new Rank<string, int>();
            Assert.AreEqual(0, rank.Count);

            rank["A"] = 10;
            Assert.AreEqual(1, rank.Count);

            rank["B"] = 20;
            Assert.AreEqual(2, rank.Count);

            rank["C"] = 30;
            Assert.AreEqual(3, rank.Count);
        }


        [TestMethod]
        public void Clear()
        {
            var rank = new Rank<string, int>();
            Assert.AreEqual(0, rank.Count);

            rank["A"] = 10;
            rank["B"] = 20;
            rank["C"] = 30;
            Assert.AreEqual(3, rank.Count);

            rank.Clear();
            Assert.AreEqual(0, rank.Count);
        }


        [TestMethod]
        public void GetDefault()
        {
            var rankInt = new Rank<int, int>();
            Assert.AreEqual(0, rankInt[0]);

            var rankTimeSpan = new Rank<int, TimeSpan>();
            Assert.AreEqual(TimeSpan.Zero, rankTimeSpan[0]);
        }
    }
}
#endif