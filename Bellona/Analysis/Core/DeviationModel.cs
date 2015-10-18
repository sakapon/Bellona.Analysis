using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bellona.Core
{
    public static class DeviationModel
    {
        public static DeviationModel<T> Create<T>(IList<T> records, Func<T, ArrayVector> featuresSelector)
        {
            return new DeviationModel<T>(records, featuresSelector);
        }
    }

    [DebuggerDisplay(@"\{Records: {Records.Length}\}")]
    public class DeviationModel<T>
    {
        public DeviationRecord<T>[] Records { get; private set; }

        Lazy<ArrayVector> _mean;
        public ArrayVector Mean { get { return _mean.Value; } }

        Lazy<double> _standardDeviation;
        public double StandardDeviation { get { return _standardDeviation.Value; } }

        public DeviationModel(IList<T> records, Func<T, ArrayVector> featuresSelector)
        {
            Records = records.Select(r => new DeviationRecord<T>(this, r, featuresSelector)).ToArray();

            _mean = new Lazy<ArrayVector>(() => ArrayVector.GetAverage(Records.Select(r => r.Features).ToArray()));
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

        public DeviationRecord(DeviationModel<T> model, T element, Func<T, ArrayVector> featuresSelector)
        {
            DeviationModel = model;
            Element = element;
            Features = featuresSelector(element);

            _deviation = new Lazy<double>(() => ArrayVector.GetDistance(DeviationModel.Mean, Features));
            _standardScore = new Lazy<double>(() => Deviation / DeviationModel.StandardDeviation);
        }

        string ToDebugString()
        {
            return string.Format("{0}: {1}", Element, Features);
        }
    }
}
