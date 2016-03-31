using System;
using System.Linq;
using Bellona.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Linq
{
    [TestClass]
    public class Enumerable2Test
    {
        [TestMethod]
        public void Execute_1()
        {
            var sum = 0;

            Enumerable2.Repeat(3)
                .Take(10)
                .Do(i => sum += i)
                .Execute();

            Assert.AreEqual(30, sum);
        }

        [TestMethod]
        public void Execute_2()
        {
            var sum = 0;

            Enumerable2.Repeat(3)
                .Take(10)
                .Execute(i => sum += i);

            Assert.AreEqual(30, sum);
        }

        [TestMethod]
        public void Distinct_1()
        {
            var result = Enumerable.Range(1, 10)
                .Select(x => x * x)
                .Distinct(x => x % 3)
                .ToArray();

            CollectionAssert.AreEqual(new[] { 1, 9 }, result);
        }

        [TestMethod]
        public void Segment_1()
        {
            var result = Enumerable.Range(0, 9)
                .Segment(3)
                .ToArray();

            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, result[0]);
            CollectionAssert.AreEqual(new[] { 3, 4, 5 }, result[1]);
            CollectionAssert.AreEqual(new[] { 6, 7, 8 }, result[2]);
        }

        [TestMethod]
        public void Segment_2()
        {
            var result = Enumerable.Range(0, 8)
                .Segment(3)
                .ToArray();

            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, result[0]);
            CollectionAssert.AreEqual(new[] { 3, 4, 5 }, result[1]);
            CollectionAssert.AreEqual(new[] { 6, 7 }, result[2]);
        }

        [TestMethod]
        public void Prepend_1()
        {
            var result = Enumerable.Range(0, 10)
                .Prepend(-1)
                .ToArray();
            var expected = Enumerable.Range(-1, 11)
                .ToArray();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Append_1()
        {
            var result = Enumerable.Range(0, 10)
                .Append(10)
                .ToArray();
            var expected = Enumerable.Range(0, 11)
                .ToArray();

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
