using FWI.Prompt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI;

namespace FWITest.FWIPrompt
{
    [TestClass]
    public class PromptExecuterTest
    {
        [TestMethod]
        public void TestAddCommand()
        {
            var executer = new PromptExecuter();

            var array = "hello".Split(' ');

            var queue = array.ToQueue();
            var result = executer.AddCommand(queue, (args, output) => { });
            Assert.IsTrue(result);

            queue = array.ToQueue();
            result = executer.AddCommand(queue, (args, output) => { });
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestExecute()
        {
            bool mustBeTrue = false;

            var executer = new PromptExecuter();

            var queue = new Queue<string>();
            executer.AddCommand(queue, (ar, output) =>
            {
                mustBeTrue = true;
            });

            var args = new PromptArgs("execute");
            executer.Execute(args, NullOutputStream.Instance);

            Assert.IsTrue(mustBeTrue);
        }

        [TestMethod]
        public void TestAddCommandMultiWord()
        {
            var executer = new PromptExecuter();

            var array = "set true".Split(' ');

            var queue = new Queue<string>(array);
            var result = executer.AddCommand(queue, (args, output) => { });
            Assert.IsTrue(result);

            queue = new Queue<string>(array);
            result = executer.AddCommand(queue, (args, output) => { });
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestExecuteMultiWord()
        {
            var executer = new PromptExecuter();
            var array = "set true".Split(' ');

            bool expected = true;
            bool actual = false;

            executer.AddCommand(array.ToQueue(), (args, output) =>
            {
                actual = true;
            });

            var promptArgs = new PromptArgs("set true");
            executer.Execute(promptArgs, NullOutputStream.Instance);

            Assert.AreEqual(expected, actual);
        }
    }
}
