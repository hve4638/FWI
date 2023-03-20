using FWI;
using FWI.Prompt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUtility;

namespace FWITest.FWIPrompt
{
    [TestClass]
    public class PromptExecuterTest
    {
        public void CommandEmpty(PromptArgs args, IOutputStream output)
        {

        }

        [TestMethod]
        public void TestAdd()
        {
            var executer = new PromptExecuter();

            bool result;
            Queue<string> queue;
            queue = MakeQueue("settrue");
            result = executer.AddCommand(queue, CommandEmpty);
            Assert.IsTrue(result);

            queue = MakeQueue("settrue");
            result = executer.AddCommand(queue, CommandEmpty);
            Assert.IsFalse(result);

            queue = MakeQueue("setfalse");
            result = executer.AddCommand(queue, CommandEmpty);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestAddMultiWord()
        {
            var executer = new PromptExecuter();

            bool result;
            Queue<string> queue;
            queue = MakeQueue("set true");
            result = executer.AddCommand(queue, CommandEmpty);
            Assert.IsTrue(result);

            queue = MakeQueue("set true");
            result = executer.AddCommand(queue, CommandEmpty);
            Assert.IsFalse(result);

            queue = MakeQueue("set false");
            result = executer.AddCommand(queue, CommandEmpty);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestExecute()
        {
            var executer = new PromptExecuter();
            var no = NullOutputStream.Instance;
            bool result = false;

            executer.AddCommand(MakeQueue("settrue"), (args, output) => result = true);
            executer.AddCommand(MakeQueue("setfalse"), (args, output) => result = false);

            executer.Execute(new PromptArgs("settrue"), no);
            Assert.IsTrue(result);

            executer.Execute(new PromptArgs("setfalse"), no);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestExecuteMultiWord()
        {
            var executer = new PromptExecuter();
            var no = NullOutputStream.Instance;
            bool result = false;

            executer.AddCommand(MakeQueue("set true"), (args, output) => result = true);
            executer.AddCommand(MakeQueue("set false"), (args, output) => result = false);

            executer.Execute(new PromptArgs("set true"), no);
            Assert.IsTrue(result);

            executer.Execute(new PromptArgs("set false"), no);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestExecuteWithArgs()
        {
            var executer = new PromptExecuter();
            var no = NullOutputStream.Instance;
            int result = -1;

            executer.AddCommand(MakeQueue("set"), (args, output) => result = args.GetArgInt(0));

            executer.Execute(new PromptArgs("set 5"), no);
            Assert.AreEqual(5, result);

            executer.Execute(new PromptArgs("set 10"), no);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestExecuteDepth()
        {
            var executer = new PromptExecuter();
            var no = NullOutputStream.Instance;
            int result = -1;

            executer.AddCommand(MakeQueue("set zero"), (args, output) => result = 0);
            executer.AddCommand(MakeQueue("set"), (args, output) => result = args.GetArgInt(0));

            executer.Execute(new PromptArgs("set 5"), no);
            Assert.AreEqual(5, result);

            executer.Execute(new PromptArgs("set zero"), no);
            Assert.AreEqual(0, result);
        }

        Queue<string> MakeQueue(string text) => text.Split(' ').ToQueue();
    }
}
