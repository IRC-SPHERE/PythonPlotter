# PythonPlotter
Library for using matplotlib from .NET programs (e.g. C#). Currently tested only on *nix platforms.

Python packages required: numpy, matplotlib, seaborn, mpltools.

Example usage:

```C#
var x = Enumerable.Range(0, 200).Select(ia => (double)ia / 100.0);
var y = x.Select(ia => Math.Sin(2.0 * ia * Math.PI));
Plotter.Plot(x, y, "Test figure", "$x$", @"$\sin(2 \pi x)$");
```

![line plot](https://github.com/IRC-SPHERE/PythonPlotter/blob/master/test_figure.png "Line plot")

```C#
var random = new Random(0);
var data = Enumerable.Range(0, 20).Select(
	i => Enumerable.Range(0, 10).Select(
            j => random.NextDouble() - 0.5).ToArray()).ToArray();
var plotter = new Plotter
{
    Series = new ISeries[] { new HintonSeries { Values = data } },
    Title = "Hinton diagram"
};
plotter.Plot();
```

![hinton diagram](https://github.com/IRC-SPHERE/PythonPlotter/blob/master/hinton.png "Hinton diagram")

# NUGET
Available on NuGet at: 
https://www.nuget.org/packages/PythonPlotter/

To install Python Plotter, run the following command in the Package Manager Console

    PM> Install-Package PythonPlotter
