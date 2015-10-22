using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bellona.Core
{
    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public class ArrayVector
    {
        public double[] Value { get; private set; }

        public int Dimension { get { return Value.Length; } }

        Lazy<double> _norm;
        public double Norm { get { return _norm.Value; } }

        public ArrayVector(double[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length == 0) throw new ArgumentException("The dimension of the array must be positive.", "value");

            Value = value;

            _norm = new Lazy<double>(() => Math.Sqrt(Value.Sum(x => x * x)));
        }

        public static implicit operator ArrayVector(double[] value)
        {
            return new ArrayVector(value);
        }

        public static bool operator ==(ArrayVector v1, ArrayVector v2)
        {
            return Enumerable.SequenceEqual(v1.Value, v2.Value);
        }

        public static bool operator !=(ArrayVector v1, ArrayVector v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            return obj is ArrayVector && this == (ArrayVector)obj;
        }

        public override int GetHashCode()
        {
            return Value[0].GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(",", Value);
        }

        internal string ToDebugString()
        {
            return string.Join(", ", Value.Select(x => x.ToString("F3")));
        }

        public static double GetDistance(ArrayVector v1, ArrayVector v2)
        {
            return GetDistance(v1.Value, v2.Value);
        }

        public static double GetDistance(double[] v1, double[] v2)
        {
            return Math.Sqrt(v1.Zip(v2, (x1, x2) => x1 - x2).Sum(x => x * x));
        }

        public static ArrayVector GetAverage(IList<ArrayVector> vectors)
        {
            if (vectors == null) throw new ArgumentNullException("vectors");
            if (vectors.Count == 0) throw new ArgumentException("The source must not be empty.", "vectors");

            return Enumerable.Range(0, vectors[0].Dimension)
                .Select(i => vectors.Average(v => v.Value[i]))
                .ToArray();
        }
    }
}
