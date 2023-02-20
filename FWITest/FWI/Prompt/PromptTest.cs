using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI.Prompt;

namespace FWITest.FWIPrompt
{
    [TestClass]
    public class PromptTest
    {
        [TestMethod]
        public void TestGetEmpty()
        {
            var prompt = new Prompt();

            var expected = new List<string>();
            var actual = prompt.GetCommands();
            expected.Sort();
            actual.Sort();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAdd()
        {
            var prompt = new Prompt();

            prompt.Add("echo", (args, output) => { });

            var expected = new List<string> { "echo" };
            var actual = prompt.GetCommands();
            expected.Sort();
            actual.Sort();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestExecute()
        {
            var prompt = new Prompt();
            bool result = false;

            prompt.Add("settrue", (args, output) => result = true);
            prompt.Add("setfalse", (args, output) => result = false);
            prompt.Execute("settrue");
            Assert.IsTrue(result);

            prompt.Execute("setfalse");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestExecuteMultiWord()
        {
            var prompt = new Prompt();
            bool result = false;

            prompt.Add("set true", (args, output) => result = true);
            prompt.Add("set false", (args, output) => result = false);
            prompt.Execute("set true");
            Assert.IsTrue(result);

            prompt.Execute("set false");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestExecuteWithArgs()
        {
            var prompt = new Prompt();
            string expected = "hello world";
            string actual = "";

            prompt.Add("set", (args, output) =>
            {
                actual = args.GetArgs();
            });
            prompt.Execute("set hello world");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestExecuteWithArgsInt()
        {
            var prompt = new Prompt();
            int[] expected = { 1, 2, 3, 4 };
            int[] actual = { 0, 0, 0, 0 };

            prompt.Add("set", (args, output) =>
            {
                actual[0] = args.GetArgInt(0);
                actual[1] = args.GetArgInt(1);
                actual[2] = args.GetArgInt(2);
                actual[3] = args.GetArgInt(3);
            });
            prompt.Execute("set 1 2 3 4");

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestNotExecuted()
        {
            var prompt = new Prompt();
            bool result = false;

            prompt.OnNotExecuted = (args, output) => result = false;
            prompt.Add("set", (args, output) => result = true);
            prompt.Execute("set");
            Assert.IsTrue(result);

            prompt.Execute("nocommand");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestRedirection()
        {
            var prompt = new Prompt();
            int result = -1;

            prompt.Add("result set", (args, output) => result = args.GetArgInt(0));
            prompt.AddRedirect("set", "result set");

            prompt.Execute("result set 5");
            Assert.AreEqual(5, result);

            prompt.Execute("set 10");
            Assert.AreEqual(10, result);
        }
    }
}
