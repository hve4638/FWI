using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI.Prompt;

namespace FWITest.FWI
{
    [TestClass]
    public class PromptTest
    {
        [TestMethod]
        public void TestGetEmpty()
        {
            var prompt = new Prompt();

            var expected = new List<string> { "help" };
            var actual = prompt.GetCommandList();
            expected.Sort();
            actual.Sort();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAdd()
        {
            var prompt = new Prompt();

            prompt.Add("echo", () => { });

            var expected = new List<string> { "echo", "help" };
            var actual = prompt.GetCommandList();
            expected.Sort();
            actual.Sort();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestExecute()
        {
            var prompt = new Prompt();
            bool mustTrue = false;

            prompt.Add("call", () => {
                mustTrue = true;
            });

            prompt.Execute("call");

            Assert.IsTrue(mustTrue);
        }

        [TestMethod]
        public void TestExecuteWithArgs()
        {
            var prompt = new Prompt();
            string[] expected = { "hello", "world" };
            string[] actual = { "", "" };

            prompt.Add("call", (args) =>
            {
                actual[0] = args[0];
                actual[1] = args[1];
            });
            prompt.Execute("call hello world");

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestExecuteWithArgsInt()
        {
            var prompt = new Prompt();
            int[] expected = { 1, 2, 3, 4 };
            int[] actual = { 0, 0, 0, 0 };

            prompt.Add("call", (args) =>
            {
                actual[0] = args.GetArgInt(0);
                actual[1] = args.GetArgInt(1);
                actual[2] = args.GetArgInt(2);
                actual[3] = args.GetArgInt(3);
            });
            prompt.Execute("call 1 2 3 4");

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
