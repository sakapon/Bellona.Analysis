using System;
using System.Linq;
using Bellona.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Core
{
    [TestClass]
    public class DeviationModelTest
    {
        [TestMethod]
        public void Test_1()
        {
            var data = new[]
            {
                new SamplePoint { Id = 1, Point = new[] { 2.0, 3.0 } },
                new SamplePoint { Id = 2, Point = new[] { 4.0, 5.0 } },
            };

            var model = DeviationModel.Create(data, d => d.Point);

            Assert.AreEqual(1.0, model.Records[0].StandardScore);
            Assert.AreEqual(1.0, model.Records[1].StandardScore);
        }
    }
}
