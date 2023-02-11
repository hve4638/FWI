using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWI.Results;

namespace FWITest.FWI
{
    [TestClass]
    public class LazyRequestResultTest
    {
        [TestMethod]
        public void TestAccept()
        {
            int expected = 15;
            int actual = -1;
            var manager = new LazyRequestResultManager<int>();
            manager.Result
                .WithAccepted((num) => { actual = num; })
                .WithDenied((_) => { Assert.Fail(); });

            AcceptResult(manager, expected);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDenied()
        {
            int expected = 15;
            int actual = -1;
            var manager = new LazyRequestResultManager<int>();
            manager.Result
                .WithAccepted((num) => { Assert.Fail(); })
                .WithDenied((num) => { actual = num; });

            DenyResult(manager, expected);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestNothing()
        {
            var manager = new LazyRequestResultManager<int>();
            manager.Result
                .WithAccepted((num) => { Assert.Fail(); })
                .WithDenied((num) => { Assert.Fail(); });
        }

        public void AcceptResult<T>(LazyRequestResultManager<T> manager, T result)
        {
            manager.Accept(result);
        }

        public void DenyResult<T>(LazyRequestResultManager<T> manager, T result)
        {
            manager.Deny(result);
        }
    }
}
