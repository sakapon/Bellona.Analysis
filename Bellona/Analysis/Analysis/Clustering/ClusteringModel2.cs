using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bellona.Core;
using Bellona.Linq;

namespace Bellona.Analysis.Clustering
{
    public static class ClusteringModel2
    {
        public static ClusteringModel2<T> CreateFromNumber<T>(Func<T, ArrayVector> featuresSelector, int clustersNumber)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");
            if (clustersNumber <= 0) throw new ArgumentOutOfRangeException("clustersNumber", clustersNumber, "The value must be positive.");

            return new ClusteringModel2<T>(featuresSelector, clustersNumber, new Cluster<T>[0], new ClusteringRecord<T>[0]);
        }

        public static ClusteringModel2<T> CreateFromStandardScore<T>(Func<T, ArrayVector> featuresSelector, double maxStandardScore = 1.645)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");

            throw new NotImplementedException();
        }
    }

    [DebuggerDisplay(@"\{Clusters={Clusters.Length}, Records={Records.Length}\}")]
    public class ClusteringModel2<T>
    {
        Func<T, ArrayVector> _featuresSelector;

        public int ClustersNumber { get; private set; }
        public Cluster<T>[] Clusters { get; private set; }
        public ClusteringRecord<T>[] Records { get; private set; }

        public ClusteringModel2(Func<T, ArrayVector> featuresSelector, int clustersNumber, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
        {
            _featuresSelector = featuresSelector;
            ClustersNumber = clustersNumber;
            Clusters = clusters;
            Records = records;
        }

        public ClusteringModel2<T> Train(IEnumerable<T> source, int? maxIterations = null)
        {
            throw new NotImplementedException();
        }

        public Cluster<T> Assign(T element)
        {
            if (Clusters.Length == 0) throw new InvalidOperationException("This model is not trained.");

            return ClusteringHelper.AssignFeatures(Clusters, _featuresSelector(element));
        }
    }

    public static class ClusteringHelper
    {
        public static Cluster<T> AssignFeatures<T>(Cluster<T>[] clusters, ArrayVector features)
        {
            return clusters.FirstToMin(c => ArrayVector.GetDistance(c.Centroid, features));
        }

        public static Cluster<T> AssignRecord<T>(Cluster<T>[] clusters, ClusteringRecord<T> record)
        {
            return AssignFeatures(clusters, record.Features);
        }
    }
}
