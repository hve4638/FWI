using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FWITest.Learning
{
    [TestClass]
    public class ParsingTest
    {
        [TestMethod]
        public void TestParse()
        {
            var actual = new List<string>();
            string text = "[A][B] [C]";

            int index = 0;
            var re = new Regex(@"\[[A-Z]\]");
            while (index + 3 < text.Length)
            {
                var s = text.Substring(index, 3);
                if (re.IsMatch(s))
                {
                    actual.Add(s);
                    index += 3;
                }
                else
                {
                    break;
                }
            }

            string[] expected = { "[A]", "[B]" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParse2()
        {
            string actual = "";
            string text = "[A][B] [C]";

            int index = 0;
            var re = new Regex(@"\[[A-Z]\]");
            while (index + 3 < text.Length)
            {
                var s = text.Substring(index, 3);
                if (re.IsMatch(s))
                {
                    index += 3;
                }
                else
                {
                    actual = text.Substring(index);
                    break;
                }
            }

            var expected = " [C]";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParse3()
        {
            var actual = new List<char>();
            string text = "[A][B] [C]";

            int index = 0;
            var re = new Regex(@"\[[A-Z]\]");
            while (index + 3 < text.Length)
            {
                var s = text.Substring(index, 3);
                if (re.IsMatch(s))
                {
                    actual.Add(s[1]);
                    index += 3;
                }
                else
                {
                    break;
                }
            }

            char[] expected = { 'A', 'B' };
            CollectionAssert.AreEqual(expected, actual);
        }
    }

    
}
