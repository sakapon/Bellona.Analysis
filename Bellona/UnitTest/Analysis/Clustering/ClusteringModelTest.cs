using System;
using System.Drawing;
using System.Linq;
using Bellona.Analysis.Clustering;
using Bellona.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Analysis.Clustering
{
    [TestClass]
    public class ClusteringModelTest
    {
        [TestMethod]
        public void Test_1()
        {
            var empty = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B });
            var model = empty.Train(TestData.GetColors());
            DisplayResultForColors(model);

            var cluster = model.Assign(Color.FromArgb(0, 92, 175)); // Ruri
            Console.WriteLine("Ruri: Cluster {0}", cluster.Id);
        }

        [TestMethod]
        public void Test_2()
        {
            var empty = ClusteringModel.CreateFromNumber<Color>(c => new double[] { c.R, c.G, c.B }, 12);
            var model = empty.Train(TestData.GetColors());
            DisplayResultForColors(model);
        }

        [TestMethod]
        public void Test_3()
        {
            var empty = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B });
            var model = empty.Train(TestData.GetColors(), maxStandardScore: 1.5);
            DisplayResultForColors(model);
        }

        [TestMethod]
        public void Test_4()
        {
            var empty = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B });
            var model = empty.Train(TestData.GetColors(), 2);
            DisplayResultForColors(model);
        }

        void DisplayResultForColors(ClusteringModelBase<Color> model)
        {
            model.Clusters
                .Do(c => Console.WriteLine(c.Id))
                .SelectMany(c => c.DeviationInfo.Records)
                .Execute(r => Console.WriteLine("{0:F3}, {1}, {2}", r.StandardScore, r.Features, r.Element.Element.Name));
        }
    }
}
