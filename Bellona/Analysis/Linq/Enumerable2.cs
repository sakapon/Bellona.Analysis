using System;
using System.Collections.Generic;
using System.Linq;

namespace Bellona.Linq
{
    public static class Enumerable2
    {
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
