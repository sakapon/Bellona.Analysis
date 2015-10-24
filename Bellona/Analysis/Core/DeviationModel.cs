using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bellona.Core
{
    public static class DeviationModel
    {
        public static DeviationModel<T> Create<T>(IEnumerable<T> source, Func<T, ArrayVector> featuresSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (featuresSelector == null) throw new ArgumentNullException("featuresSelector");

            return new DeviationModel<T>(source, featuresSelector);
        }
    }

    [DebuggerDisplay(@"\{Records={Records.Length}\}")]
    public class DeviationModel<T>
    {
        public DeviationRecord<T>[] Records { get; private set; }
        public bool HasRecords { get { return Records.Length > 0; } }

        Lazy<ArrayVector> _mean;
        public ArrayVector Mean { get { return _mean.Value; } }

        Lazy<double> _standardDeviation;
        public double StandardDeviation { get { return _standardDeviation.Value; } }

        internal DeviationModel(IEnumerable<T> source, Func<T, ArrayVector> featuresSelector)
        {
            Records = source.Select(e => new DeviationRecord<T>(this, e, featuresSelector(e))).ToArray();

            _mean = new Lazy<ArrayVector>(() => HasRecords ? ArrayVector.GetAverage(Records.Select(r => r.Features).ToArray()) : null);
            _standardDeviation = new Lazy<double>(() => Math.Sqrt(Records.Sum(r => r.Deviation * r.Deviation) / Records.Length));
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class DeviationRecord<T>
    {
        public DeviationModel<T> DeviationModel { get; private set; }
        public T Element { get; private set; }
        public ArrayVector Features { get; private set; }

        Lazy<double> _deviation;
        public double Deviation { get { return _deviation.Value; } }

        Lazy<double> _standardScore;
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
