using FWI;
using FWI.Message;
using FWIConnection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace FWITest.FWIConnection
{
    [TestClass]
    public class MessageSerializeTest
    {
        [TestMethod]
        public void TestMessage()
        {
            var expected = new TextMessage()
            {
                Text = "Hello World"
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = TextMessage.Deserialize(reader);

            Assert.AreEqual(expected.Text, actual.Text);
        }

        [TestMethod]
        public void TestEcho()
        {
            var expected = new EchoMessage()
            {
                Text = "Hello World"
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = EchoMessage.Deserialize(reader);

            Assert.AreEqual(expected.Text, actual.Text);
        }

        [TestMethod]
        public void TestServerCall()
        {
            var expected = new ServerCallMessage()
            {
                Command = "echo hello world"
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = ServerCallMessage.Deserialize(reader);

            Assert.AreEqual(expected.Command, actual.Command);
        }

        [TestMethod]
        public void TestRequestTimeline()
        {
            var original = new TimelineRequest();
            original.BeginDate = DateTime.MinValue;
            original.EndDate = DateTime.MaxValue;

            var bytes = original.Serialize();
            var reader = new ByteReader(bytes);
            TimelineRequest.Deserialize(reader);
        }

        [TestMethod]
        public void TestRequestRank()
        {
            var original = new RequestRankMessage();
            var bytes = original.Serialize();
            var reader = new ByteReader(bytes);
            RequestRankMessage.Deserialize(reader);
        }

        [TestMethod]
        public void TestUpdateWI()
        {
            var expected = new UpdateWIMessage()
            {
                Date = new DateTime(2000, 1, 1, 12, 00, 30),
                Name = "Name",
                Title = "Title"
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = UpdateWIMessage.Deserialize(reader);

            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Title, actual.Title);
        }
        [TestMethod]
        public void TestAFK()
        {
            var expected = new AFKMessage()
            {
                FromDate = new DateTime(2000,1,1, 12,00,30),
                ToDate = new DateTime(2000, 1, 1, 12, 00, 45),
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = AFKMessage.Deserialize(reader);

            Assert.AreEqual(expected.FromDate, actual.FromDate);
            Assert.AreEqual(expected.ToDate, actual.ToDate);
        }

        [TestMethod]
        public void TestNoAFK()
        {
            var expected = new NoAFKMessage()
            {
                FromDate = new DateTime(2000, 1, 1, 12, 00, 30),
                ToDate = new DateTime(2000, 1, 1, 12, 00, 45),
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = NoAFKMessage.Deserialize(reader);

            Assert.AreEqual(expected.FromDate, actual.FromDate);
            Assert.AreEqual(expected.ToDate, actual.ToDate);
        }

        [TestMethod]
        public void TestRequestToBeTarget()
        {
            var expected = new RequestToBeTargetMessage()
            {
                Id = 1,
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = RequestToBeTargetMessage.Deserialize(reader);

            Assert.AreEqual(expected.Id, actual.Id);
        }

        [TestMethod]
        public void TestResponseToBeTarget()
        {
            var expected = new ResponseToBeTargetMessage()
            {
                Id = 1,
                Accepted = true,
            };

            var bytes = expected.Serialize();
            var reader = new ByteReader(bytes);
            var actual = ResponseToBeTargetMessage.Deserialize(reader);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Accepted, actual.Accepted);
        }
    }
}
