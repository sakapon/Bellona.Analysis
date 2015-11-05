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
// Data.
var colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

// Initializes and trains the model.
var model = ClusteringModel.CreateAuto<Color>(c => new double[] { c.R, c.G, c.B })
    .Train(colors);
```
