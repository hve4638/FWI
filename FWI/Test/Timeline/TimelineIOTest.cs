#if TEST
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
    public class TimelineIOTest
    {
        [TestMethod]
        public void MockIO()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: TestUtils.MakeDate("000101")),
                new WindowInfo(name:"2", date: TestUtils.MakeDate("000102")),
                new WindowInfo(name:"3", date: TestUtils.MakeDate("000103")),
                new WindowInfo(name:"4", date: TestUtils.MakeDate("000104")),
            };

            MockStreamWriter writer = new MockStreamWriter();
            MockStreamReader reader;

            timeline.AddLog(items);
            timeline.Export(writer);

            reader = new MockStreamReader(writer.OutputAsString);

            var timeline2 = new Timeline();
            timeline2.Import(reader);

            var expected = timeline.GetAllWIs();
            var actual = timeline2.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ActualIO()
        {
            Timeline timeline = new Timeline();
            WindowInfo[] items = {
                new WindowInfo(name:"1", date: TestUtils.MakeDate("000101")),
                new WindowInfo(name:"2", date: TestUtils.MakeDate("000102")),
                new WindowInfo(name:"3", date: TestUtils.MakeDate("000103")),
                new WindowInfo(name:"4", date: TestUtils.MakeDate("000104")),
            };

            timeline.AddLog(items);
            timeline.Export("test-io");

            var timeline2 = new Timeline();
            timeline2.Import("test-io");

            var expected = timeline.GetAllWIs();
            var actual = timeline2.GetAllWIs();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
#endif