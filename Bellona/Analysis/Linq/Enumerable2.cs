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
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="obj">An object.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> that contains the input object.</returns>
        public static IEnumerable<TSource> ToEnumerable<TSource>(this TSource obj)
        {
            yield return obj;
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

            foreach (var item in source)
            {
            }
        }

        public static void Execute<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
            {
                action(item);
            }
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
