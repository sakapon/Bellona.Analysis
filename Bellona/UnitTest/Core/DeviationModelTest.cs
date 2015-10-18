using System;
using System.Linq;
using Bellona.Core;
using Bellona.Linq;
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
                new SamplePoint { Id = 2, Point = new[] { 8.0, 11.0 } },
            };

            var model = DeviationModel.Create(data, d => d.Point);

            Assert.AreEqual(new[] { 5.0, 7.0 }, model.Mean);
            Assert.AreEqual(5.0, model.StandardDeviation);
            Assert.AreEqual(1.0, model.Records[0].StandardScore);
            Assert.AreEqual(1.0, model.Records[1].StandardScore);
        }

        [TestMethod]
        public void Test_2()
        {
            var model = DeviationModel.Create(TestData.GetColors(), c => new double[] { c.R, c.G, c.B });

            model.Records
                .OrderBy(r => r.StandardScore)
                .Execute(r => Console.WriteLine("{0:F3}: {1}, {2}", r.StandardScore, r.Element.Name, r.Features));
        }
    }
}
