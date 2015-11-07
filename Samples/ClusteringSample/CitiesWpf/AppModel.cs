using System;
using System.Collections.Generic;
using System.Linq;

namespace CitiesWpf
{
    public class AppModel
    {
        public AppModel()
        {
            var CityType = EntityType.Create(new { CityName = "", Latitude = 0.0, Longitude = 0.0 });
            var cities = CsvFile.ReadEntities("Cities.csv", CityType)
                .ToArray();
        }
    }
}
