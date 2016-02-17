# PythonPlotter
Library for using matplotlib from .NET programs (e.g. C#). Currently tested only on *nix platforms

Example usage:

				var x = Enumerable.Range(0, 200).Select(ia => (double)ia / 100.0);
				var y = x.Select(ia => Math.Sin(2.0 * ia * Math.PI));
				Plotter.Plot(x, y, "Test figure", "$x$", @"$\sin(2 \pi x)$");

# NUGET
Available on NuGet at: 
https://www.nuget.org/packages/PythonPlotter/

To install Python Plotter, run the following command in the Package Manager Console

    PM> Install-Package PythonPlotter
