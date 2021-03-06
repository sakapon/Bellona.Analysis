﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bellona.Core;
using Bellona.Linq;

namespace Bellona.Analysis.Clustering
{
    /// <summary>
    /// Provides a set of methods for the clustering model.
    /// </summary>
    public static class ClusteringModel
    {
        /// <summary>
        /// Creates an empty clustering model with the specified number of clusters.
        /// </summary>
        /// <typeparam name="T">The type of the target elements.</typeparam>
        /// <param name="featuresSelector">A function to extract features from each element.</param>
        /// <param name="clustersNumber">The number of clusters.</param>
        /// <returns>An empty clustering model in which the number of clusters is fixed.</returns>
        public static ClusteringModel<T> CreateFromNumber<T>(Func<T, ArrayVector> featuresSelector, int clustersNumber)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");
            if (clustersNumber <= 0) throw new ArgumentOutOfRangeException("clustersNumber", clustersNumber, "The value must be positive.");

            return new ClusteringModel<T>(featuresSelector, new Cluster<T>[0], new ClusteringRecord<T>[0], clustersNumber);
        }

        /// <summary>
        /// Creates an empty clustering model in which the number of clusters is determined automatically.
        /// </summary>
        /// <typeparam name="T">The type of the target elements.</typeparam>
        /// <param name="featuresSelector">A function to extract features from each element.</param>
        /// <returns>An empty clustering model in which the number of clusters is determined automatically.</returns>
        public static AutoClusteringModel<T> CreateAuto<T>(Func<T, ArrayVector> featuresSelector)
        {
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");

            return new AutoClusteringModel<T>(featuresSelector, new Cluster<T>[0], new ClusteringRecord<T>[0]);
        }
    }

    /// <summary>
    /// Represents the clustering model that contains target elements.
    /// This object is immutable.
    /// </summary>
    /// <typeparam name="T">The type of the target elements.</typeparam>
    [DebuggerDisplay(@"\{Clusters={Clusters.Length}, Records={Records.Length}\}")]
    public abstract class ClusteringModelBase<T>
    {
        /// <summary>
        /// Gets the function to extract features from each element.
        /// </summary>
        protected Func<T, ArrayVector> FeaturesSelector { get; private set; }

        /// <summary>
        /// Gets the clusters.
        /// </summary>
        public Cluster<T>[] Clusters { get; private set; }

        /// <summary>
        /// Gets the target records.
        /// </summary>
        public ClusteringRecord<T>[] Records { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusteringModelBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="featuresSelector">A function to extract features from each element.</param>
        /// <param name="clusters">Clusters.</param>
        /// <param name="records">Target records.</param>
        protected ClusteringModelBase(Func<T, ArrayVector> featuresSelector, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
        {
            FeaturesSelector = featuresSelector;
            Clusters = clusters;
            Records = records;
        }

        /// <summary>
        /// Assigns the specified element to the most suitable cluster in the clusters of the current model.
        /// </summary>
        /// <param name="element">A target element.</param>
        /// <returns>The cluster that the specified element is assigned to.</returns>
        public Cluster<T> Assign(T element)
        {
            if (Clusters.Length == 0) throw new InvalidOperationException("This model is not trained.");

            return ClusteringHelper.AssignFeatures(Clusters, FeaturesSelector(element));
        }

        /// <summary>
        /// Creates an array of the target elements grouped by the clusters.
        /// </summary>
        /// <returns>An array of the target elements grouped by the clusters.</returns>
        public T[][] ToSimpleArray()
        {
            return Clusters
                .Select(c => c.Records
                    .Select(r => r.Element)
                    .ToArray())
                .ToArray();
        }

        /// <summary>
        /// Creates an array of the target elements grouped by the clusters.
        /// </summary>
        /// <param name="sortKeySelector">A function to extract a sort key from each element.</param>
        /// <returns>An array of the target elements grouped by the clusters.</returns>
        public T[][] ToSimpleArray(Func<T, double> sortKeySelector)
        {
            if (sortKeySelector == null) throw new ArgumentNullException("sortKeySelector");

            return Clusters
                .OrderBy(c => c.Records.Average(r => sortKeySelector(r.Element)))
                .Select(c => c.Records
                    .OrderBy(r => sortKeySelector(r.Element))
                    .Select(r => r.Element)
                    .ToArray())
                .ToArray();
        }
    }

    /// <summary>
    /// Represents the clustering model in which the number of clusters is fixed.
    /// This object is immutable.
    /// </summary>
    /// <typeparam name="T">The type of the target elements.</typeparam>
    public class ClusteringModel<T> : ClusteringModelBase<T>
    {
        /// <summary>
        /// Gets the number of clusters.
        /// </summary>
        public int ClustersNumber { get; private set; }

        internal ClusteringModel(Func<T, ArrayVector> featuresSelector, Cluster<T>[] clusters, ClusteringRecord<T>[] records, int clustersNumber)
            : base(featuresSelector, clusters, records)
        {
            ClustersNumber = clustersNumber;
        }

        /// <summary>
        /// Adds and trains new elements.
        /// </summary>
        /// <param name="source">A sequence of elements.</param>
        /// <param name="maxIterations">The maximum number of iterations.</param>
        /// <returns>A new clustering model that contains the generated clusters.</returns>
        public ClusteringModel<T> Train(IEnumerable<T> source, int? maxIterations = null)
        {
            if (source == null) throw new ArgumentNullException("source");

            var newRecords = source.Select(e => new ClusteringRecord<T>(e, FeaturesSelector(e)));
            var records = Records.Concat(newRecords).ToArray();
            if (records.Length == 0) throw new InvalidOperationException("This model has no records.");

            var initial = Clusters.Length > 0 ? Clusters : ClusteringHelper.Initialize(records, ClustersNumber);
            var clusters = ClusteringHelper.TrainForNumber(initial, records, maxIterations);

            return new ClusteringModel<T>(FeaturesSelector, clusters, records, ClustersNumber);
        }

        /// <summary>
        /// Creates an <see cref="AutoClusteringModel{T}"/> with the data of this model.
        /// The number of clusters is not taken over.
        /// </summary>
        /// <returns>An <see cref="AutoClusteringModel{T}"/> with the data of this model.</returns>
        public AutoClusteringModel<T> ToAutoModel()
        {
            return new AutoClusteringModel<T>(FeaturesSelector, Clusters, Records);
        }
    }

    /// <summary>
    /// Represents the clustering model in which the number of clusters is determined automatically.
    /// This object is immutable.
    /// </summary>
    /// <typeparam name="T">The type of the target elements.</typeparam>
    public class AutoClusteringModel<T> : ClusteringModelBase<T>
    {
        internal AutoClusteringModel(Func<T, ArrayVector> featuresSelector, Cluster<T>[] clusters, ClusteringRecord<T>[] records)
            : base(featuresSelector, clusters, records)
        {
        }

        /// <summary>
        /// Adds and trains new elements.
        /// </summary>
        /// <param name="source">A sequence of elements.</param>
        /// <param name="maxClustersNumber">The maximum number of clusters.</param>
        /// <param name="maxStandardScore">The maximum standard score for each cluster.</param>
        /// <returns>A new clustering model that contains the generated clusters.</returns>
        public AutoClusteringModel<T> Train(IEnumerable<T> source, int? maxClustersNumber = null, double maxStandardScore = 1.645)
        {
            if (source == null) throw new ArgumentNullException("source");

            var newRecords = source.Select(e => new ClusteringRecord<T>(e, FeaturesSelector(e)));
            var records = Records.Concat(newRecords).ToArray();
            if (records.Length == 0) throw new InvalidOperationException("This model has no records.");

            var initial = Clusters.Length > 0 ? Clusters : new Cluster<T>(0, records.Take(1)).MakeArray();
            var clusters = ClusteringHelper.TrainForAuto(initial, records, maxClustersNumber, maxStandardScore);

            return new AutoClusteringModel<T>(FeaturesSelector, clusters, records);
        }

        /// <summary>
        /// Creates a <see cref="ClusteringModel{T}"/> with the data of this model.
        /// The number of clusters is also taken over.
        /// </summary>
        /// <returns>A <see cref="ClusteringModel{T}"/> with the data of this model.</returns>
        public ClusteringModel<T> ToFixedModel()
        {
            return new ClusteringModel<T>(FeaturesSelector, Clusters, Records, Clusters.Length);
        }
    }
}
