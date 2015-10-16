using System;
using System.Collections.Generic;
using System.Linq;

namespace Bellona.Core
{
    public static class RandomHelper
    {
        static readonly Random _random = new Random();

        public static double NextDouble(double minValue, double maxValue)
        {
            return minValue + (maxValue - minValue) * _random.NextDouble();
        }

        public static IEnumerable<int> ShuffleRange(int maxValue)
        {
            if (maxValue < 0) throw new ArgumentOutOfRangeException("maxValue", "maxValue is less than 0.");

            return ShuffleRange(0, maxValue);
        }

        public static IEnumerable<int> ShuffleRange(int minValue, int maxValue)
        {
            if (maxValue < minValue) throw new ArgumentOutOfRangeException("maxValue", "maxValue is less than minValue.");

            var l = Enumerable.Range(minValue, maxValue - minValue).ToList();

            while (l.Count > 0)
            {
                var index = _random.Next(l.Count);
                yield return l[index];
                l.RemoveAt(index);
            }
        }

        public static T GetRandomElement<T>(this IList<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.Count == 0) throw new ArgumentException("The array must have elements.", "source");

            return source[_random.Next(source.Count)];
        }

        public static IEnumerable<T> GetRandomPiece<T>(this IList<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count", count, "The value must be non-negative.");

            if (count == 0) return Enumerable.Empty<T>();
            if (count >= source.Count) return source;

            return Enumerable.Range(_random.Next(source.Count - count + 1), count)
                .Select(i => source[i]);
        }
    }
}
