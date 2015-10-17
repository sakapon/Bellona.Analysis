using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bellona.Core;
using Bellona.Linq;

namespace Bellona.Clustering
{
    public class ClusteringModel<T>
    {
        public int ClustersNumber { get; private set; }
        public Cluster<T>[] Clusters { get; private set; }

        Func<T, double[]> _featuresSelector;
        List<ClusteringRecord<T>> _records = new List<ClusteringRecord<T>>();

        public ClusteringModel(int clustersNumber, Func<T, double[]> featuresSelector)
        {
            ClustersNumber = clustersNumber;
            _featuresSelector = featuresSelector;
        }

        public void Train(IEnumerable<T> source, int iterationsNumber)
        {
            _records.AddRange(source.Select(e => new ClusteringRecord<T>(e, _featuresSelector)));

            if (Clusters == null)
                Clusters = InitializeClusters(ClustersNumber, _records);

            for (var i = 0; i < iterationsNumber; i++)
                TrainOnce(Clusters, _records);
        }

        public Cluster<T> AssignElement(T element)
        {
            if (Clusters == null) throw new InvalidOperationException("This model is not trained.");

            var features = _featuresSelector(element);
            return Clusters.FirstToMin(c => ArrayVector.GetDistance(c.Centroid, features));
        }

        static Cluster<T>[] InitializeClusters(int clustersNumber, List<ClusteringRecord<T>> records)
        {
            return RandomHelper.ShuffleRange(records.Count)
                .Select(i => records[i])
                .Distinct(r => r.Features)
                .Select((r, i) => new Cluster<T>(i, r.Features))
                .Take(clustersNumber)
                .ToArray();
        }

        static void TrainOnce(Cluster<T>[] clusters, IEnumerable<ClusteringRecord<T>> records)
        {
            Array.ForEach(clusters, c => c.Records.Clear());
            AssignRecords(clusters, records);
            Array.ForEach(clusters, c => c.TuneCentroid());
        }

        static void AssignRecords(Cluster<T>[] clusters, IEnumerable<ClusteringRecord<T>> records)
        {
            foreach (var record in records)
            {
                var cluster = clusters.FirstToMin(c => ArrayVector.GetDistance(c.Centroid, record.Features));
                cluster.Records.Add(record);
            }
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class Cluster<T>
    {
        public int Id { get; private set; }
        public ArrayVector Centroid { get; private set; }

        internal List<ClusteringRecord<T>> Records { get; private set; }
        public T[] Elements { get { return Records.Select(r => r.Element).ToArray(); } }

        public ArrayVector Mean { get { return Records.Count == 0 ? null : ArrayVector.GetAverage(Records.Select(r => r.Features).ToArray()); } }

        public Cluster(int id, ArrayVector centroid)
        {
            Id = id;
            Centroid = centroid;

            Records = new List<ClusteringRecord<T>>();
        }

        internal void TuneCentroid()
        {
            if (Records.Count == 0) return;

            Centroid = Mean;
        }

        string ToDebugString()
        {
            return string.Format("{0}: {1}: {2} records", Id, Centroid, Records.Count);
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class ClusteringRecord<T>
    {
        public T Element { get; private set; }
        public ArrayVector Features { get; private set; }

        public ClusteringRecord(T element, Func<T, double[]> featuresSelector)
        {
            Element = element;
            Features = featuresSelector(element);
        }

        string ToDebugString()
        {
            return string.Format("{0}: {1}", Element, Features);
        }
    }
}
