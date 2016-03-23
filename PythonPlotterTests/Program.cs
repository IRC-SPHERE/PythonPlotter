//
// PythonPlotterTests.cs
//
// Author:
//       Tom Diethe <tom.diethe@bristol.ac.uk>
//
// Copyright (c) 2015 University of Bristol
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using PythonPlotter;

namespace PythonPlotterTests
{
    internal class Program
    {
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
        private static void Main(string[] args)
        {
            Plotter.Demo();
            Hinton();
			MatShow();
			Subplots();
        }

		/// <summary>
		/// Test Hinton diagrams.
		/// </summary>
		public static void Hinton()
		{
			var plotter = new Plotter();
			var random = new Random(0);
			var data = Enumerable.Range(0, 20).Select(
				           i => Enumerable.Range(0, 10).Select(
					           j => random.NextDouble() - 0.5).ToArray()).ToArray();
			plotter.Series = new ISeries[] { new HintonSeries { Values = data } };
		    plotter.Title = "Hinton diagram";
			plotter.Plot();
		}

		/// <summary>
		/// Test MatShow.
		/// </summary>
		public static void MatShow()
		{
			var plotter = new Plotter { Grid = false };
			var random = new Random(0);
			var data = Enumerable.Range(0, 20).Select(
				i => Enumerable.Range(0, 10).Select(
					j => random.NextDouble() - 0.5).ToArray()).ToArray();
			plotter.Series = new ISeries[] { new MatrixSeries { Values = data, ColorMap = "gray" } };
			plotter.Title = "MatShow";
			plotter.Plot();
		}

		public static void Subplots()
		{
			var zeros = Enumerable.Repeat(0.0, 150).ToArray();
			var ones = Enumerable.Repeat(1.0, 100).ToArray();
			var sine = Enumerable.Range(0, 400).Select(x => Math.Sin(x / 10.0)).ToArray();

			// zObs = new[] { zeros.Concat(ones).Concat(zeros).ToArray(), sine }; 
			var plotter = new Plotter { Subplots = new Subplots { Rows = 2, Columns = 1 }, Title = "Subplots" };
			plotter.Series = new ISeries[] { new LineSeries { X = zeros.Concat(ones).Concat(zeros).ToArray(), Row = 0 }, new LineSeries { X = sine, Row = 1 } };
			plotter.Plot();
		}
    }
}
