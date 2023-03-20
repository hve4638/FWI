#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUtility;
using HUtility.Testing;

namespace HUtility.Test
{
    [TestClass]
    public class StringExtenderTest
    {
        [TestMethod]
        public void PadCenter4Word()
        {
            var original = "exam";
            var list = new List<(string, string)>
            {
                ("exam", original.PadCenter(1, '*')),
                ("exam", original.PadCenter(2, '*')),
                ("exam", original.PadCenter(3, '*')),
                ("exam", original.PadCenter(4, '*')),
                ("exam*", original.PadCenter(5, '*')),
                ("*exam*", original.PadCenter(6, '*')),
                ("*exam**", original.PadCenter(7, '*')),
                ("**exam**", original.PadCenter(8, '*')),
                ("**exam***", original.PadCenter(9, '*')),
            };

            CustomAssert.AllTrue(list, (item) => item.Item1 == item.Item2);
        }

        [TestMethod]
        public void PadCenter7Word()
        {
            var original = "example";

            var list = new List<(string, string)>
            {
                ("example", original.PadCenter(1, '*')),
                ("example", original.PadCenter(2, '*')),
                ("example", original.PadCenter(3, '*')),
                ("example", original.PadCenter(4, '*')),
                ("example", original.PadCenter(5, '*')),
                ("example", original.PadCenter(6, '*')),
                ("example", original.PadCenter(7, '*')),
                ("example*", original.PadCenter(8, '*')),
                ("*example*", original.PadCenter(9, '*')),
                ("*example**", original.PadCenter(10, '*')),
                ("**example**", original.PadCenter(11, '*')),
                ("**example***", original.PadCenter(12, '*')),
            };

            CustomAssert.AllTrue(list, (item) => item.Item1 == item.Item2);
        }
    }
}

#endif