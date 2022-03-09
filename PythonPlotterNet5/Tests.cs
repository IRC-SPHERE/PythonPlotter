//
// Tests.cs
//
// Author:
//       Tom Diethe <tom.diethe@bristol.ac.uk>
//
// Copyright (c) 2016 University of Bristol
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

namespace PythonPlotter
{
	using System;
	using System.Linq;

	public static class Tests
	{
		/// <summary>
		/// Test Hinton diagrams.
		/// </summary>
		public static void Hinton(string python = "/usr/bin/python")
		{
            var plotter = new Plotter { Grid = false, Python = python };
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
		public static void MatShow(string python = "/usr/bin/python")
		{
			var plotter = new Plotter { Grid = false, Python = python };
			var random = new Random(0);
			var data = Enumerable.Range(0, 20).Select(
				i => Enumerable.Range(0, 10).Select(
					j => random.NextDouble() - 0.5).ToArray()).ToArray();
			plotter.Series = new ISeries[] { new MatrixSeries { Values = data, ColorMap = "gray" } };
			plotter.Title = "MatShow";
			plotter.Plot();
		}

        /// <summary>
        /// Tests Subplots.
        /// </summary>
		public static void Subplots(string python = "/usr/bin/python")
		{
			var zeros = Enumerable.Repeat(0.0, 150).ToArray();
			var ones = Enumerable.Repeat(1.0, 100).ToArray();
			var sine = Enumerable.Range(0, 400).Select(x => Math.Sin(x / 10.0)).ToArray();

			var plotter = new Plotter
			{
				Subplots = new Subplots { Rows = 2, Columns = 1 }, Title = "Subplots", Python = python,
				Series = new ISeries[] 
				{
					new LineSeries { X = zeros.Concat(ones).Concat(zeros).ToArray(), Row = 0, Label = "square" }, 
					new LineSeries { X = sine, Row = 1, Label = "$sin(x)$" } 
				}
			};
			plotter.Plot();
		}
	}
}

