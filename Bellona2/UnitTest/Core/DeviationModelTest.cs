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
        public void Test_0()
        {
            var model = DeviationModel.Create(Enumerable.Empty<SamplePoint>(), d => d.Point);

            Assert.AreEqual(false, model.HasRecords);
            Assert.AreEqual(null, model.Mean);
            Assert.AreEqual(double.NaN, model.StandardDeviation);
        }

        [TestMethod]
        public void Test_1()
        {
            var sample = new SamplePoint { Id = 1, Point = new[] { 2.0, 3.0 } };
            var model = DeviationModel.Create(sample.MakeEnumerable(), d => d.Point);

            Assert.AreEqual(true, model.HasRecords);
            Assert.AreEqual(sample.Point, model.Mean);
            Assert.AreEqual(0.0, model.StandardDeviation);
            Assert.AreEqual(0.0, model.Records[0].Deviation);
            Assert.AreEqual(0.0, model.Records[0].StandardScore);
        }

        [TestMethod]
        public void Test_2()
        {
            var data = new[]
            {
                new SamplePoint { Id = 1, Point = new[] { 2.0, 3.0 } },
                new SamplePoint { Id = 2, Point = new[] { 8.0, 11.0 } },
            };

            var model = DeviationModel.Create(data, d => d.Point);

            Assert.AreEqual(new[] { 5.0, 7.0 }, model.Mean);
            Assert.AreEqual(5.0, model.StandardDeviation);
            Assert.AreEqual(5.0, model.Records[0].Deviation);
            Assert.AreEqual(1.0, model.Records[0].StandardScore);
            Assert.AreEqual(5.0, model.Records[1].Deviation);
            Assert.AreEqual(1.0, model.Records[1].StandardScore);
        }

        [TestMethod]
        public void Test_140()
        {
            var model = DeviationModel.Create(TestData.GetColors(), c => new double[] { c.R, c.G, c.B });

            model.Records
                .OrderBy(r => r.StandardScore)
                .Execute(r => Console.WriteLine("{0:F3}, {1}, {2}", r.StandardScore, r.Features, r.Element.Name));
        }
    }
}
