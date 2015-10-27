using System;
using System.Collections.Generic;
using System.Linq;
using Bellona.Core;
using Bellona.Linq;

namespace Bellona.Analysis.Clustering
{
    static class ClusteringHelper
    {
        public static Cluster<T>[] Initialize<T>(IList<ClusteringRecord<T>> records, int clustersNumber)
        {
            return RandomHelper.ShuffleRange(records.Count)
                .Select(i => records[i])
                .Distinct(r => r.Features)
                .Select((r, i) => new Cluster<T>(i, r.MakeEnumerable()))
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

        public static Cluster<T>[] TrainForNumber<T>(Cluster<T>[] clusters, IList<ClusteringRecord<T>> records, int? maxIterations)
        {
            var current = clusters;

            foreach (var _ in Enumerable2.Repeat(true, maxIterations))
            {
                var cs = TrainOnce(current, records);
                if (ClustersEquals(current, cs)) return current;

                current = cs;
            }

            return current;
        }

        public static Cluster<T>[] TrainForAuto<T>(Cluster<T>[] clusters, IList<ClusteringRecord<T>> records, int? maxClustersNumber, double maxStandardScore)
        {
            var maxClustersNumber2 = maxClustersNumber.HasValue ? Math.Min(maxClustersNumber.Value, records.Count) : records.Count;
            var current = clusters;

            while (true)
            {
                current = TrainForNumber(current, records, null);
                if (current.Length >= maxClustersNumber2) return current;

                var farthestRecord = GetFarthestRecord(current);
                if (farthestRecord.StandardScore <= maxStandardScore) return current;

                current = SeparateClusters(current, farthestRecord.Element);
            }
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

        public static Cluster<T>[] SeparateClusters<T>(Cluster<T>[] clusters, ClusteringRecord<T> recordForNewCluster)
        {
            return clusters
                .Select(c => new Cluster<T>(c.Id, c.Records.Where(r => r != recordForNewCluster)))
                .Where(c => c.HasRecords)
                .Concat(new Cluster<T>(clusters.Length, recordForNewCluster.MakeEnumerable()).MakeEnumerable())
                .ToArray();
        }

        public static DeviationRecord<ClusteringRecord<T>> GetFarthestRecord<T>(Cluster<T>[] clusters)
        {
            return clusters
                .SelectMany(c => c.DeviationInfo.Records)
                .FirstToMax(r => r.StandardScore);
        }
    }
}
