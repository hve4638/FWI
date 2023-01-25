using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWITest.Learning
{
    [TestClass]
    public class TimeSpanTest
    {
        [TestMethod]
        public void TestTimeSpanTicks()
        {
            var expected = new TimeSpan(0, 10, 0);
            var actual = new TimeSpan(0, 0, 600);

            Assert.AreEqual(expected, actual);
        }
    }
}
