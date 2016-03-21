//
// PythonPlotter.cs
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


namespace PythonPlotter
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Linq;

	/// <summary>
	/// Plot type.
	/// </summary>
	public enum PlotType
	{
		Line,
		ErrorLine,
		Bar
	}

	/// <summary>
	/// Legend position.
	/// </summary>
	public enum LegendPosition
	{
		Best,
		UpperRight,
		UpperLeft,
		LowerLeft,
		LowerRight,
		Right,
		CenterLeft,
		CenterRight,
		LowerCenter,
		UpperCenter,
		Center
	}

	/// <summary>
	/// Python plotter. Currently *NIX only
	/// </summary>
	public class Plotter
	{
		/// <summary>
		/// Gets or sets the python executable.
		/// </summary>
		/// <value>The python.</value>
		public string Python { get; set; } = "/usr/bin/python";

		/// <summary>
		/// Gets or sets the series.
		/// </summary>
		/// <value>The series.</value>
		public IList<ISeries> Series { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the X label.
		/// </summary>
		/// <value>The X label.</value>
		public string XLabel { get; set; }

		/// <summary>
		/// Gets or sets the Y label.
		/// </summary>
		/// <value>The Y label.</value>
		public string YLabel { get; set; }

		/// <summary>
		/// Gets or sets the X lim.
		/// </summary>
		/// <value>The X lim.</value>
		public Tuple<double, double> XLim { get; set; }

		/// <summary>
		/// Gets or sets the Y lim.
		/// </summary>
		/// <value>The Y lim.</value>
		public Tuple<double, double> YLim { get; set; }

		/// <summary>
		/// Gets or sets the legend position.
		/// </summary>
		/// <value>The legend position.</value>
		public LegendPosition LegendPosition { get; set; }

		/// <summary>
		/// Gets or sets the name of the script.
		/// </summary>
		/// <value>The name of the script.</value>
		public string ScriptName { get; set; } = "/tmp/script.py";

		/// <summary>
		/// Gets or sets the name of the pdf figure.
		/// </summary>
		/// <value>The name of the pdf figure.</value>
		public string FigureName
		{ 
			get
			{
			    var figureName = string.IsNullOrEmpty(ScriptName)
			        ? "plot.pdf"
			        : (ScriptName.EndsWith(".py", StringComparison.Ordinal)
			            ? ScriptName.Substring(
			                0,
			                ScriptName.Length - 3) + ".pdf"
			            : ScriptName);
                
				figureName = figureName.EndsWith(".pdf", StringComparison.Ordinal) ? figureName : figureName + ".pdf";
				figureName = figureName.Replace(":", "-");
				return figureName;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PythonPlotter"/> is shown.
		/// </summary>
		/// <value><c>true</c> if show; otherwise, <c>false</c>.</value>
		public bool Show { get; set; } = true;

		/// <summary>
		/// Gets or sets the subplots.
		/// </summary>
		/// <value>The subplots.</value>

		public Subplots Subplots { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PythonPlotter"/> is blocking.
		/// </summary>
		/// <value><c>true</c> if block; otherwise, <c>false</c>.</value>
		public bool Block { get; set; } = true;

	    /// <summary>
        /// Gets or sets a value indicating whether to use SeaBorn library.
        /// </summary>
        public bool SeaBorn { get; set; } = true;

		/// <summary>
		/// Do the plotting.
		/// </summary>
		public void Plot()
		{
			var script = new StringBuilder();
			script.AppendLine("import warnings;");
			#if FALSE
            {
                // Disable warnings for pylab only
                script.AppendLine("with warnings.catch_warnings():");
                script.AppendLine("    warnings.simplefilter('ignore');");
                script.AppendLine("    from pylab import *");
            }
			#else
			{
				// Globally disable warnings 
				script.AppendLine("warnings.simplefilter('ignore');"); 
				script.AppendLine("from pylab import *");
			}
			#endif

		    if (SeaBorn)
		    {
		        script.AppendLine("import seaborn as sns");
		        script.AppendLine("sns.set(style='darkgrid')");
		        script.AppendLine("sns.set_context('paper')");
		    }

		    var scriptName = string.IsNullOrEmpty(ScriptName) ? "script.py" : (ScriptName.EndsWith(
				                    ".py",
				                    StringComparison.Ordinal) ? ScriptName : ScriptName + ".py");
			scriptName = scriptName.Replace(":", "-").Replace("/", "-");

		    script.AppendLine(
		        Subplots != null
		            ? $"fig, axs = subplots({Subplots.Rows}, {Subplots.Columns}, sharex={Subplots.ShareX}, sharey={Subplots.ShareY})"
		            : "fig, ax = subplots()");

		    // var legend = new List<string>();
			foreach (var line in Series)
			{
				// legend.Add("'" + line.Label + "'");
				if (Subplots != null)
				{
					if (Subplots.Rows > 1 && Subplots.Columns > 1)
					{
					    script.AppendLine($"ax = axs[{line.Row}, {line.Column}]");
					}
					else if (Subplots.Rows == 1 && Subplots.Columns > 1)
					{
					    script.AppendLine($"ax = axs[{line.Column}]");
					}
					else if (Subplots.Columns == 1 && Subplots.Rows > 1)
					{
					    script.AppendLine($"ax = axs[{line.Row}]");
					}
					else if (Subplots.Rows == 1 && Subplots.Columns == 1)
					{
						// Just in case someone specificies subplots(1, 1) for some reason
						script.AppendFormat("ax = axs\n", line.Row);
					}
					else
					{
						throw new InvalidOperationException("Invalid row/column specificiation for subplots");
					}
                    
					ConfigureAxis(script, true);
				}
                
				line.Plot(script);
			}

		    script.AppendLine($"{(Subplots == null ? "" : "fig.sup")}title('{Title}', fontsize=16)");
			ConfigureAxis(script, false);
            
			// script.AppendFormat("ax.legend([{0}], loc={1})\n", string.Join(", ", legend), (int)LegendPosition);

		    script.AppendLine(
		        Series.Any(ia => !string.IsNullOrEmpty(ia.Label))
		            ? $"fig.savefig('{FigureName}', format='pdf', bbox_extra_artists=(lgd,), bbox_inches='tight')"
		            : $"fig.savefig('{FigureName}', format='pdf')");


		    if (Show)
			{
			    script.AppendLine($"show(block={Block})");
			}

			Utils.RunPythonScript(script.ToString(), scriptName, Python);
		}

		/// <summary>
		/// Configures the axis.
		/// </summary>
		/// <param name="script">Script.</param>
		/// <param name="isSubPlot">If set to <c>true</c> is sub plot.</param>
		public void ConfigureAxis(StringBuilder script, bool isSubPlot)
		{
			if (!isSubPlot)
			{
				if (Subplots == null)
				{
				    script.AppendLine($"ax.set_xlabel('{XLabel}', fontsize=16)");
				    script.AppendLine($"ax.set_ylabel('{YLabel}', fontsize=16)");
				}
				else
				{
					// Set common labels
				    script.AppendLine($"fig.text(0.5, 0.04, '{XLabel}', fontsize=16, ha='center', va='center')");
				    script.AppendLine(
				        $"fig.text(0.04, 0.5, '{YLabel}', fontsize=16, ha='center', va='center', rotation='vertical')");
				}
                
				if (Series.Any(ia => !string.IsNullOrEmpty(ia.Label)))
				{
				    script.Append($"lgd = ax.legend(fontsize=14, loc={(int) LegendPosition})");
				}
			}
			else
			{
				if (Series.Count() == 1)
				{
					var label = Series.First().Label;
					if (!string.IsNullOrEmpty(label))
					{
					    script.AppendLine($"ax.set_title('{label}')\n");
					}
				}
			}
            
			script.AppendLine("tick_params(axis='both', which='major')");
			script.AppendLine("tick_params(axis='both', which='minor')");
            
			if (XLim != null)
			{
			    script.AppendLine($"ax.set_xlim([{XLim.Item1},{XLim.Item2}])");
			}

			if (YLim != null)
			{
			    script.AppendLine($"ax.set_ylim([{YLim.Item1},{YLim.Item2}])");
			}              
		}

		// Some helper static functions
		/// <summary>
		/// Plot the specified x, title, xlabel, ylabel and plotType.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="title">Title.</param>
		/// <param name="xlabel">Xlabel.</param>
		/// <param name="ylabel">Ylabel.</param>
		/// <param name="plotType">Plot type.</param>
		/// <param name="python">Path of python executable.</param>
		public static void Plot(IEnumerable<double> x,
		                        string title = "",
		                        string xlabel = "",
		                        string ylabel = "",
		                        PlotType plotType = PlotType.Line,
		                        string python = "/usr/bin/python")
		{
			Plot(x, null, title, xlabel, ylabel, plotType, python);
		}

		/// <summary>
		/// Plot the specified x, y, title, xlabel, ylabel and plotType.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="title">Title.</param>
		/// <param name="xlabel">Xlabel.</param>
		/// <param name="ylabel">Ylabel.</param>
		/// <param name="plotType">Plot type.</param>
		/// <param name="python">Path of python executable.</param>
		public static void Plot(IEnumerable<double> x,
		                        IEnumerable<double> y = null,
		                        string title = "",
		                        string xlabel = "",
		                        string ylabel = "",
		                        PlotType plotType = PlotType.Line,
		                        string python = "/usr/bin/python")
		{
			ISeries[] series;

			switch (plotType)
			{
				case PlotType.Line:
					series = new ISeries[] { new LineSeries { X = x, Y = y } };
					break;
				case PlotType.ErrorLine:
					series = new ISeries[] { new ErrorLineSeries { X = x, Y = y } };
					break;
				case PlotType.Bar:
					series = new ISeries[] { new BarSeries<double> { DependentValues = x, IndependentValues = y } };
					break;
				default:
					series = new ISeries[] { new LineSeries { X = x, Y = y } };
					break;
			}

		    var plotter = new Plotter { Title = title, XLabel = xlabel, YLabel = ylabel, Series = series, Python = python };
			plotter.Plot();
		}

		/// <summary>
		/// Plot lines with shaded error regions 
		/// </summary>
		/// <param name="x">X values.</param>
		/// <param name="errorValues">Error values.</param>
		/// <param name="y">Y values.</param>
		/// <param name="title">Plot title.</param>
		/// <param name="xLabel">x-axis label.</param>
		/// <param name="yLabel">y-axis label.</param>
		/// <param name="python">Path of python executable.</param>
		public static void ErrorPlot(IEnumerable<double> x,
		                             IEnumerable<double> errorValues,
		                             IEnumerable<double> y = null,
		                             string title = "",
		                             string xLabel = "",
		                             string yLabel = "",
		                             string python = "/usr/bin/python")
		{
			var series = new ISeries[] { new ErrorLineSeries { X = x, Y = y, ErrorValues = errorValues } };
			var plotter = new Plotter { Title = title, XLabel = xLabel, YLabel = yLabel, Series = series, Python = python };
			plotter.Plot();
		}

		/// <summary>
		/// Plot the specified series, title, xlabel, ylabel and plotType.
		/// </summary>
		/// <param name="series">Series.</param>
		/// <param name="title">Title.</param>
		/// <param name="xlabel">Xlabel.</param>
		/// <param name="ylabel">Ylabel.</param>
		/// <param name="plotType">Plot type.</param>
		/// <param name="python">Path of python executable.</param>
		public static void Plot(Dictionary<string, IEnumerable<double>> series,
		                        string title = "",
		                        string xlabel = "",
		                        string ylabel = "",
		                        PlotType plotType = PlotType.Line,
		                        string python = "/usr/bin/python")
		{
			ISeries[] plotSeries;

			switch (plotType)
			{
				case PlotType.Line:
					plotSeries = series.Select(ia => (ISeries)(new LineSeries { Label = ia.Key, X = ia.Value })).ToArray();
					break;
				case PlotType.ErrorLine:
					plotSeries = series.Select(ia => (ISeries)(new ErrorLineSeries { Label = ia.Key, X = ia.Value })).ToArray();
					break;
				case PlotType.Bar:
					plotSeries = series.Select(ia => (ISeries)(new BarSeries<double> { Label = ia.Key, IndependentValues = ia.Value })).ToArray();
					break;
				default:
					plotSeries = series.Select(ia => (ISeries)(new LineSeries { Label = ia.Key, X = ia.Value })).ToArray();
					break;
			}

			var plotter = new Plotter { Title = title, XLabel = xlabel, YLabel = ylabel, Series = plotSeries, Python = python };
			plotter.Plot();
		}

	    public static void Hinton(double[][] matrix, string title = "", string xlabel = "", string ylabel = "", string python = "/usr/lib/python")
	    {
	        var plotter = new Plotter { Title = title, XLabel = xlabel, YLabel = ylabel, Python = python };
            // from mpltools import special

	    }

        /// <summary>
        /// Demo plot.
        /// </summary>
        public static void Demo(string python = "/usr/bin/python")
        {
            var x = Enumerable.Range(0, 200).Select(ia => (double)ia / 100.0).ToArray();
            var y = x.Select(ia => Math.Sin(2.0 * ia * Math.PI));
            Plotter.Plot(x, y, "Test figure", "$x$", @"$\sin(2 \pi x)$", PlotType.Line, python);
        }
	}

	/// <summary>
	/// Subplots
	/// </summary>
	public class Subplots
	{
		/// <summary>
		/// Gets or sets the number of rows
		/// </summary>
		public int Rows { get; set; }

		/// <summary>
		/// Gets or sets the number of columns 
		/// </summary>
		public int Columns { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether plots share the x axis
		/// </summary>
		public bool ShareX { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether plots share the y axis
		/// </summary>
		public bool ShareY { get; set; }
	}
}

