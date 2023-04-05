#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;
using HUtility.Testing;

namespace FWI.Test
{
    [TestClass]
    public class RankTest
    {
        [TestMethod]
        public void TestRankEqual()
        {
            var list = new List<(WindowInfoLegacy wi, TimeSpan time)>
            {
                (new WindowInfoLegacy(title: "abc1", name: "abc1.exe"), new TimeSpan(00, 20, 10)),
                (new WindowInfoLegacy(title: "abc2", name: "abc2.exe"), new TimeSpan(00, 50, 00)),
            };

            Rank rank = new Rank();
            Rank rank2 = new Rank();
            AddToRank(rank, list);
            AddToRank(rank2, list);

            Assert.AreEqual(rank, rank2);
        }

        void AddToRank(Rank rank, List<(WindowInfoLegacy wi, TimeSpan time)> items)
        {
            foreach(var (wi, time) in items) rank.Add(wi, time);
        }

        [TestMethod]
        public void TestRankOne()
        {
            Rank rank = new Rank();
            WindowInfoLegacy wi = new WindowInfoLegacy(title: "abc", name: "abc.exe");
            rank.Add(wi, new TimeSpan(0, 20, 10));

            var res = rank.One();
            Assert.AreEqual(wi.Name, res);
        }

        [TestMethod]
        public void TestRankAddMulti()
        {
            Rank rank = new Rank();
            WindowInfoLegacy wi1 = new WindowInfoLegacy(title: "abc", name: "abc.exe");
            WindowInfoLegacy wi2 = new WindowInfoLegacy(title: "notepad", name: "notepade.exe");
            rank.Add(wi1, new TimeSpan(0, 10, 00));
            rank.Add(wi2, new TimeSpan(0, 5, 00));

            string expected;
            expected = rank.GetRank(1);
            Assert.AreEqual(wi1.Name, expected);

            expected = rank.GetRank(2);
            Assert.AreEqual(wi2.Name, expected);
        }

        [TestMethod]
        public void TestTryGetRank()
        {
            Rank rank = new Rank();
            rank.Add("1st", new TimeSpan(0, 10, 00));
            rank.Add("2nd", new TimeSpan(0, 5, 00));

            string expected;
            rank.TryGetRank(1, out expected);
            Assert.AreEqual("1st", expected);

            rank.TryGetRank(2, out expected);
            Assert.AreEqual("2nd", expected);
        }

        [TestMethod]
        public void TestTryGetRankTime()
        {
            Rank rank = new Rank();
            rank.Add("1st", new TimeSpan(0, 10, 00));
            rank.Add("2nd", new TimeSpan(0, 5, 00));

            string name;
            TimeSpan time;
            rank.TryGetRank(1, out name, out time);
            Assert.AreEqual("1st", name);
            Assert.AreEqual(new TimeSpan(0, 10, 00), time);

            rank.TryGetRank(2, out name, out time);
            Assert.AreEqual("2nd", name);
            Assert.AreEqual(new TimeSpan(0, 5, 00), time);
        }

        [TestMethod]
        public void TestTryGetRankFail()
        {
            Rank rank = new Rank();
            rank.Add("1st", new TimeSpan(0, 10, 00));
            rank.Add("2nd", new TimeSpan(0, 5, 00));

            bool actual;
            actual = rank.TryGetRank(1, out _);
            Assert.AreEqual(true, actual, "#1");
            actual = rank.TryGetRank(2, out _);
            Assert.AreEqual(true, actual, "#2");
            actual = rank.TryGetRank(3, out _);
            Assert.AreEqual(false, actual, "#3");
        }


        [TestMethod]
        public void TestRankConflict()
        {
            Rank rank = new Rank();
            rank.Add("1st", new TimeSpan(0, 10, 00));
            rank.Add("1st-2", new TimeSpan(0, 10, 00));
            rank.Add("3rd", new TimeSpan(0, 5, 00));

            var expected = rank.GetRank(3);
            Assert.AreEqual("3rd", expected);
        }


        [TestMethod]
        public void TestRankConflict2()
        {
            Rank rank = new Rank();
            rank.Add("1st", new TimeSpan(0, 10, 00));
            rank.Add("2nd", new TimeSpan(0, 10, 00));
            rank.Add("3rd", new TimeSpan(0, 5, 00));

            string expected;
            expected = rank.GetRank(1);
            Assert.IsTrue(expected == "1st" || expected == "2nd");
            expected = rank.GetRank(2);
            Assert.IsTrue(expected == "1st" || expected == "2nd");
        }

        [TestMethod]
        public void TestRankCount()
        {
            Rank rank = new Rank();
            Assert.AreEqual(0, rank.Count, "#1");

            rank.Add("1st", new TimeSpan(0, 5, 00));
            Assert.AreEqual(1, rank.Count, "#2");

            rank.Add("2nd", new TimeSpan(0, 3, 00));
            Assert.AreEqual(2, rank.Count, "#3");

            rank.Add("3rd", new TimeSpan(0, 5, 00));
            Assert.AreEqual(3, rank.Count, "#4");

            rank.Add("3rd", new TimeSpan(0, 8, 00));
            Assert.AreEqual(3, rank.Count, "#5");
        }

        [TestMethod]
        public void TestRankException()
        {
            Rank rank = new Rank();
            try
            {
                rank.One();
            }
            catch (RankNotFoundException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TestAddRank()
        {
            Rank rank1 = new Rank();
            rank1.Add("1st", new TimeSpan(0, 5, 00));
            rank1.Add("2nd", new TimeSpan(0, 2, 00));
            rank1.Add("3rd", new TimeSpan(0, 1, 00));

            Rank rank2 = new Rank();
            rank1.Add("1st", new TimeSpan(0, 0, 45));
            rank1.Add("3rd", new TimeSpan(0, 0, 30));
            rank1.Add("4th", new TimeSpan(0, 0, 10));

            Rank expectedRank = new Rank();
            expectedRank.Add("1st", new TimeSpan(0, 5, 45));
            expectedRank.Add("2nd", new TimeSpan(0, 2, 00));
            expectedRank.Add("3rd", new TimeSpan(0, 1, 30));
            expectedRank.Add("4th", new TimeSpan(0, 0, 10));

            Rank actualRank = new Rank();
            actualRank.Add(rank1);
            actualRank.Add(rank2);

            Assert.AreEqual(expectedRank, actualRank);
        }
    }

    [TestClass]
    public class RankIOTest
    {
        [TestMethod]
        public void TestImportExport()
        {
            Rank erank = new Rank();
            erank.Add("1st", new TimeSpan(0, 10, 00));
            erank.Add("2nd", new TimeSpan(0, 15, 00));
            erank.Add("3rd", new TimeSpan(0, 05, 00));

            var writer = new MockStreamWriter();
            erank.Export(writer);

            var reader = new MockStreamReader(writer.GetOutputAsString());
            Rank irank = new Rank();
            irank.Import(reader);

            Assert.AreEqual(erank, irank);
        }
    }

    [TestClass]
    public class RankHashTest
    {
        [TestMethod]
        public void TestHash()
        {
            var rank = new Rank();
            var rank2 = new Rank();

            Assert.AreEqual(rank.GetContentsHash(), rank2.GetContentsHash());
        }

        [TestMethod]
        public void TestHashNotEqual()
        {
            var rank = new Rank();
            var rank2 = new Rank();
            rank2.Add(new WindowInfoLegacy(name:"1"), new TimeSpan(0, 0, 10));

            Assert.AreNotEqual(rank.GetContentsHash(), rank2.GetContentsHash());
        }
    }
}
#endif