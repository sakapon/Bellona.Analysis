﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bellona.Core;

namespace Bellona.Analysis.Clustering
{
    public static class ClusteringModel
    {
        public static ClusteringModel<T> CreateFromNumber<T>(Func<T, ArrayVector> featuresSelector, int clustersNumber)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");
            if (clustersNumber <= 0) throw new ArgumentOutOfRangeException("clustersNumber", clustersNumber, "The value must be positive.");

            return new ClusteringModel<T>(featuresSelector, clustersNumber, new Cluster<T>[0], new ClusteringRecord<T>[0]);
        }
    }

    [DebuggerDisplay(@"\{Clusters={Clusters.Length}, Records={Records.Length}\}")]
    public class ClusteringModel<T>
    {
        Func<T, ArrayVector> _featuresSelector;

        public int ClustersNumber { get; private set; }
        public Cluster<T>[] Clusters { get; private set; }
        public ClusteringRecord<T>[] Records { get; private set; }

        public ClusteringModel(Func<T, ArrayVector> featuresSelector, int clustersNumber, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
        {
            _featuresSelector = featuresSelector;
            ClustersNumber = clustersNumber;
            Clusters = clusters;
            Records = records;
        }

        public ClusteringModel<T> Train(IEnumerable<T> source, int? maxIterations = null)
        {
            if (source == null) throw new ArgumentNullException("source");

            var newRecords = source.Select(e => new ClusteringRecord<T>(e, _featuresSelector(e)));
            var records = Records.Concat(newRecords).ToArray();

            var initial = Clusters.Length > 0 ? Clusters : ClusteringHelper.InitializeClusters(ClustersNumber, records);
            var clusters = ClusteringHelper.TrainIteratively(initial, records, maxIterations);

            return new ClusteringModel<T>(_featuresSelector, ClustersNumber, clusters, records);
        }

        public Cluster<T> Assign(T element)
        {
            if (Clusters.Length == 0) throw new InvalidOperationException("This model is not trained.");

            return ClusteringHelper.AssignFeatures(Clusters, _featuresSelector(element));
        }
    }
}
