using FWIConnection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWITest.FWIConnection
{
    [TestClass]
    public class BrBwTest
    {
        [TestMethod]
        public void TestBoolean()
        {
            var bw = new ByteWriter();
            bw.WriteBoolean(true);
            bw.WriteBoolean(false);
            bw.WriteBoolean(true);

            var br = new ByteReader(bw);
            Assert.IsTrue(br.ReadBoolean());
            Assert.IsFalse(br.ReadBoolean());
            Assert.IsTrue(br.ReadBoolean());
        }

        [TestMethod]
        public void TestRWShort()
        {
            var bw = new ByteWriter();
            bw.WriteShort(0);
            bw.WriteShort(1);
            bw.WriteShort(2);

            var br = new ByteReader(bw);

            Assert.AreEqual((short)0, br.ReadShort());
            Assert.AreEqual((short)1, br.ReadShort());
            Assert.AreEqual((short)2, br.ReadShort());
        }

        [TestMethod]
        public void TestRWInt()
        {
            var bw = new ByteWriter();
            bw.WriteInt(0);
            bw.WriteInt(1);
            bw.WriteInt(2);

            var br = new ByteReader(bw);

            Assert.AreEqual((int)0, br.ReadInt());
            Assert.AreEqual((int)1, br.ReadInt());
            Assert.AreEqual((int)2, br.ReadInt());
        }

        [TestMethod]
        public void TestRWString()
        {
            var bw = new ByteWriter();
            bw.WriteString("hello world");

            var br = new ByteReader(bw);

            Assert.AreEqual("hello world", br.ReadString());
        }

        [TestMethod]
        public void TestRWByte()
        {
            byte[] bytes = { 0x01, 0x02, 0x03, 0x04 };
            var bw = new ByteWriter();
            bw.WriteBytes(bytes);

            var br = new ByteReader(bw);

            CollectionAssert.AreEqual(bytes, br.ReadBytes());
        }

        [TestMethod]
        public void TestRWStringEnd()
        {
            var bw = new ByteWriter();
            bw.WriteString("hello world");

            var br = new ByteReader(bw);

            br.ReadString();
            Assert.AreEqual("", br.ReadString());
        }

        [TestMethod]
        public void TestRWByteEnd()
        {
            byte[] bytes = { 0x01, 0x02, 0x03, 0x04 };
            var bw = new ByteWriter();
            bw.WriteBytes(bytes);

            var br = new ByteReader(bw);

            CollectionAssert.AreEqual(bytes, br.ReadBytes());

            byte[] empty = { };
            CollectionAssert.AreEqual(empty, br.ReadBytes());
        }

        [TestMethod]
        public void TestPeekShort()
        {
            var bw = new ByteWriter();
            bw.WriteShort(0);
            bw.WriteShort(1);
            bw.WriteShort(2);

            var br = new ByteReader(bw);

            Assert.AreEqual((short)0, br.PeekShort());
            br.ReadShort();
            Assert.AreEqual((short)1, br.PeekShort());
            br.ReadShort();
            Assert.AreEqual((short)2, br.PeekShort());
        }

        [TestMethod]
        public void TestPeekInt()
        {
            var bw = new ByteWriter();
            bw.WriteInt(0);
            bw.WriteInt(1);
            bw.WriteInt(2);

            var br = new ByteReader(bw);

            Assert.AreEqual((int)0, br.PeekInt());
            br.ReadInt();
            Assert.AreEqual((int)1, br.PeekInt());
            br.ReadInt();
            Assert.AreEqual((int)2, br.PeekInt());
        }

        [TestMethod]
        public void TestPeekString()
        {
            var bw = new ByteWriter();
            bw.WriteString("hello world");

            var br = new ByteReader(bw);

            Assert.AreEqual("hello world", br.PeekString());
            Assert.AreEqual("hello world", br.ReadString());
        }

        [TestMethod]
        public void TestPeekBytes()
        {
            byte[] bytes = { 0x01, 0x02, 0x03, 0x04 };
            var bw = new ByteWriter();
            bw.WriteBytes(bytes);

            var br = new ByteReader(bw);
            CollectionAssert.AreEqual(bytes, br.PeekBytes());
            CollectionAssert.AreEqual(bytes, br.ReadBytes());
        }

        [TestMethod]
        public void TestPeekBoolean()
        {
            var bw = new ByteWriter();
            bw.WriteBoolean(true);

            var br = new ByteReader(bw);
            Assert.IsTrue(br.PeekBoolean());
            Assert.IsTrue(br.ReadBoolean());
        }
    }
}
