﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Bellona.Clustering;

namespace ColorsWpf
{
    public class AppModel
    {
        public ColorInfo[][] ColorClusters { get; private set; }

        public AppModel()
        {
            var colors = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(null))
                .Where(c => c.A == 255) // Exclude Transparent.
                .ToArray();

            var model = new ClusteringModel<Color>(c => new double[] { c.R, c.G, c.B });
            model.Train(colors);

            ColorClusters = model.Clusters
                .Select(c => c.Records
                    .Select(r => new ColorInfo(r.Element))
                    .OrderBy(ci => ci.Color.GetHue())
                    .ToArray())
                .OrderBy(cs => cs.Average(c => c.Color.GetHue()))
                .ToArray();
        }
    }

    [DebuggerDisplay(@"\{{Color}\}")]
    public class ColorInfo
    {
        public Color Color { get; private set; }
        public string RGB { get { return string.Format("#{0:X2}{1:X2}{2:X2}", Color.R, Color.G, Color.B); } }

        public ColorInfo(Color color)
        {
            Color = color;
        }
    }
}
