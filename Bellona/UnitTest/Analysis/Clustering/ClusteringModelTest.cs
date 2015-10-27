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
        public void CreateFromNumber_1()
        {
            var empty = ClusteringModel.CreateFromNumber<Color>(c => new double[] { c.R, c.G, c.B }, 12);
            var model = empty.Train(TestData.GetColors());
            DisplayResultForColors(model);

            var cluster = model.Assign(Color.FromArgb(0, 92, 175)); // Ruri
            Console.WriteLine("Ruri: Cluster {0}", cluster.Id);
        }

        [TestMethod]
        public void CreateFromNumber_2()
        {
            var initialColors = new[] { Color.Red, Color.OrangeRed, Color.DarkOrange, Color.Orange, Color.Gold, Color.Yellow, Color.Chartreuse, Color.Lime, Color.SpringGreen, Color.Cyan, Color.DeepSkyBlue, Color.Blue, Color.Magenta };

            var empty = ClusteringModel.CreateFromNumber<Color>(c => new double[] { c.R, c.G, c.B }, initialColors.Length);
            var initial = empty.Train(initialColors);
            var model = initial.Train(TestData.GetColors().Except(initialColors));
            DisplayResultForColors(model);
        }

        [TestMethod]
        public void CreateFromNumber_138()
        {
            var data = TestData.GetColors().Except(new[] { Color.Fuchsia, Color.Aqua }).ToArray();
            var model = ClusteringModel.CreateFromNumber<Color>(c => new double[] { c.R, c.G, c.B }, data.Length)
                .Train(data);

            Assert.AreEqual(data.Length, model.Clusters.Length);
            model.Clusters.Execute(c => Assert.AreEqual(1, c.Records.Length));
        }

        [TestMethod]
        public void CreateAuto_1()
        {
            var empty = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B });
            var model = empty.Train(TestData.GetColors());
            DisplayResultForColors(model);

            var cluster = model.Assign(Color.FromArgb(0, 92, 175)); // Ruri
            Console.WriteLine("Ruri: Cluster {0}", cluster.Id);
        }

        [TestMethod]
        public void CreateAuto_2()
        {
            var empty = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B });
            var model = empty.Train(TestData.GetColors(), 2);
            DisplayResultForColors(model);
        }

        [TestMethod]
        public void CreateAuto_3()
        {
            var empty = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B });
            var model = empty.Train(TestData.GetColors(), maxStandardScore: 1.5);
            DisplayResultForColors(model);
        }

        [TestMethod]
        public void CreateAuto_9()
        {
            var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.GetSaturation(), c.GetBrightness() })
                .Train(TestData.GetColors());
            DisplayResultForColors(model);
        }

        [TestMethod]
        public void CreateAuto_138()
        {
            var data = TestData.GetColors().Except(new[] { Color.Fuchsia, Color.Aqua }).ToArray();
            var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
                .Train(data, maxStandardScore: 0.99);

            Assert.AreEqual(data.Length, model.Clusters.Length);
            model.Clusters.Execute(c => Assert.AreEqual(1, c.Records.Length));
        }

        [TestMethod]
        public void ToSimpleArray_1()
        {
            var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
                .Train(TestData.GetColors())
                .ToSimpleArray();
            DisplayResultForColors(model);
        }

        void DisplayResultForColors(ClusteringModelBase<Color> model)
        {
            model.Clusters
                .Do(c => Console.WriteLine(c.Id))
                .SelectMany(c => c.DeviationInfo.Records)
                .Execute(r => Console.WriteLine("{0:F3}, {1}, {2}", r.StandardScore, r.Features, r.Element.Element.Name));
        }

        void DisplayResultForColors(Color[][] colors)
        {
            colors
                .Select((c, i) => new { color = c, i })
                .Do(_ => Console.WriteLine(_.i))
                .SelectMany(_ => _.color)
                .Execute(c => Console.WriteLine(c.Name));
        }
    }
}
