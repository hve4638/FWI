using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Testing
{
    public static class CustomAssert
    {
        static (bool, int) IsAllPass<T>(List<T> items, Func<T, bool> func)
        {
            int i;
            var count = items.Count;
            for (i = 0; i < count; i++)
            {
                var item = items[i];
                if (!func(item)) return (false, i);
            }
            return (true, 0);
        }


        public static void AllTrue(List<bool> items)
        {
            var (passed, index) = IsAllPass(items, (x) => x);

            if (!passed) Assert.Fail($"{index}번째 인덱스가 실패했습니다.");
        }

        public static void AllTrue<T>(List<T> items, Func<T, bool> func)
        {
            var (passed, index) = IsAllPass(items, func);

            if (!passed)
            {
                var value = items[index];
                Assert.Fail($"{index}번째 인덱스가 실패했습니다. Value: {value}");
            }
        }

        public static void AllFalse(List<bool> items)
        {
            var (passed, index) = IsAllPass(items, (x) => !x);

            if (!passed) Assert.Fail($"{index}번째 인덱스가 실패했습니다.");
        }
        public static void AllFalse<T>(List<T> items, Converter<T, bool> func)
        {
            var (passed, index) = IsAllPass(items, (x) => !func(x));

            if (!passed) Assert.Fail($"{index}번째 인덱스가 실패했습니다. Value: {items[index]}");
        }

        public static void AllEqual<T, S>(List<T> items, Func<T, (S, S)> spliter)
        {
            var expected = new List<S>();
            var actual = new List<S>();

            foreach(var item in items)
            {
                var (e, a) = spliter(item);
                expected.Add(e);
                actual.Add(a);
            }

            AllEqual(expected, actual);
        }

        public static void AllEqual<T, S, U>(List<T> items, Func<T, (S, U)> spliter, Func<U, S> converter)
        {
            var expected = new List<S>();
            var actual = new List<U>();

            foreach (var item in items)
            {
                var (e, a) = spliter(item);
                expected.Add(e);
                actual.Add(a);
            }

            AllEqual(expected, actual, converter);
        }
        public static void AllEqual<T>(List<T> expected, List<T> actual)
        {
            var (passed, index) = IsAllEqual(expected, actual, (x) => x);

            if (!passed) Assert.Fail($"{index}번째 인덱스가 실패했습니다. 예상: {expected[index]} 실제: {actual[index]}");
        }

        public static void AllEqual<T, S>(List<T> expected, List<S> actual, Func<S, T> converter)
        {
            var (passed, index) = IsAllEqual(expected, actual, converter);

            var e = expected[index];
            var a = actual[index];
            if (!passed) Assert.Fail($"{index}번째 인덱스가 실패했습니다. 예상: {e} 실제: {converter(a)} (before: {a})");
        }

        static (bool, int) IsAllEqual<T, S>(List<T> expected, List<S> actual, Func<S, T> converter)
        {
            if (expected.Count != actual.Count) return (false, -1);

            var count = expected.Count;
            for (int i = 0; i < count; i++)
            {
                var x = expected[i];
                var y = converter(actual[i]);

                if (!x.Equals(y)) return (false, i);
            }
            return (true, 0);
        }
    }
}