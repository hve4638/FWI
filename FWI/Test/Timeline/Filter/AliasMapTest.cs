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
    public class WindowInfoAliasFilterTest
    {
        [TestMethod]
        public void TestAdd()
        {
            var manager = new WindowInfoAliasFilter();
            manager["hello.exe"] = "Hello World";
            manager["make.exe"] = "MAKE";

            var expected = new Dictionary<string, string>
            {
                { "hello.exe", "Hello World" },
                { "make.exe", "MAKE" },
            };
            var actual = manager.Items;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMockImport()
        {
            var format = @"
                hello.exe : Hello World
                make.exe : MAKE
            ";
            var manager = new AliasMapLegacy();
            var reader = new MockStreamReader(format);

            manager.Import(reader);

            var expected = new Dictionary<string, string>
            {
                { "hello.exe", "Hello World" },
                { "make.exe", "MAKE" },
            };
            var actual = manager.Items;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMockExportAndImport()
        {
            var manager = new AliasMapLegacy();
            manager.Add("hello.exe", "Hello World");
            manager.Add("make.exe", "MAKE");

            var writer = new MockStreamWriter();
            manager.Export(writer);

            var manager2 = new AliasMapLegacy();

            var reader = new MockStreamReader(writer.GetOutputAsString());
            manager2.Import(reader);

            var expected = manager.Items;
            var actual = manager2.Items;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestFilter()
        {
            var manager = new AliasMapLegacy();
            manager.Add("make.exe", "MAKE");

            var wi = new WindowInfoLegacy(name: "make.exe", title: "make");
            manager.Filter(ref wi);

            var expected = "MAKE";
            var actual = wi.Alias;
            Assert.AreEqual(expected, actual);
        }
    }
}
