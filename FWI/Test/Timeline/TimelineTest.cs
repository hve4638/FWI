#if TEST
using FWI.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUtility.Testing;

namespace FWI.Test
{
    [TestClass]
    public class TimelineTest
    {
        [TestMethod]
        public void GetAllWIs()
        {
            var timeline = new Timeline();

            WindowInfo[] expected = {
                new WindowInfo(name:"1", date: TestUtils.MakeDate("000101")),
                new WindowInfo(name:"2", date: TestUtils.MakeDate("000102")),
                new WindowInfo(name:"3", date: TestUtils.MakeDate("000103")),
            };
            timeline.AddLog(expected);

            var actual = timeline.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAllWIsIgnoreDuplicate()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: TestUtils.MakeDate("000101")),
                new WindowInfo(name:"1", date: TestUtils.MakeDate("000102")),
                new WindowInfo(name:"1", date: TestUtils.MakeDate("000103")),
            };

            timeline.AddLog(items);
            WindowInfo[] expected = { items[0] };

            var actual = timeline.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetWIs()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: TestUtils.MakeDateTime("000102 120000")),
                new WindowInfo(name:"3", date: TestUtils.MakeDateTime("000103 123000")),
                new WindowInfo(name:"4", date: TestUtils.MakeDateTime("000104 123000")),
            };
            timeline.AddLog(items);

            WindowInfo[] expected = {
                items[0],
                items[1],
                items[2],
            };
            var range = TestUtils.MakeDateTimeRange("000101 120000", "000104 120000");
            var actual = timeline.GetWIs(range);
            CollectionAssert.AreEqual(expected, actual);
        }

        // 잘못된 시간 순서로 값을 추가했을때 예외처리
        [TestMethod]
        public void ExceptionTimeSequence()
        {
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(title:"2", name:"", date: TestUtils.MakeDate("000105")),
                new WindowInfo(title:"1", name:"", date: TestUtils.MakeDate("000101")),
            };

            try
            {
                foreach (var item in items) timeline.AddLog(item);
            }
            catch (TimeSequenceException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void AddListenerWithNoInterval()
        {
            WindowInfo expected = new WindowInfo(name: "1", date: TestUtils.MakeDate("000101"));
            WindowInfo actual = null;

            var timeline = new Timeline();
            timeline.SetOnAddListener((WindowInfo item) => { actual = item; });

            timeline.AddLog(expected);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Find()
        {
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name: "0", date: TestUtils.MakeDate("000101")),
                new WindowInfo(name: "1", date: TestUtils.MakeDate("000102")),
                new WindowInfo(name: "2", date: TestUtils.MakeDate("000103")),
                new WindowInfo(name: "3", date: TestUtils.MakeDate("000104")),
            };
            timeline.AddWIs(items);

            var results = new List<(int, DateTime)>()
            {
                (0, TestUtils.MakeDate("000101")),
                (1, TestUtils.MakeDate("000102")),
                (2, TestUtils.MakeDate("000103")),
                (3, TestUtils.MakeDate("000104")),
                (-1, TestUtils.MakeDate("000105")),
            };

            CustomAssert.AllEqual(results,
                (x) => x,
                (date) => timeline.Find(date)
            );
        }

        [TestMethod]
        public void FindNearestPast()
        {
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name: "2", date: TestUtils.MakeDate("000201")),
                new WindowInfo(name: "3", date: TestUtils.MakeDate("000301")),
                new WindowInfo(name: "4", date: TestUtils.MakeDate("000401")),
            };
            timeline.AddWIs(items);

            var results = new List<(int, DateTime)>()
            {
                (-1, TestUtils.MakeDate("000101")),
                (-1, TestUtils.MakeDate("000115")),

                (0, TestUtils.MakeDate("000201")),
                (0, TestUtils.MakeDate("000215")),

                (1, TestUtils.MakeDate("000301")),
                (1, TestUtils.MakeDate("000315")),

                (2, TestUtils.MakeDate("000401")),
                (2, TestUtils.MakeDate("000415")),

                (2, TestUtils.MakeDate("000501")),
            };

            CustomAssert.AllEqual(results,
                (x) => x,
                (date) => timeline.FindNearestPast(date)
            );
        }
        [TestMethod]
        public void FindNearestFuture()
        {
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name: "2", date: TestUtils.MakeDate("000201")),
                new WindowInfo(name: "3", date: TestUtils.MakeDate("000301")),
                new WindowInfo(name: "4", date: TestUtils.MakeDate("000401")),
            };
            timeline.AddWIs(items);

            var results = new List<(int, DateTime)>()
            {
                (0, TestUtils.MakeDate("000101")),
                (0, TestUtils.MakeDate("000115")),

                (0, TestUtils.MakeDate("000201")),
                (1, TestUtils.MakeDate("000215")),

                (1, TestUtils.MakeDate("000301")),
                (2, TestUtils.MakeDate("000315")),

                (2, TestUtils.MakeDate("000401")),
                (-1, TestUtils.MakeDate("000415")),

                (-1, TestUtils.MakeDate("000501")),
            };

            CustomAssert.AllEqual(results,
                (x) => x,
                (date) => timeline.FindNearestPast(date)
            );
        }

        [TestMethod]
        public void FindNearest()
        {
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name: "0", date: TestUtils.MakeDateTime("000101 000000")),
                new WindowInfo(name: "1", date: TestUtils.MakeDateTime("000102 000000")),
                new WindowInfo(name: "2", date: TestUtils.MakeDateTime("000103 000000")),
            };
            timeline.AddWIs(items);

            var results = new List<(int, DateTime)>()
            {
                (0, TestUtils.MakeDateTime("000101 000000")),
                (0, TestUtils.MakeDateTime("000101 060000")),
                (0, TestUtils.MakeDateTime("000101 120000")),
                
                (1, TestUtils.MakeDateTime("000101 180000")),
                (1, TestUtils.MakeDateTime("000102 000000")),
                (1, TestUtils.MakeDateTime("000102 060000")),
                (1, TestUtils.MakeDateTime("000102 120000")),

                (2, TestUtils.MakeDateTime("000102 180000")),
                (2, TestUtils.MakeDateTime("000103 000000")),
                (2, TestUtils.MakeDateTime("000103 000600")),
                (2, TestUtils.MakeDateTime("000103 001200")),
                (2, TestUtils.MakeDateTime("000103 001800")),
                (2, TestUtils.MakeDateTime("000104 000000")),
            };

            CustomAssert.AllEqual(results,
                (result) => {
                    return result;
                },
                (date) =>
                {
                    return timeline.FindNearest(date);
                }
            );
        }
    }
}

#endif