using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;
using FWIConnection;
using FWITest;

namespace FWITest.FWIConnection
{
    [TestClass]
    public class BrBwExtenderTest
    {
        [TestMethod]
        public void TestWI()
        {
            var eName = "name";
            var eTitle = "title";
            var eDate = Date.MakeDate("000101");

            var bw = new ByteWriter();
            bw.WriteWI(eName, eTitle, eDate);

            var br = new ByteReader(bw.ToBytes());
            br.ReadWI(out string aName, out string aTitle, out DateTime aDate);

            Assert.AreEqual(eName, aName);
            Assert.AreEqual(eTitle, aTitle);
            Assert.AreEqual(eDate, aDate);
        }

        [TestMethod]
        public void TestDateTime()
        {
            var expected = Date.MakeDate("000101");

            var bw = new ByteWriter();
            bw.WriteDateTime(expected);

            var br = new ByteReader(bw.ToBytes());
            br.ReadDateTime(out DateTime actual);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestString()
        {
            var expected = "hello world";

            var bw = new ByteWriter();
            bw.WriteText(expected);

            var br = new ByteReader(bw.ToBytes());
            br.ReadText(out string actual);

            Assert.AreEqual(expected, actual);
        }
    }
}
