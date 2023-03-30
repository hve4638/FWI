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
    public class StateResultsTest
    {
        /*
        [TestMethod]
        public void TestState()
        {
            var results = new Result<ResultState, string>(ResultState.Normal);

            var expected = ResultState.Normal;
            var actual = results.State;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestResultsState()
        {
            var results = new Results<ResultState, string>();

            results += new Result<ResultState, string>(ResultState.Normal);
            results += new Result<ResultState, string>(ResultState.HasProblem);



            var expected = new List<ResultState> { ResultState.Normal, ResultState.HasProblem };
            var actual = new List<ResultState>();

            foreach (var result in results)
            {
                actual.Add(result.State);
            }

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestResultsObjectEqual()
        {
            var results = new Results<ResultState, string>();

            var result1 = new Result<ResultState, string>(ResultState.Normal);
            var result2 = new Result<ResultState, string>(ResultState.HasProblem);

            results += result1;
            results += result2;

            var expected = new List<object> { result1, result2 };
            var actual = new List<object>();

            foreach (var result in results)
            {
                actual.Add(result);
            }

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestResultsParser()
        {
            var results = new Results<ResultState, int>();
            var result1 = new Result<ResultState, int>(ResultState.Normal);
            var result2 = new Result<ResultState, int>(ResultState.Normal);
            result1 += 1;
            result1 += 2;
            result2 += 3;
            result2 += 4;
            results += result1;
            results += result2;

            int expected = 10;
            int actual = 0;
            results.Parse()
                .With(ResultState.Normal, (result) => {
                    foreach (var item in result)
                    {
                        actual += item;
                    }
                });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestResultsParser2()
        {
            var results = new Results<ResultState, int>();
            var result1 = new Result<ResultState, int>(ResultState.Normal);
            var result2 = new Result<ResultState, int>(ResultState.HasProblem);
            result1 += 1;
            result1 += 2;
            result2 += 3;
            result2 += 4;
            results += result1;
            results += result2;

            int expected = 73;
            int actual = 0;
            results.Parse()
                .With(ResultState.Normal, (result) =>
                {
                    foreach (var item in result)
                    {
                        actual += item;
                    }
                })
                .With(ResultState.HasProblem, (result) => {
                    foreach (var item in result)
                    {
                        actual += item * 10;
                    }
                });

            Assert.AreEqual(expected, actual);
        }

        /**/
    }
}

#endif