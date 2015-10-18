using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Bellona.Core;

namespace UnitTest
{
    static class TestData
    {
        public static Color[] GetColors()
        {
            return typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(null))
                .Where(c => c.A == 255) // Exclude Transparent.
                .ToArray();
        }

        public static SamplePoint[] GetRandomPoints()
        {
            return Enumerable.Range(0, 100)
                .Select(i => new SamplePoint
                {
                    Id = i,
                    Point = Enumerable.Range(1, 3).Select(j => RandomHelper.NextDouble(j)).ToArray(),
                })
                .ToArray();
        }
    }

    class SamplePoint
    {
        public int Id { get; set; }
        public double[] Point { get; set; }
    }
}
