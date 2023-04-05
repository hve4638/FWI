#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUtility.Testing;
using System.Collections.ObjectModel;

namespace FWI.Test
{
    [TestClass]
    public class SingleLoggerLegacyTest
    {
        [TestMethod]
        public void TestGetLog()
        {
            ILogger logger = new LoggerLegacy();
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name: "1", date: TestUtils.MakeDate("000101")),
                new WindowInfoLegacy(name: "2", date: TestUtils.MakeDate("000103")),
                new WindowInfoLegacy(name: "3", date: TestUtils.MakeDate("000105")),
                new WindowInfoLegacy(name: "4", date: TestUtils.MakeDate("000107")),
            };
            foreach (var item in items) logger.AddWI(item);

            var range = TestUtils.MakeDateRange("000101", "000103");
            var actual = logger.GetLog(range);

            WindowInfoLegacy[] expected = { items[0], items[1] };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetLogInterval()
        {
            DateTime current = TestUtils.MakeDateTime("000101 120000");
            var logger = new LoggerLegacy();
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120010")),
                new WindowInfoLegacy(name: "3", date: TestUtils.MakeDateTime("000101 120045")),
                new WindowInfoLegacy(name: "4", date: TestUtils.MakeDateTime("000101 120110")),
            };

            logger.SetDateTimeDelegate(() => current);
            logger.SetLoggingInterval(1);

            foreach (var item in items)
            {
                current = item.Date;
                logger.AddWI(item); 
            }
            current = TestUtils.MakeDateTime("000101 120300");

            WindowInfoLegacy[] expected = { items[1], items[3] };
            var actual = logger.GetLog();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetLogInterval2()
        {
            DateTime current = TestUtils.MakeDateTime("000101 120000");
            var logger = new LoggerLegacy();
            WindowInfoLegacy[] items = {
                new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 120000")),
                new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120010")),
                new WindowInfoLegacy(name: "3", date: TestUtils.MakeDateTime("000101 120045")),
                new WindowInfoLegacy(name: "4", date: TestUtils.MakeDateTime("000101 120110")),
            };

            logger.SetDateTimeDelegate(() => current);
            logger.SetLoggingInterval(1);

            foreach (var item in items)
            {
                current = item.Date;
                logger.AddWI(item);
            }
            current = TestUtils.MakeDateTime("000101 120140");

            WindowInfoLegacy[] expected = { items[1] };
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
            ILogger logger = new LoggerLegacy();
            logger.AddWI(new WindowInfoLegacy(name: "hello.exe", title: "hello", date: new DateTime(2022, 11, 27, 15, 10, 10)));
            logger.AddWI(new WindowInfoLegacy(name: "hello2.exe", title: "hello2", date: new DateTime(2022, 11, 27, 16, 20, 15)));
            logger.AddWI(new WindowInfoLegacy(name: "hello3.exe", title: "hello3", date: new DateTime(2022, 11, 27, 17, 03, 00)));
            logger.AddWI(new WindowInfoLegacy(name: "hello.exe", title: "hello", date: new DateTime(2022, 11, 28, 04, 10, 00)));
            logger.Export("test-log");

            ILogger logger2 = new LoggerLegacy();
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
            ILogger logger = new LoggerLegacy();
            for (int i = 0; i < 500; i++)
            {
                date = getRandDate(date);
                logger.AddWI(new WindowInfoLegacy(name: getRandString(6), title: getRandString(4), date: date));
            }
            logger.Export(fname);

            ILogger logger2 = new LoggerLegacy();
            logger2.Import(fname);

            var list1 = logger.GetLog();
            var list2 = logger2.GetLog();
            AssertWindowInfoList(list1, list2);
        }

        void AssertWindowInfoList(ReadOnlyCollection<WindowInfoLegacy> list1, ReadOnlyCollection<WindowInfoLegacy> list2)
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
            var logger = new LoggerLegacy();
            var logger2 = new LoggerLegacy();

            Assert.AreEqual(logger.GetContentsHashCode(), logger2.GetContentsHashCode());
        }


        [TestMethod]
        public void TestHashNotEqual()
        {
            var logger = new LoggerLegacy();
            var logger2 = new LoggerLegacy();
            logger2.AddWI(new WindowInfoLegacy(name: "1", date: TestUtils.MakeDateTime("000101 120000")));
            logger2.AddWI(new WindowInfoLegacy(name: "2", date: TestUtils.MakeDateTime("000101 120010")));


            var expected = logger.GetContentsHashCode();
            var actual = logger2.GetContentsHashCode();
            Assert.AreNotEqual(expected, actual);
        }
    }
}

#endif