#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Test
{
    [TestClass]
    public class TimelineMenualDateTimeTest
    {
        [TestMethod]
        public void MenualTimeline()
        {
            Timeline timeline = new Timeline();
            timeline.SetMenualDateTime(() => DateTime.MinValue);

            var expected = DateTime.MinValue;
            var actual = timeline.CurrentDateTime;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MenualTimelineClosure()
        {
            DateTime date = DateTime.MinValue;
            Timeline timeline = new Timeline();
            timeline.SetMenualDateTime(() => date);

            date = new DateTime(2000, 1, 1);
            Assert.AreEqual(date, timeline.CurrentDateTime);

            date = new DateTime(2000, 1, 2);
            Assert.AreEqual(date, timeline.CurrentDateTime);
        }
    }
}
#endif