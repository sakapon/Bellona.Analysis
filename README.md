## Bellona.Analysis

The library for statistical analysis.

### Setup
To install Bellona.Analysis, run the following command in the Package Manager Console on Visual Studio:

```
Install-Package Bellona.Analysis
```

[NuGet Gallery | Bellona.Analysis](https://www.nuget.org/packages/Bellona.Analysis/)

### Usage

#### Simplest Way
The following code creates a trained clustering model with colors data.

```c#
// Colors data.
var colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

// Initializes and trains a clustering model.
// The lambda expression is to specify features of each color by an array of System.Double.
var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
    .Train(colors);
```

#### Use Result
You can access the trained result via Clusters property.

```c#
// Gets a cluster in the trained model.
var cluster0 = model.Clusters[0];
Console.WriteLine(cluster0.Id);

// Enumerates colors in cluster0.
foreach (var record in cluster0.Records)
{
    Console.WriteLine(record.Element.Name);
}
```

### Prerequisites
* .NET Framework 4.5

### Release Notes
* **v1.0.10** Add the features for clustering.
