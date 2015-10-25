using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bellona.Core;

namespace Bellona.Analysis.Clustering
{
    public static class ClusteringModel2
    {
        public static ClusteringModel2<T> CreateFromNumber<T>(Func<T, ArrayVector> featuresSelector, int clustersNumber)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");
            if (clustersNumber <= 0) throw new ArgumentOutOfRangeException("clustersNumber", clustersNumber, "The value must be positive.");

            return new ClusteringModelForNumber<T>(featuresSelector, clustersNumber, new Cluster<T>[0], new ClusteringRecord<T>[0]);
        }

        public static ClusteringModel2<T> CreateFromStandardScore<T>(Func<T, ArrayVector> featuresSelector, double maxStandardScore = 1.645)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");

            throw new NotImplementedException();
        }
    }

    [DebuggerDisplay(@"\{Clusters={Clusters.Length}, Records={Records.Length}\}")]
    public abstract class ClusteringModel2<T>
    {
        protected Func<T, ArrayVector> FeaturesSelector { get; private set; }

        public Cluster<T>[] Clusters { get; private set; }
        public ClusteringRecord<T>[] Records { get; private set; }

        protected ClusteringModel2(Func<T, ArrayVector> featuresSelector, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
        {
            FeaturesSelector = featuresSelector;
            Clusters = clusters;
            Records = records;
        }

        public abstract ClusteringModel2<T> Train(IEnumerable<T> source, int? maxIterations = null);

        public Cluster<T> Assign(T element)
        {
            if (Clusters.Length == 0) throw new InvalidOperationException("This model is not trained.");

            return ClusteringHelper.AssignFeatures(Clusters, FeaturesSelector(element));
        }
    }

    public class ClusteringModelForNumber<T> : ClusteringModel2<T>
    {
        public int ClustersNumber { get; private set; }

        public ClusteringModelForNumber(Func<T, ArrayVector> featuresSelector, int clustersNumber, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
            : base(featuresSelector, clusters, records)
        {
            ClustersNumber = clustersNumber;
        }

        public override ClusteringModel2<T> Train(IEnumerable<T> source, int? maxIterations = null)
        {
            if (source == null) throw new ArgumentNullException("source");

            var newRecords = source.Select(e => new ClusteringRecord<T>(e, FeaturesSelector(e)));
            var records = Records.Concat(newRecords).ToArray();

            var initial = Clusters.Length > 0 ? Clusters : ClusteringHelper.InitializeClusters(ClustersNumber, records);
            var clusters = ClusteringHelper.TrainIteratively(initial, records, maxIterations);

            return new ClusteringModelForNumber<T>(FeaturesSelector, ClustersNumber, clusters, records);
        }
    }
}
