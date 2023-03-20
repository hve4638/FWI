#if TEST
using FWI;
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
    public class TimelineSerializableTest
    {
        List<WindowInfoSerializable> GetSampleWIs()
        {
            var wis = new List<WindowInfoSerializable>
            {
                GetSampleWI("1", "a", "1a", DateTime.MinValue, false),
                GetSampleWI("2", "b", "2b", DateTime.MinValue, false),
                GetSampleWI("3", "c", "3c", DateTime.MinValue, false),
                GetSampleWI("4", "d", "4d", DateTime.MinValue, true),
            };

            return wis;
        }

        WindowInfoSerializable GetSampleWI(string name, string title, string alias, DateTime date, bool special)
        {
            var wi = new WindowInfoSerializable()
            {
                Name = name, Title = title, Alias = alias, Date = date, Special = special,
            };

            return wi;
        }

        [TestMethod]
        public void TestSerializeXML()
        {
            var wis = GetSampleWIs();
            SerializeTest(wis, SerializeType.Xml);
        }

        public void SerializeTest(List<WindowInfoSerializable> wis, SerializeType type)
        {
            var serializable = new TimelineSerializable
            {
                WIs = wis ?? new List<WindowInfoSerializable>()
            };

            var mw = new MockStreamWriter();
            serializable.Serialize(mw, type);

            var mr = new MockStreamReader(mw);
            var deserialized = TimelineSerializableDeserializer.Deserialize(mr, type);

            AssertEqualTimelineSerializable(serializable, deserialized);
        }

        public void AssertEqualTimelineSerializable(TimelineSerializable a, TimelineSerializable b)
        {
            Assert.AreEqual(a.WIs.Count, b.WIs.Count, "a, b의 리스트 길이가 다릅니다");

            var len = a.WIs.Count;
            for (var i = 0; i < len; i++)
            {
                try
                {
                    AssertEqualWISerializable(a.WIs[i], b.WIs[i]);
                }
                catch(AssertFailedException)
                {
                    throw new AssertFailedException($"{i}번째 인덱스의 값이 다릅니다.");
                }
            }
        }

        public void AssertEqualWISerializable(WindowInfoSerializable a, WindowInfoSerializable b)
        {
            Assert.AreEqual(a.Special, b.Special);
            Assert.AreEqual(a.Alias, b.Alias);
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.Title, b.Title);
            Assert.AreEqual(a.Date, b.Date);
        }
    }
}
#endif