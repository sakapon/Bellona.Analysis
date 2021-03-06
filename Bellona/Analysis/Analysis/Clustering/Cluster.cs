﻿using System;
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
        public bool HasRecords { get { return Records.Length > 0; } }

        public DeviationModel<ClusteringRecord<T>> DeviationInfo { get; private set; }
        public ArrayVector Centroid { get { return DeviationInfo.Mean; } }

        public Cluster(int id, IEnumerable<ClusteringRecord<T>> records)
        {
            Id = id;
            Records = records.ToArray();

            DeviationInfo = DeviationModel.Create(Records, r => r.Features);
        }

        internal string ToDebugString()
        {
            return string.Format("Id={0}, Records={1}, Centroid={2}", Id, Records.Length, Centroid.ToDebugString());
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class ClusteringRecord<T>
    {
        public T Element { get; private set; }
        public ArrayVector Features { get; private set; }

        public ClusteringRecord(T element, ArrayVector features)
        {
            Element = element;
            Features = features;
        }

        internal string ToDebugString()
        {
            return string.Format("{0}: {1}", Element, Features.ToDebugString());
        }
    }
}
