using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI.Prompt;

namespace FWITest.FWI
{
    [TestClass]
    public class PromptArgsTest
    {
        [TestMethod]
        public void TestGetCMD()
        {
            var str = "echo".Split(' ');
            var args = new PromptArgs(str);

            var expected = str[0];
            var actual = args.GetCMD();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetArg()
        {
            var arr = "echo hello world".Split(' ');
            var args = new PromptArgs(arr);

            string[] expectedArray = { "hello", "world" };
            foreach (var (i, expected) in expectedArray.Select((x, i) => (i, x)))
            {
                var actual = args.GetArg(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestIndexer()
        {
            var arr = "echo hello world".Split(' ');
            var args = new PromptArgs(arr);

            for(var i = 0; i < arr.Length-1; i++)
            {
                var actual = args.GetArg(i);
                var expected = args[i];
                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod]
        public void TestGetArgInt()
        {
            var arr = "echo 1 2 3".Split(' ');
            var args = new PromptArgs(arr);

            int[] expectedArray = { 1, 2, 3 };
            foreach (var (i, expected) in expectedArray.Select((x, i) => (i, x)))
            {
                var actual = args.GetArgInt(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestOutOfIndex()
        {
            var arr = "echo hello world".Split(' ');
            var args = new PromptArgs(arr);
            var catched = false;

            try
            {
                var n = args[5];
            }
            catch (IndexOutOfRangeException)
            {
                catched = true;
            }

            Assert.IsTrue(catched);
        }

        [TestMethod]
        public void TestOutOfIndexNegative()
        {
            var arr = "echo hello world".Split(' ');
            var args = new PromptArgs(arr);
            var catched = false;

            try
            {
                var n = args[-1];
            }
            catch (IndexOutOfRangeException)
            {
                catched = true;
            }

            Assert.IsTrue(catched);
        }

        [TestMethod]
        public void TestToString()
        {
            string expected = "echo hello world";

            var arr = expected.Split(' ');
            var args = new PromptArgs(arr);

            var actual = args.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetArgsAll()
        {
            var arr = "echo i wanna be the guy".Split(' ');
            var args = new PromptArgs(arr);

            var expected = "i wanna be the guy";
            var actual = args.GetArgs();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetArgsSplit()
        {
            var arr = "echo i wanna be the guy".Split(' ');
            var args = new PromptArgs(arr);

            var expected = "wanna be the guy";
            var actual = args.GetArgs(1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetArgsRange()
        {
            var arr = "echo i wanna be the guy".Split(' ');
            var args = new PromptArgs(arr);

            var expected = "wanna be";
            var actual = args.GetArgs(1, 3);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetArgsRange2()
        {
            var arr = "echo i wanna be the guy".Split(' ');
            var args = new PromptArgs(arr);

            var expected = "i wanna be the";
            var actual = args.GetArgs(0, 4);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetArgsResultNo()
        {
            var arr = "echo i wanna be the guy".Split(' ');
            var args = new PromptArgs(arr);

            var expected = "";
            var actual = args.GetArgs(15);
            Assert.AreEqual(expected, actual);
        }

    }
}
