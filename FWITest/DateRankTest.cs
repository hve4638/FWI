﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;

namespace FWITest
{
    [TestClass]
    public class DateRankTest
    {
        [TestMethod]
        public void TestRankName()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120200")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120300")),
            };
            var rank = new DateRank();

            foreach (var item in items) rank.Add(item);

            var expected = new Dictionary<int, RankResult<WindowInfo>>
            {
                { 1, new RankResult<WindowInfo>(item: items[0], duration: TimeSpan.Zero, ranking: 1) },
                { 2, new RankResult<WindowInfo>(item: items[1], duration: TimeSpan.Zero, ranking: 2) },
            };
            var actual = rank.GetRanks(beginRank: 1, endRank: 2);

            AssertEqualsRanking(expected, actual, (a, b) => (a.Item.Name == b.Item.Name));
        }

        [TestMethod]
        public void TestRankDuration()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120200")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120300")),
            };
            var rank = new DateRank();

            foreach (var item in items) rank.Add(item);

            var expected = new Dictionary<int, RankResult<WindowInfo>>
            {
                { 1, new RankResult<WindowInfo>(item: items[1], duration: new TimeSpan(0,2,0), ranking: 1) },
                { 2, new RankResult<WindowInfo>(item: items[0], duration: new TimeSpan(0,1,0), ranking: 2) },
            };
            var actual = rank.GetRanks(beginRank: 1, endRank: 2);

            AssertEqualsRanking(expected, actual, (a, b) => (a.Duration == b.Duration));
        }


        [TestMethod]
        public void TestRankLast()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120200")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120300")),
            };
            var rank = new DateRank();

            foreach (var item in items) rank.Add(item);
            rank.AddLast(Date.MakeDateTime("000101 120330"));

            var expected = new Dictionary<int, RankResult<WindowInfo>>
            {
                { 1, new RankResult<WindowInfo>(item: items[0], duration: new TimeSpan(0,2,0), ranking: 1) },
                { 2, new RankResult<WindowInfo>(item: items[1], duration: new TimeSpan(0,1,0), ranking: 2) },
                { 3, new RankResult<WindowInfo>(item: items[2], duration: new TimeSpan(0,0,30), ranking: 3) },
            };
            var actual = rank.GetRanks(beginRank: 1, endRank: 3);

            AssertEqualsRanking(expected, actual, (a, b) => (a.Item.Name == b.Item.Name && a.Duration == b.Duration));
        }


        [TestMethod]
        public void TestRankLast2()
        {
            WindowInfo item = new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000"));
            var rank = new DateRank();

            rank.Add(item);
            rank.AddLast(Date.MakeDateTime("000101 120100"));
            rank.AddLast(Date.MakeDateTime("000101 120200"));
            rank.AddLast(Date.MakeDateTime("000101 120300"));

            var expected = new Dictionary<int, RankResult<WindowInfo>>
            {
                { 1, new RankResult<WindowInfo>(item: item, duration: new TimeSpan(0,3,0), ranking: 1) },
            };
            var actual = rank.GetRanks(beginRank: 1, endRank: 1);

            AssertEqualsRanking(expected, actual, (a, b) => (a.Item.Name == b.Item.Name && a.Duration == b.Duration));
        }

        [TestMethod]
        public void TestDuplication()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120200")),
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120300")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120400")),
            };
            var rank = new DateRank();

            foreach (var item in items) rank.Add(item);
            rank.AddLast(Date.MakeDateTime("000101 120500"));
        }

        [TestMethod]
        public void TestThrowTimeSequenceException()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120200")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120300")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120100")),
            };
            var rank = new DateRank();

            try
            {
                foreach (var item in items) rank.Add(item);
            }
            catch
            {
                return;
            }

            Assert.Fail();
        }


        [TestMethod]
        public void TestThrowTimeSequenceException2()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"0", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120200")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120300")),
            };
            var rank = new DateRank();
            foreach (var item in items) rank.Add(item);

            try
            {
                rank.AddLast(Date.MakeDateTime("000101 120000"));
            }
            catch
            {
                return;
            }

            Assert.Fail();
        }



        void AssertEqualsRanking<T>(Dictionary<int, T> expected, Dictionary<int, T> actual, Func<T, T, bool> onCompare)
        {
            Assert.AreEqual(expected.Count, actual.Count, "요소 수가 일치하지 않습니다.");
            foreach (var index in expected.Keys)
            {
                var a = expected[index];
                var b = actual[index];
                if (b != null)
                {
                    if (!onCompare(a, b)) Assert.Fail($"Assert.Fail() Key: {index}");
                }
                else
                {
                    Assert.Fail($"Assert.Fail() 인덱스 : {index}");
                }
            }
        }
    }

    [TestClass]
    public class DateRankIOTest
    {
        [TestMethod]
        public void TestExportAndImportContents()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120100")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120200")),
            };
            var eRank = new DateRank();
            foreach (var item in items) eRank.Add(item);

            var rankWriter = new MockStreamWriter();
            var wiWriter = new MockStreamWriter();
            eRank.ExportContents(rankWriter, wiWriter);


            var rankReader = new MockStreamReader(rankWriter);
            var wiReader = new MockStreamReader(wiWriter);
            var iRank = new DateRank();
            iRank.ImportContents(rankReader, wiReader);

            Assert.AreEqual(eRank.GetContentsHash(), iRank.GetContentsHash());
        }

        /// <summary>
        /// 실제 파일시스템을 이용함
        /// </summary>
        [TestMethod]
        public void TestExport()
        {
            WindowInfo[] items =
            {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120100")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120200")),
            };
            var eRank = new DateRank();
            foreach (var item in items) eRank.Add(item);

            var name = "test-save";
            eRank.Export(name);

            var iRank = new DateRank();
            iRank.Import(name);

            Assert.AreEqual(eRank.GetContentsHash(), iRank.GetContentsHash());
        }
    }
}
