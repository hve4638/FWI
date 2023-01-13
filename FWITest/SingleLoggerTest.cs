using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;
using System.Collections.ObjectModel;

namespace FWITest
{
    [TestClass]
    public class SingleLoggerTest
    {
        [TestMethod]
        public void TestGetLog()
        {
            Logger logger = new SingleLogger();
            WindowInfo[] items = {
                new WindowInfo(name: "1", date: Date.MakeDate("000101")),
                new WindowInfo(name: "2", date: Date.MakeDate("000103")),
                new WindowInfo(name: "3", date: Date.MakeDate("000105")),
                new WindowInfo(name: "4", date: Date.MakeDate("000107")),
            };
            foreach (var item in items) logger.AppendWindowInfo(item);

            var range = Date.MakeDateRange("000101", "000103");
            var actual = logger.GetLog(range);

            WindowInfo[] expected = { items[0], items[1] };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetLogInterval()
        {
            DateTime current = Date.MakeDateTime("000101 120000");
            var logger = new SingleLogger();
            WindowInfo[] items = {
                new WindowInfo(name: "1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120010")),
                new WindowInfo(name: "3", date: Date.MakeDateTime("000101 120045")),
                new WindowInfo(name: "4", date: Date.MakeDateTime("000101 120110")),
            };

            logger.SetDateTimeDelegate(() => current);
            logger.SetLoggingInterval(1);

            foreach (var item in items)
            {
                current = item.Date;
                logger.AppendWindowInfo(item); 
            }
            current = Date.MakeDateTime("000101 120300");

            WindowInfo[] expected = { items[1], items[3] };
            var actual = logger.GetLog();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetLogInterval2()
        {
            DateTime current = Date.MakeDateTime("000101 120000");
            var logger = new SingleLogger();
            WindowInfo[] items = {
                new WindowInfo(name: "1", date: Date.MakeDateTime("000101 120000")),
                new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120010")),
                new WindowInfo(name: "3", date: Date.MakeDateTime("000101 120045")),
                new WindowInfo(name: "4", date: Date.MakeDateTime("000101 120110")),
            };

            logger.SetDateTimeDelegate(() => current);
            logger.SetLoggingInterval(1);

            foreach (var item in items)
            {
                current = item.Date;
                logger.AppendWindowInfo(item);
            }
            current = Date.MakeDateTime("000101 120140");

            WindowInfo[] expected = { items[1] };
            var actual = logger.GetLog();
            CollectionAssert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class SingleLoggerIOTest
    {
        [TestMethod]
        public void TestExportAndImport()
        {
            Logger logger = new SingleLogger();
            logger.AppendWindowInfo(new WindowInfo(name: "hello.exe", title: "hello", date: new DateTime(2022, 11, 27, 15, 10, 10)));
            logger.AppendWindowInfo(new WindowInfo(name: "hello2.exe", title: "hello2", date: new DateTime(2022, 11, 27, 16, 20, 15)));
            logger.AppendWindowInfo(new WindowInfo(name: "hello3.exe", title: "hello3", date: new DateTime(2022, 11, 27, 17, 03, 00)));
            logger.AppendWindowInfo(new WindowInfo(name: "hello.exe", title: "hello", date: new DateTime(2022, 11, 28, 04, 10, 00)));
            logger.Export("test-log");

            Logger logger2 = new SingleLogger();
            logger2.Import("test-log");

            var list1 = logger.GetLog();
            var list2 = logger2.GetLog();
            AssertWindowInfoList(list1, list2);
        }

        [TestMethod]
        public void TestExportAndImportRandom()
        {
            var rand = new Random(15);
            var date = new DateTime(rand.Next(2000, 2022), rand.Next(1, 13), rand.Next(1, 20), rand.Next(0, 23), rand.Next(0, 60), rand.Next(0, 60));
            DateTime getRandDate(DateTime initDate)
            {
                return initDate + new TimeSpan(rand.Next(0, 24), rand.Next(0, 60), rand.Next(0, 60));
            }
            string getRandString(int length)
            {
                string result = "";
                for (var i = 0; i < length; i++)
                {
                    result += Convert.ToChar(rand.Next(65, 91));
                }
                return result;
            }
            string fname = "test-log-export-and-import-random.txt";
            Logger logger = new SingleLogger();
            for (int i = 0; i < 500; i++)
            {
                date = getRandDate(date);
                logger.AppendWindowInfo(new WindowInfo(name: getRandString(6), title: getRandString(4), date: date));
            }
            logger.Export(fname);

            Logger logger2 = new SingleLogger();
            logger2.Import(fname);

            var list1 = logger.GetLog();
            var list2 = logger2.GetLog();
            AssertWindowInfoList(list1, list2);
        }

        void AssertWindowInfoList(ReadOnlyCollection<WindowInfo> list1, ReadOnlyCollection<WindowInfo> list2)
        {
            Assert.AreEqual(list1.Count, list2.Count);
            for (var i = 0; i < list1.Count; i++)
            {
                var item1 = list1[i];
                var item2 = list2[i];
                Assert.IsTrue(item1 == item2, $"index {i} item does not match.");
            }
        }

    }



    [TestClass]
    public class SingleLoggerHashTest
    {
        [TestMethod]
        public void TestHash()
        {
            var logger = new SingleLogger();
            var logger2 = new SingleLogger();

            Assert.AreEqual(logger.GetContentsHashCode(), logger2.GetContentsHashCode());
        }


        [TestMethod]
        public void TestHashNotEqual()
        {
            var logger = new SingleLogger();
            var logger2 = new SingleLogger();
            logger2.AppendWindowInfo(new WindowInfo(name: "1", date: Date.MakeDateTime("000101 120000")));
            logger2.AppendWindowInfo(new WindowInfo(name: "2", date: Date.MakeDateTime("000101 120010")));


            var expected = logger.GetContentsHashCode();
            var actual = logger2.GetContentsHashCode();
            Assert.AreNotEqual(expected, actual);
        }
    }
}

