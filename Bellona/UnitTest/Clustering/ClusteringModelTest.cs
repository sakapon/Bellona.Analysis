using System;
using System.Drawing;
using System.Linq;
using Bellona.Clustering;
using Bellona.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Clustering
{
    [TestClass]
    public class ClusteringModelTest
    {
        [TestMethod]
        public void Test_1()
        {
            var model = new ClusteringModel<Color>(20, c => new double[] { c.R, c.G, c.B });
            model.Train(TestData.GetColors(), 50);

            model.Clusters
                .Do(c => Console.WriteLine(c.Id))
                .Execute(c => c.Records.Execute(r => Console.WriteLine(r.Element)));

            var cluster = model.AssignElement(Color.FromArgb(0, 92, 175)); // Ruri
            Console.WriteLine("Ruri: {0}", cluster.Id);
        }
    }
}
