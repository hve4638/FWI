#if TEST
using HUtility.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Test.Results
{

    [TestClass]
    public class ResultsTest
    {
        [TestMethod]
        public void TestAddItem()
        {
            var results = new Result<string>();
            results += "hello";
            results += "world";

            var expected = new List<string> { "hello", "world" };
            var actual = new List<string>();
            foreach (var item in results) actual.Add(item);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddResult()
        {

        }
    }
}

#endif