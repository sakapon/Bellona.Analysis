using System;
using System.Collections.Generic;
using System.Linq;
using Bellona.Core;
using Bellona.Linq;

namespace Bellona.Analysis.Clustering
{
    static class ClusteringHelper
    {
        public static Cluster<T>[] InitializeClusters<T>(int clustersNumber, IList<ClusteringRecord<T>> records)
        {
            return RandomHelper.ShuffleRange(records.Count)
                .Select(i => records[i])
                .Distinct(r => r.Features)
                .Select((r, i) => new Cluster<T>(i, r.ToEnumerable()))
                .Take(clustersNumber)
                .ToArray();
        }

        public static Cluster<T>[] TrainOnce<T>(Cluster<T>[] clusters, IList<ClusteringRecord<T>> records)
        {
            return records
                .GroupBy(r => AssignRecord(clusters, r))
                .OrderBy(g => g.Key.Id)
                .Select((g, i) => new Cluster<T>(i, g))
                .ToArray();
        }

        public static Cluster<T>[] TrainIteratively<T>(Cluster<T>[] clusters, IList<ClusteringRecord<T>> records, int? maxIterations)
        {
            var current = clusters;

            Enumerable2.Repeat(true, maxIterations)
                .Select(_ => TrainOnce(current, records))
                .TakeWhile(cs => !ClustersEquals(current, cs))
                .Execute(cs => current = cs);

            return current;
        }

        public static Cluster<T> AssignFeatures<T>(Cluster<T>[] clusters, ArrayVector features)
        {
            return clusters.FirstToMin(c => ArrayVector.GetDistance(c.Centroid, features));
        }

        public static Cluster<T> AssignRecord<T>(Cluster<T>[] clusters, ClusteringRecord<T> record)
        {
            return AssignFeatures(clusters, record.Features);
        }

        public static bool ClustersEquals<T>(Cluster<T>[] clusters1, Cluster<T>[] clusters2)
        {
            if (clusters1.Length != clusters2.Length) return false;

            return Enumerable.Range(0, clusters1.Length)
                .All(i => ClusterEquals(clusters1[i], clusters2[i]));
        }

        public static bool ClusterEquals<T>(Cluster<T> cluster1, Cluster<T> cluster2)
        {
            if (cluster1.Records.Length != cluster2.Records.Length) return false;

            return Enumerable.Range(0, cluster1.Records.Length)
                .All(i => cluster1.Records[i] == cluster2.Records[i]);
        }
    }
}
