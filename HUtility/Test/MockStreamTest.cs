#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUtility.Testing;

namespace HUtility.Test
{
    [TestClass]
    public class MockStreamTest
    {
        [TestMethod]
        public void TestReadAndWrite()
        {
            var expected = "hello world?";
            var writer = new MockStreamWriter();
            writer.WriteLine(expected);

            var reader = new MockStreamReader(writer.GetOutputAsString());
            var actual = reader.ReadLine();

            Assert.AreEqual(expected, actual);
        }
    }
}
#endif