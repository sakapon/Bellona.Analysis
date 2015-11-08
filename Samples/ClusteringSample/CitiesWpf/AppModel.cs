using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Bellona.Analysis.Clustering;
using Bellona.Core;

namespace CitiesWpf
{
    public class AppModel
    {
        public ClusteredCity[] ClusteredCities { get; private set; }

        public AppModel()
        {
            var CityType = EntityType.Create(default(City));
            var cities = CsvFile.ReadEntities("Cities.csv", CityType).ToArray();
            var model = ClusteringModel.CreateAuto<City>(c => new[] { c.Latitude, c.Longitude })
                .Train(cities);

            var colors = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(null))
                .Where(c => c.A == 255) // Exclude Transparent.
                .Where(c => c.GetSaturation() <= 0.9)
                .Where(c => c.GetBrightness() <= 0.7)
                .ToArray();
            var colorsToDisplay = ClusteringModel.CreateFromNumber<Color>(c => new double[] { c.GetHue() }, model.Clusters.Length)
                .Train(colors)
                .Clusters
                .Select(c => c.Records.GetRandomElement().Element)
                .ToArray();

            ClusteredCities = model.Clusters
                .SelectMany(c => c.Records.Select(r => new ClusteredCity { City = r.Element, Color = new Color2(colorsToDisplay[c.Id]) }))
                .OrderBy(c => c.City.PrefectureId)
                .ToArray();
        }
    }

    [DebuggerDisplay(@"\{{CityName}\}")]
    public class City
    {
        public int PrefectureId { get; private set; }
        public string CityName { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public City(int prefectureId, string cityName, double latitude, double longitude)
        {
            PrefectureId = prefectureId;
            CityName = cityName;
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    [DebuggerDisplay(@"\{{Color}\}")]
    public class Color2
    {
        public Color Color { get; private set; }

        public string Name { get { return Color.Name; } }
        public string RGB { get { return string.Format("#{0:X2}{1:X2}{2:X2}", Color.R, Color.G, Color.B); } }

        public Color2(Color color)
        {
            Color = color;
        }
    }

    [DebuggerDisplay(@"\{City={City.CityName}, Color={Color.Name}\}")]
    public struct ClusteredCity
    {
        public City City { get; set; }
        public Color2 Color { get; set; }
    }
}
