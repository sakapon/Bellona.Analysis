using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bellona.Core;
using Bellona.Linq;

namespace Bellona.Analysis.Clustering
{
    public static class ClusteringModel
    {
        public static ClusteringModel<T> CreateFromNumber<T>(Func<T, ArrayVector> featuresSelector, int clustersNumber)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");
            if (clustersNumber <= 0) throw new ArgumentOutOfRangeException("clustersNumber", clustersNumber, "The value must be positive.");

            return new ClusteringModel<T>(featuresSelector, new Cluster<T>[0], new ClusteringRecord<T>[0], clustersNumber);
        }

        public static AutoClusteringModel<T> CreateAuto<T>(Func<T, ArrayVector> featuresSelector)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");

            return new AutoClusteringModel<T>(featuresSelector, new Cluster<T>[0], new ClusteringRecord<T>[0]);
        }
    }

    [DebuggerDisplay(@"\{Clusters={Clusters.Length}, Records={Records.Length}\}")]
    public abstract class ClusteringModelBase<T>
    {
        protected Func<T, ArrayVector> FeaturesSelector { get; private set; }

        public Cluster<T>[] Clusters { get; private set; }
        public ClusteringRecord<T>[] Records { get; private set; }

        protected ClusteringModelBase(Func<T, ArrayVector> featuresSelector, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
        {
            FeaturesSelector = featuresSelector;
            Clusters = clusters;
            Records = records;
        }

        public Cluster<T> Assign(T element)
        {
            if (Clusters.Length == 0) throw new InvalidOperationException("This model is not trained.");

            return ClusteringHelper.AssignFeatures(Clusters, FeaturesSelector(element));
        }
    }

    public class ClusteringModel<T> : ClusteringModelBase<T>
    {
        public int ClustersNumber { get; private set; }

        public ClusteringModel(Func<T, ArrayVector> featuresSelector, Cluster<T>[] clusters, ClusteringRecord<T>[] records, int clustersNumber)
            : base(featuresSelector, clusters, records)
        {
            ClustersNumber = clustersNumber;
        }

        public ClusteringModel<T> Train(IEnumerable<T> source, int? maxIterations = null)
        {
            if (source == null) throw new ArgumentNullException("source");

            var newRecords = source.Select(e => new ClusteringRecord<T>(e, FeaturesSelector(e)));
            var records = Records.Concat(newRecords).ToArray();
            if (records.Length == 0) throw new InvalidOperationException("This model has no records.");

            var initial = Clusters.Length > 0 ? Clusters : ClusteringHelper.InitializeClusters(ClustersNumber, records);
            var clusters = ClusteringHelper.TrainForNumber(initial, records, maxIterations);

            return new ClusteringModel<T>(FeaturesSelector, clusters, records, ClustersNumber);
        }
    }

    public class AutoClusteringModel<T> : ClusteringModelBase<T>
    {
        public AutoClusteringModel(Func<T, ArrayVector> featuresSelector, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
            : base(featuresSelector, clusters, records)
        {
        }

        public AutoClusteringModel<T> Train(IEnumerable<T> source, int? maxClustersNumber = null, double maxStandardScore = 1.645)
        {
            if (source == null) throw new ArgumentNullException("source");

            var newRecords = source.Select(e => new ClusteringRecord<T>(e, FeaturesSelector(e)));
            var records = Records.Concat(newRecords).ToArray();
            if (records.Length == 0) throw new InvalidOperationException("This model has no records.");

            var initial = Clusters.Length > 0 ? Clusters : new Cluster<T>(0, records.Take(1)).MakeArray();
            var clusters = ClusteringHelper.TrainForStandardScore(initial, records, maxClustersNumber, maxStandardScore);

            return new AutoClusteringModel<T>(FeaturesSelector, clusters, records);
        }
    }
}
