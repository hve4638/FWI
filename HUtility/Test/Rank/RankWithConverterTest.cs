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
    public class RankWithConverterTest
    {
        struct NameValue
        {
            public string name;
        }

        Rank<NameValue, string, int> GetRankWithConvert()
        {
            return new Rank<NameValue, string, int>((NameValue input) => input.name);
        }

        [TestMethod]
        public void SetAndGet()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();

            var nv1 = new NameValue { name = "A" };
            var nv2 = new NameValue { name = "B" };

            rank[nv1] += 10;
            rank[nv1] += 20;
            rank[nv1] += 30;
            rank[nv2] += 40;
            rank[nv2] += 50;
            rank[nv2] += 60;

            Assert.AreEqual(60, rank[nv1]);
            Assert.AreEqual(150, rank[nv2]);
        }

        [TestMethod]
        public void SetAndGetWithMerge()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();
            var nv1 = new NameValue { name = "A" };
            var nv2 = new NameValue { name = "A" };

            rank[nv1] += 10;
            rank[nv1] += 20;
            rank[nv2] += 30;
            rank[nv2] += 40;

            Assert.AreEqual(100, rank[nv1]);
            Assert.AreEqual(100, rank[nv2]);
        }

        [TestMethod]
        public void HasOne()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();
            var nv1 = new NameValue { name = "A" };

            Assert.IsFalse(rank.HasOne());

            rank[nv1] = 10;
            Assert.IsTrue(rank.HasOne());
        }

        [TestMethod]
        public void One()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();
            var nv1 = new NameValue { name = "A" };
            var nv2 = new NameValue { name = "B" };
            Assert.IsFalse(rank.HasOne());

            rank[nv1] = 20;
            rank[nv2] = 10;
            Assert.AreEqual("A", rank.One().name);

            rank[nv1] = 10;
            rank[nv2] = 20;
            Assert.AreEqual("B", rank.One().name);
        }

        [TestMethod]
        public void TryGetRank()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();
            var nv1 = new NameValue { name = "A" };
            var nv2 = new NameValue { name = "B" };
            var nv3 = new NameValue { name = "C" };
            var nv4 = new NameValue { name = "D" };
            var nv5 = new NameValue { name = "E" };
            Assert.IsFalse(rank.HasOne());

            rank[nv1] = 10;
            rank[nv2] = 20;
            rank[nv3] = 30;
            rank[nv4] = 40;
            rank[nv5] = 50;

            NameValue nv;
            rank.TryGetRank(1, out nv);
            Assert.AreEqual("A", nv.name);
            rank.TryGetRank(2, out nv);
            Assert.AreEqual("B", nv.name);
            rank.TryGetRank(3, out nv);
            Assert.AreEqual("C", nv.name);
            rank.TryGetRank(4, out nv);
            Assert.AreEqual("D", nv.name);
            rank.TryGetRank(5, out nv);
            Assert.AreEqual("E", nv.name);

            var isSuccess = rank.TryGetRank(6, out nv);
            Assert.IsFalse(isSuccess);
        }

        [TestMethod]
        public void GetRank()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();
            var nv1 = new NameValue { name = "A" };
            var nv2 = new NameValue { name = "B" };
            var nv3 = new NameValue { name = "C" };
            var nv4 = new NameValue { name = "D" };
            var nv5 = new NameValue { name = "E" };
            Assert.IsFalse(rank.HasOne());

            rank[nv1] = 10;
            rank[nv2] = 20;
            rank[nv3] = 30;
            rank[nv4] = 40;
            rank[nv5] = 50;

            Assert.AreEqual("A", rank.GetRank(1).name);
            Assert.AreEqual("B", rank.GetRank(2).name);
            Assert.AreEqual("C", rank.GetRank(3).name);
            Assert.AreEqual("D", rank.GetRank(4).name);
            Assert.AreEqual("E", rank.GetRank(5).name);
        }


        [TestMethod]
        public void Count()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();
            var nv1 = new NameValue { name = "A" };
            var nv2 = new NameValue { name = "B" };
            var nv3 = new NameValue { name = "C" };
            Assert.AreEqual(0, rank.Count);

            rank[nv1] = 10;
            Assert.AreEqual(1, rank.Count);

            rank[nv2] = 20;
            Assert.AreEqual(2, rank.Count);

            rank[nv3] = 30;
            Assert.AreEqual(3, rank.Count);
        }


        [TestMethod]
        public void Clear()
        {
            IRank<NameValue, int> rank = GetRankWithConvert();
            var nv1 = new NameValue { name = "A" };
            var nv2 = new NameValue { name = "B" };
            var nv3 = new NameValue { name = "C" };
            Assert.AreEqual(0, rank.Count);

            rank[nv1] = 10;
            rank[nv2] = 20;
            rank[nv3] = 30;
            Assert.AreEqual(3, rank.Count);

            rank.Clear();
            Assert.AreEqual(0, rank.Count);
        }
    }
}
#endif