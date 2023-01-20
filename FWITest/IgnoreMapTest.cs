using FWI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWITest
{
    [TestClass]
    public class IgnoreMapTest
    {
        [TestMethod]
        public void TestAdd()
        {
            var manager = new IgnoreMap();
            manager.Add("hello.exe");
            manager.Add("make.exe");

            var expected = new HashSet<string>
            {
                "hello.exe",
                "make.exe",
            };
            var actual = manager.Items;
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void TestMockImport()
        {
            var format = @"
                hello.exe
                make.exe
            ";
            var manager = new IgnoreMap();
            var reader = new MockStreamReader(format);

            manager.Import(reader);

            var expected = new HashSet<string>
            {
                "hello.exe",
                "make.exe",
            };
            var actual = manager.Items;
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void TestMockImportAndExport()
        {
            var manager = new IgnoreMap();
            manager.Add("hello.exe");
            manager.Add("make.exe");

            var writer = new MockStreamWriter();
            manager.Export(writer);

            var manager2 = new IgnoreMap();
            var reader = new MockStreamReader(writer.GetOutputAsString());
            manager2.Import(reader);

            var expected = manager.Items;
            var actual = manager2.Items;
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void TestContains()
        {
            var manager = new IgnoreMap();
            manager.Add("hello.exe");
            manager.Add("make.exe");

            Assert.IsTrue(manager.Contains("hello.exe"));
            Assert.IsTrue(manager.Contains("make.exe"));
            Assert.IsFalse(manager.Contains("apple.exe"));
        }
    }

}
