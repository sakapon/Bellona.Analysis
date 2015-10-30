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
        public static IEnumerable<TResult> MakeEnumerable<TResult>(this TResult element)
        {
            yield return element;
        }

        /// <summary>
        /// Creates an array from a single object.
        /// </summary>
        /// <typeparam name="TResult">The type of the object.</typeparam>
        /// <param name="element">An object.</param>
        /// <returns>An array that contains the input object.</returns>
        public static TResult[] MakeArray<TResult>(this TResult element)
        {
            return new[] { element };
        }

        /// <summary>
        /// Generates an infinite sequence that contains one repeated value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
        /// <param name="element">The value to be repeated.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> that contains a repeated value.</returns>
        public static IEnumerable<TResult> Repeat<TResult>(TResult element)
        {
            while (true)
                yield return element;
        }

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
        /// <param name="element">The value to be repeated.</param>
        /// <param name="count">The number of times to repeat the value in the generated sequence. <see langword="null"/> if the value is repeated infinitely.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> that contains a repeated value.</returns>
        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int? count)
        {
            return count.HasValue ? Enumerable.Repeat(element, count.Value) : Repeat(element);
        }

        /// <summary>
        /// Does an action for each element of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="action">An action to apply to each element.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> that contains the same elements as the input sequence.</returns>
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

        /// <summary>
        /// Executes enumeration of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values.</param>
        public static void Execute<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            foreach (var item in source) ;
        }

        /// <summary>
        /// Executes enumeration of a sequence, and does an action for each element of the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="action">An action to apply to each element.</param>
        public static void Execute<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
                action(item);
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            var keySet = new HashSet<TKey>();

            foreach (var item in source)
            {
                var key = keySelector(item);
                if (!keySet.Add(key)) continue;
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
