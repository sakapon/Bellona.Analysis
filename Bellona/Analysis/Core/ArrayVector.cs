using System;
using System.Collections.Generic;
using System.Linq;

namespace Bellona.Core
{
    public class ArrayVector
    {
        public double[] Value { get; private set; }

        public int Dimension { get { return Value.Length; } }
        public double Norm { get { return Math.Sqrt(Value.Sum(x => x * x)); } }

        public ArrayVector(double[] value)
        {
            if (value == null) throw new ArgumentNullException("value");

            Value = value;
        }

        public static implicit operator ArrayVector(double[] value)
        {
            return new ArrayVector(value);
        }

        public override string ToString()
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
            if (vectors.Count == 0) throw new ArgumentException("The list must not be empty.", "vectors");

            return Enumerable.Range(0, vectors[0].Dimension)
                .Select(i => vectors.Average(v => v.Value[i]))
                .ToArray();
        }
    }
}
