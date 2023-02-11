using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;
using System.Collections.ObjectModel;
using System.IO;
using FWI.Exceptions;

namespace FWITest.FWITimeline
{
    [TestClass]
    public class TimelineHashTest
    {
        [TestMethod]
        public void TestHash()
        {
            var timeline = new Timeline();
            var timeline2 = new Timeline();

            Assert.AreNotEqual(timeline.GetHashCode(), timeline2.GetHashCode());
        }

        [TestMethod]
        public void TestContentsHashEqual()
        {
            var timeline = new Timeline();
            var timeline2 = new Timeline();

            Assert.AreEqual(timeline.GetContentsHashCode(), timeline2.GetContentsHashCode());
        }

        [TestMethod]
        public void TestContentsHashNotEqual()
        {
            var timeline = new Timeline();
            var timeline2 = new Timeline();
            timeline2.AddLog(new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")));
            timeline2.AddLog(new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120005")));
            
            Assert.AreNotEqual(timeline.GetContentsHashCode(), timeline2.GetContentsHashCode());
        }
    }

    [TestClass]
    public class TimelineTest
    {
        [TestMethod]
        public void TestGetList()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDate("000101")),
                new WindowInfo(name:"2", date: Date.MakeDate("000102")),
                new WindowInfo(name:"3", date: Date.MakeDate("000103")),
            };

            timeline.AddLog(items);
            var actual = timeline.GetTimeline();
            CollectionAssert.AreEqual(items, actual);
        }

        [TestMethod]
        public void TestIgnoreDuplicate()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDate("000101")),
                new WindowInfo(name:"1", date: Date.MakeDate("000102")),
                new WindowInfo(name:"1", date: Date.MakeDate("000103")),
            };

            timeline.AddLog(items);
            WindowInfo[] expected =
            {
                items[0]
            };
            var actual = timeline.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetListRange()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000102 120000")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000103 123000")),
                new WindowInfo(name:"4", date: Date.MakeDateTime("000104 123000")),
            };
            timeline.AddLog(items);

            WindowInfo[] expected = {
                items[0],
                items[1],
                items[2],
            };
            var range = Date.MakeDateTimeRange("000101 120000", "000104 120000");
            var actual = timeline.GetTimeline(range);
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// 잘못된 시간 순서로 값을 추가했을때 예외처리
        /// </summary>
        [TestMethod]
        public void TestTimeSequence()
        {
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(title:"2", name:"", date: Date.MakeDate("000105")),
                new WindowInfo(title:"1", name:"", date: Date.MakeDate("000101")),
            };

            try
            {
                foreach(var item in items) timeline.AddLog(item);
            }
            catch (TimeSequenceException)
            {
                return;
            }
            Assert.Fail();
        }
    }

    [TestClass]
    public class TimelineIntervalTest
    {

        [TestMethod]
        public void TestGetRankTimeline()
        {
            var current = Date.MakeDateTime("000101 120000");
            var timeline = new Timeline();
            WindowInfo[] array = {
                new WindowInfo(title:"A", name:"a", date: Date.MakeDateTime("000101 120001")),
                new WindowInfo(title:"B", name:"b", date: Date.MakeDateTime("000101 120040")),
                new WindowInfo(title:"C", name:"c", date: Date.MakeDateTime("000101 120050")),
            };

            timeline.SetDateTimeDelegate(() => current);
            timeline.SetInterval(minutes: 1);

            current = Date.MakeDateTime("000101 120000");
            timeline.AddLog(array);

            current = Date.MakeDateTime("000101 120105");

            WindowInfo[] expected = {
                array[0],
            };
            var actual = timeline.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetListInterval()
        {
            DateTime current = Date.MakeDateTime("000101 120000");
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name: "1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120010")),
                new WindowInfo(name: "3", date: Date.MakeDateTime("000101 120045")),
                new WindowInfo(name: "4", date: Date.MakeDateTime("000101 120110")),
            };

            timeline.SetDateTimeDelegate(() => current);
            timeline.SetInterval(1);

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }
            current = Date.MakeDateTime("000101 120300");

            WindowInfo[] expected = { items[1], items[3] };
            var actual = timeline.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }


        /// <summary>
        /// Interval 기간 내에 확정되지 않은 요소를 무시하는지 확인
        /// </summary>
        [TestMethod]
        public void TestGetListInterval2()
        {
            var current = Date.MakeDateTime("000101 120000");
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120001")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120040")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120105")),
                new WindowInfo(name:"4", date: Date.MakeDateTime("000101 120150")),
            };

            timeline.SetDateTimeDelegate(() => current);
            timeline.SetInterval(minutes: 1);

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }

            WindowInfo[] expected = { items[0], };
            var actual = timeline.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetListInterval3()
        {
            var current = Date.MakeDateTime("000101 120000");
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120040")),
                new WindowInfo(name:"3", date: Date.MakeDateTime("000101 120150")),
            };

            timeline.SetDateTimeDelegate(() => current);
            timeline.SetInterval(minutes: 1);

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }
            current = Date.MakeDateTime("000101 120200");

            WindowInfo[] expected = { items[0], items[1] };
            var actual = timeline.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIgnoreDuplicate()
        {
            var current = Date.MakeDateTime("000101 120000");
            var timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name:"2", date: Date.MakeDateTime("000101 120040")),
                new WindowInfo(name:"1", date: Date.MakeDateTime("000101 120105")),
            };

            timeline.SetDateTimeDelegate(() => current);
            timeline.SetInterval(minutes: 1);

            foreach (var wi in items)
            {
                current = wi.Date;
                timeline.AddLog(wi);
            }
            current = Date.MakeDateTime("000101 120200");

            WindowInfo[] expected = { items[0], };
            var actual = timeline.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }
    }


    [TestClass]
    public class TimelineListenerTest
    {
        [TestMethod]
        public void TestAddListenerNoInterval()
        {
            WindowInfo expected;
            WindowInfo actual;
            var timeline = new Timeline();
            timeline.SetOnAddListener((WindowInfo item) => { actual = item; });

            expected = new WindowInfo(name: "1", date: Date.MakeDate("000101"));
            actual = null;
            timeline.AddLog(expected);
            Assert.AreEqual(expected, actual);
        }
    }


    [TestClass]
    public class TimelineDateTimeDelegateTest
    {
        [TestMethod]
        public void TestDelegateTimeline()
        {
            Timeline timeline = new Timeline();
            timeline.SetDateTimeDelegate(() => DateTime.MinValue);

            var expected = DateTime.MinValue;
            var actual = timeline.GetCurrentDateTime();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDelegateClosure()
        {
            DateTime date = DateTime.MinValue;
            Timeline timeline = new Timeline();
            timeline.SetDateTimeDelegate(() => date);

            date = new DateTime(2000, 1, 1);
            var expected = date;
            var actual = timeline.GetCurrentDateTime();
            Assert.AreEqual(expected, actual);

            date = new DateTime(2000, 1, 2);
            expected = date;
            actual = timeline.GetCurrentDateTime();
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class TimelineIOTest
    {
        [TestMethod]
        public void TestMockIO()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDate("000101")),
                new WindowInfo(name:"2", date: Date.MakeDate("000102")),
                new WindowInfo(name:"3", date: Date.MakeDate("000103")),
                new WindowInfo(name:"4", date: Date.MakeDate("000104")),
            };

            MockStreamWriter writer;
            MockStreamReader reader;
            writer = new MockStreamWriter();

            timeline.AddLog(items);
            timeline.Export(writer);

            reader = new MockStreamReader(writer.GetOutputAsString());

            var timeline2 = new Timeline();
            timeline2.Import(reader);

            var expected = timeline.GetTimeline();
            var actual = timeline2.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIO()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: Date.MakeDate("000101")),
                new WindowInfo(name:"2", date: Date.MakeDate("000102")),
                new WindowInfo(name:"3", date: Date.MakeDate("000103")),
                new WindowInfo(name:"4", date: Date.MakeDate("000104")),
            };

            timeline.AddLog(items);
            timeline.Export("test-io");

            var timeline2 = new Timeline();
            timeline2.Import("test-io");

            var expected = timeline.GetTimeline();
            var actual = timeline2.GetTimeline();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
