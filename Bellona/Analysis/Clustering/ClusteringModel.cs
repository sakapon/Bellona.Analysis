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

        Func<T, ArrayVector> _featuresSelector;

        List<ClusteringRecord<T>> _records = new List<ClusteringRecord<T>>();
        public ClusteringRecord<T>[] Records { get { return _records.ToArray(); } }

        public ClusteringModel(int clustersNumber, Func<T, ArrayVector> featuresSelector)
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
                Clusters = TrainOnce(Clusters, _records);
        }

        public Cluster<T> AssignElement(T element)
        {
            if (Clusters == null) throw new InvalidOperationException("This model is not trained.");

            var features = _featuresSelector(element);
            return Clusters.FirstToMin(c => ArrayVector.GetDistance(c.Centroid, features));
        }

        static Cluster<T>[] InitializeClusters(int clustersNumber, IList<ClusteringRecord<T>> records)
        {
            return RandomHelper.ShuffleRange(records.Count)
                .Select(i => records[i])
                .Distinct(r => r.Features)
                .Select((r, i) => new Cluster<T>(i, r.ToEnumerable()))
                .Take(clustersNumber)
                .ToArray();
        }

        static Cluster<T>[] TrainOnce(Cluster<T>[] clusters, IEnumerable<ClusteringRecord<T>> records)
        {
            return records
                .GroupBy(r => clusters.FirstToMin(c => ArrayVector.GetDistance(c.Centroid, r.Features)))
                .OrderBy(g => g.Key.Id)
                .Select((g, i) => new Cluster<T>(i, g))
                .ToArray();
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class Cluster<T>
    {
        public int Id { get; private set; }
        public ClusteringRecord<T>[] Records { get; private set; }

        public DeviationModel<ClusteringRecord<T>> DeviationInfo { get; private set; }
        public ArrayVector Centroid { get { return DeviationInfo == null ? null : DeviationInfo.Mean; } }

        public Cluster(int id, IEnumerable<ClusteringRecord<T>> records)
        {
            Id = id;
            Records = records.ToArray();

            if (Records.Length == 0) return;
            DeviationInfo = DeviationModel.Create(Records, r => r.Features);
        }

        string ToDebugString()
        {
            return string.Format("{0}: {1}: {2} records", Id, Centroid, Records.Length);
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class ClusteringRecord<T>
    {
        public T Element { get; private set; }
        public ArrayVector Features { get; private set; }

        public ClusteringRecord(T element, Func<T, ArrayVector> featuresSelector)
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
