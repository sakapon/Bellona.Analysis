using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bellona.Core;

namespace Bellona.Analysis.Clustering
{
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

        internal string ToDebugString()
        {
            return string.Format("{0}: {1}: {2} records", Id, Centroid.ToDebugString(), Records.Length);
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

        internal string ToDebugString()
        {
            return string.Format("{0}: {1}", Element, Features.ToDebugString());
        }
    }
}
