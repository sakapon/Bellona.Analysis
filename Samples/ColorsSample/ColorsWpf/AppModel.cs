using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Bellona.Analysis.Clustering;

namespace ColorsWpf
{
    public class AppModel
    {
        public ColorCluster[] ColorClusters { get; private set; }

        public AppModel()
        {
            var colors = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(null))
                .Where(c => c.A == 255) // Exclude Transparent.
                .ToArray();

            var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
                .Train(colors);

            ColorClusters = model
                .ToSimpleArray(c => c.GetHue())
                .Select(cs => cs.Select(c => new ColorInfo(c)).ToArray())
                .Select((cs, i) => new ColorCluster { Id = i, Colors = cs })
                .ToArray();
        }
    }

    public struct ColorCluster
    {
        public int Id { get; set; }
        public ColorInfo[] Colors { get; set; }
    }

    [DebuggerDisplay(@"\{{Color}\}")]
    public class ColorInfo
    {
        public Color Color { get; private set; }

        public string Name { get { return Color.Name; } }
        public string RGB { get { return string.Format("#{0:X2}{1:X2}{2:X2}", Color.R, Color.G, Color.B); } }

        public ColorInfo(Color color)
        {
            Color = color;
        }
    }
}
