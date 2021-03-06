## Bellona.Analysis
[![license](https://img.shields.io/github/license/sakapon/Bellona.Analysis.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Bellona.Analysis.svg)](https://www.nuget.org/packages/Bellona.Analysis/)
[![NuGet](https://img.shields.io/nuget/dt/Bellona.Analysis.svg)](https://www.nuget.org/packages/Bellona.Analysis/)

The library for statistical analysis.

### Setup
To install Bellona.Analysis, run the following command in the Package Manager Console on Visual Studio:

```
Install-Package Bellona.Analysis
```

[NuGet Gallery | Bellona.Analysis](https://www.nuget.org/packages/Bellona.Analysis/)

### Usage of Clustering

#### Simplest Way
The following code creates a trained clustering model with colors data.

```c#
// Colors data.
var colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

// Initializes and trains a clustering model.
// The lambda expression is to extract features from each color by an array of System.Double.
var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
    .Train(colors);
```

The number of clusters doesn't need to be specified for `CreateAuto` method.

Remark that `ClusteringModel<T>` objects are immutable.
`CreateAuto` method returns an empty model, and `Train` method returns a trained model.
So use method chaining.

#### Use Result
You can access the trained result via `Clusters` property.

```c#
// Gets a cluster in the trained model.
var cluster0 = model.Clusters[0];
Console.WriteLine(cluster0.Id);

// Enumerates colors in cluster0.
foreach (var record in cluster0.Records)
    Console.WriteLine(record.Element.Name);
```

#### Specify Parameters

```c#
// Specifies the maximum number of clusters and the maximum standard score in σ.
var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
    .Train(colors, 20, 1.5)
    .Train(colors2, 30, 1.4);
```

Use `CreateFromNumber` method to fix the number of clusters.

```c#
// Specifies the number of clusters and the maximum number of iterations.
var model = ClusteringModel.CreateFromNumber<Color>(c => new double[] { c.R, c.G, c.B }, 10)
    .Train(colors, 30);
```

#### Assign Element
Remark that `Assign` method doesn't train the model with the new data.

```c#
// Assigns the gold to the suitable cluster.
var cluster = model.Assign(Color.Gold);
```

#### Convert Model Type

```c#
var autoModel = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
    .Train(colors);

// Converts AutoClusteringModel to ClusteringModel.
var fixedModel = autoModel.ToFixedModel()
    .Train(colors2);
```

```c#
var fixedModel = ClusteringModel.CreateFromNumber<Color>(c => new double[] { c.R, c.G, c.B }, 10)
    .Train(colors);

// Converts ClusteringModel to AutoClusteringModel.
var autoModel = fixedModel.ToAutoModel()
    .Train(colors2);
```

### Samples
The sample source code is [ClusteringSample](Samples/ClusteringSample).

Clustering colors in the [System.Drawing.Color structure](https://msdn.microsoft.com/library/system.drawing.color.aspx):  
![ColorClusters](Images/Clustering/ColorClusters.png)

Clustering the prefectural capitals in Japan by their positions:  
![CityClusters](Images/Clustering/CityClusters.png)

### Target Frameworks
- .NET Standard 2.0
- .NET Framework 4.5

### Release Notes
- **v2.0.15** For .NET Standard.
- **v1.1.14** Fix bugs.
- **v1.1.13** Add minor utility methods.
- **v1.0.10** Add the features for clustering.
