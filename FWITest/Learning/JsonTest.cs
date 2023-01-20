using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWITest.Learning
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void TestGetString()
        {
            var jsonString = "{ \"key\" : \"hello world!\" }";
            var doc = JsonDocument.Parse(jsonString);
            var ele = doc.RootElement.GetProperty("key");

            var expected = "hello world!";
            var actual = ele.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetInt()
        {
            var jsonString = "{ \"key\" : 1 }";
            var doc = JsonDocument.Parse(jsonString);
            var ele = doc.RootElement.GetProperty("key");

            int expected = 1;
            var result = ele.TryGetInt32(out int actual);
            Assert.IsTrue(result);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetArray()
        {
            var jsonString = "{ \"children\" : [ "
                            + "{ \"key\" : 1 },"
                            + "{ \"key\" : 2 },"
                            + "{ \"key\" : 3 }"
                            + "]}";
            var doc = JsonDocument.Parse(jsonString);
            var children = doc.RootElement.GetProperty("children");

            var expected = new List<int> { 1, 2, 3 };
            var actual = new List<int>();
            foreach (var child in children.EnumerateArray())
            {
                var ele = child.GetProperty("key");
                if (ele.TryGetInt32(out int item)) actual.Add(item);
            }

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSerialize()
        {
            var item = new JsonClass
            {
                Date = Date.MakeDateTime("000101 120000"),
                Index = 15,
                Value = "Hello World?"
            };
            var serialized = JsonSerializer.Serialize(item);
            var deserialized = JsonSerializer.Deserialize<JsonClass>(serialized);

            Assert.AreEqual(item.Date, deserialized.Date);
            Assert.AreEqual(item.Index, deserialized.Index);
            Assert.AreEqual(item.Value, deserialized.Value);
        }
    }

    class JsonClass
    {
        public DateTime Date { get; set; }
        public int Index { get; set; }
        public string Value { get; set;  }
    }
}
