using System;
using System.Collections.Generic;
using System.Linq;

namespace Bellona.Linq
{
    /// <summary>
    /// Provides a set of methods to extend LINQ to Objects.
    /// </summary>
    public static class Enumerable2
    {
        /// <summary>
        /// Creates an <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> from a single object.
        /// </summary>
        /// <typeparam name="TResult">The type of the object.</typeparam>
        /// <param name="element">An object.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> that contains the input object.</returns>
        public static IEnumerable<TResult> ToEnumerable<TResult>(this TResult element)
        {
            yield return element;
        }

        public static IEnumerable<TResult> Repeat<TResult>(TResult element)
        {
            while (true)
                yield return element;
        }

        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int? count)
        {
            return count.HasValue ? Enumerable.Repeat(element, count.Value) : Repeat(element);
        }

        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static void Execute<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            foreach (var item in source) ;
        }

        public static void Execute<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
                action(item);
        }

        public static IEnumerable<TSource> Distinct<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector)
        {
            if (source == null) throw new ArgumentNullException("source");

            var valueSet = new HashSet<TValue>();

            foreach (var item in source)
            {
                var value = selector(item);
                if (!valueSet.Add(value)) continue;
                yield return item;
            }
        }

        public static IEnumerable<TSource[]> Segment<TSource>(this IEnumerable<TSource> source, int lengthInSegment)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (lengthInSegment <= 0) throw new ArgumentOutOfRangeException("lengthInSegment", lengthInSegment, "The value must be positive.");

            var l = new List<TSource>();

            foreach (var item in source)
            {
                l.Add(item);

                if (l.Count == lengthInSegment)
                {
                    yield return l.ToArray();
                    l.Clear();
                }
            }

            if (l.Count > 0)
                yield return l.ToArray();
        }

        public static TSource FirstToMin<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var o = source
                .Select(x => new { x, v = selector(x) })
                .Aggregate((o1, o2) => o1.v <= o2.v ? o1 : o2);
            return o.x;
        }

        public static TSource FirstToMax<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var o = source
                .Select(x => new { x, v = selector(x) })
                .Aggregate((o1, o2) => o1.v >= o2.v ? o1 : o2);
            return o.x;
        }

        public static TSource LastToMin<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var o = source
                .Select(x => new { x, v = selector(x) })
                .Aggregate((o1, o2) => o1.v < o2.v ? o1 : o2);
            return o.x;
        }

        public static TSource LastToMax<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var o = source
                .Select(x => new { x, v = selector(x) })
                .Aggregate((o1, o2) => o1.v > o2.v ? o1 : o2);
            return o.x;
        }
    }
}
