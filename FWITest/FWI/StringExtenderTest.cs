using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;

namespace FWITest.FWI
{
    [TestClass]
    public class StringExtenderTest
    {
        [TestMethod]
        public void TestPadCenter()
        {
            var original = "exam";

            var expected = "  exam  ";
            var actual = original.PadCenter(8);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPadCenter1()
        {
            var original = "exam";

            var expected = "exam ";
            var actual = original.PadCenter(5);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPadCenter2()
        {
            var original = "exam";

            var expected = "  exam   ";
            var actual = original.PadCenter(9);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPadCenter3()
        {
            var original = "example";

            var expected = "example";
            var actual = original.PadCenter(5);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPadCenter4()
        {
            var original = "example";

            var expected = " example ";
            var actual = original.PadCenter(9);
            Assert.AreEqual(expected, actual);
        }
    }
}
