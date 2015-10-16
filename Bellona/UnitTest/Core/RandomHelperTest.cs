using System;
using System.Collections.Generic;
using System.Linq;
using Bellona.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Core
{
    [TestClass]
    public class RandomHelperTest
    {
        [TestMethod]
        public void ShuffleRange()
        {
            var set = new HashSet<int>(Enumerable.Range(0, 100));
            var result = RandomHelper.ShuffleRange(100).ToArray();

            Assert.IsTrue(set.SetEquals(result));
        }

        [TestMethod]
        public void GetRandomPiece_1()
        {
            var array = Enumerable.Range(10, 10).ToArray();
            var result = array.GetRandomPiece(3).ToArray();

            Assert.AreEqual(3, result.Length);
        }

        [TestMethod]
        public void GetRandomPiece_2()
        {
            var array = Enumerable.Range(10, 10).ToArray();
            var result = array.GetRandomPiece(10).ToArray();

            CollectionAssert.AreEqual(array, result);
        }
    }
}
