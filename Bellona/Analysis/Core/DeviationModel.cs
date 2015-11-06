using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bellona.Core
{
    /// <summary>
    /// Provides a set of methods for the statistical deviation model.
    /// </summary>
    public static class DeviationModel
    {
        /// <summary>
        /// Initializes a statistical deviation model with the specified elements.
        /// </summary>
        /// <typeparam name="T">The type of the target elements.</typeparam>
        /// <param name="source">A sequence of elements.</param>
        /// <param name="featuresSelector">A function to extract features from each element.</param>
        /// <returns>A statistical deviation model that contains target elements.</returns>
        public static DeviationModel<T> Create<T>(IEnumerable<T> source, Func<T, ArrayVector> featuresSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");

            return new DeviationModel<T>(source, featuresSelector);
        }
    }

    /// <summary>
    /// Represents the statistical deviation model that contains target elements.
    /// This object is immutable.
    /// </summary>
    /// <typeparam name="T">The type of the target elements.</typeparam>
    [DebuggerDisplay(@"\{Records={Records.Length}\}")]
    public class DeviationModel<T>
    {
        /// <summary>
        /// Gets the target records.
        /// </summary>
        public DeviationRecord<T>[] Records { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this model has target records.
        /// </summary>
        public bool HasRecords { get { return Records.Length > 0; } }

        Lazy<ArrayVector> _mean;

        /// <summary>
        /// Gets the mean of features for the records.
        /// </summary>
        public ArrayVector Mean { get { return _mean.Value; } }

        Lazy<double> _standardDeviation;

        /// <summary>
        /// Gets the standard deviation of features for the records.
        /// </summary>
        public double StandardDeviation { get { return _standardDeviation.Value; } }

        internal DeviationModel(IEnumerable<T> source, Func<T, ArrayVector> featuresSelector)
        {
            Records = source.Select(e => new DeviationRecord<T>(this, e, featuresSelector(e))).ToArray();

            _mean = new Lazy<ArrayVector>(() => HasRecords ? ArrayVector.GetAverage(Records.Select(r => r.Features).ToArray()) : null);
            _standardDeviation = new Lazy<double>(() => Math.Sqrt(Records.Sum(r => r.Deviation * r.Deviation) / Records.Length));
        }
    }

    /// <summary>
    /// Represents the record in the statistical deviation model.
    /// This object is immutable.
    /// </summary>
    /// <typeparam name="T">The type of the target element.</typeparam>
    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class DeviationRecord<T>
    {
        /// <summary>
        /// Gets the parent deviation model.
        /// </summary>
        public DeviationModel<T> DeviationModel { get; private set; }

        /// <summary>
        /// Gets the target element.
        /// </summary>
        public T Element { get; private set; }

        /// <summary>
        /// Gets the features of the target element.
        /// </summary>
        public ArrayVector Features { get; private set; }

        Lazy<double> _deviation;

        /// <summary>
        /// Gets the deviation of the features in the records.
        /// The value represents the distance between the mean and this features.
        /// </summary>
        public double Deviation { get { return _deviation.Value; } }

        Lazy<double> _standardScore;

        /// <summary>
        /// Gets the standard score of the features in the records.
        /// The unit is σ (the standard deviation).
        /// </summary>
        public double StandardScore { get { return _standardScore.Value; } }

        internal DeviationRecord(DeviationModel<T> model, T element, ArrayVector features)
        {
            DeviationModel = model;
            Element = element;
            Features = features;

            _deviation = new Lazy<double>(() => ArrayVector.GetDistance(DeviationModel.Mean, Features));
            _standardScore = new Lazy<double>(() => Deviation == 0.0 ? 0.0 : Deviation / DeviationModel.StandardDeviation);
        }

        internal string ToDebugString()
        {
            return string.Format("{0}: {1}", Element, Features.ToDebugString());
        }
    }
}
