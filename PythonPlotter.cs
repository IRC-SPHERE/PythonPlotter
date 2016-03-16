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
				string figureName = string.IsNullOrEmpty(ScriptName) 
                        ? "plot.pdf" 
                        : (ScriptName.EndsWith(".py", StringComparison.Ordinal) ? ScriptName.Substring(
					                    0,
					                    ScriptName.Length - 3) + ".pdf" : ScriptName); 
                
				figureName = figureName.EndsWith(".pdf", StringComparison.Ordinal) ? figureName : figureName + ".pdf";
				figureName = figureName.Replace(":", "-");
				return figureName;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SphereEngine.PythonPlotter"/> is shown.
		/// </summary>
		/// <value><c>true</c> if show; otherwise, <c>false</c>.</value>
		public bool Show { get; set; } = true;

		/// <summary>
		/// Gets or sets the subplots.
		/// </summary>
		/// <value>The subplots.</value>

		public Subplots Subplots { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SphereEngine.PythonPlotter"/> is blocking.
		/// </summary>
		/// <value><c>true</c> if block; otherwise, <c>false</c>.</value>
		public bool Block { get; set; } = true;

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
            
			// script.AppendLine("from mpltools import style, special");
			// script.AppendLine("style.use('ggplot')");
			script.AppendLine("import seaborn as sns");
			script.AppendLine("sns.set(style='darkgrid')");
			script.AppendLine("sns.set_context('paper')");
            
			// script.AppendLine("from matplotlib import rc");
			// script.AppendLine("rc('font',**{'family':'sans-serif','sans-serif':['Helvetica']})");
			// script.AppendLine("rc('text', usetex=True)");

			string scriptName = string.IsNullOrEmpty(ScriptName) ? "script.py" : (ScriptName.EndsWith(
				                    ".py",
				                    StringComparison.Ordinal) ? ScriptName : ScriptName + ".py");
			scriptName.Replace(":", "-").Replace("/", "-");
            
			if (Subplots != null)
			{
				script.AppendFormat(
					"fig, axs = subplots({0}, {1}, sharex={2}, sharey={3})\n",
					Subplots.Rows,
					Subplots.Columns,
					Subplots.ShareX,
					Subplots.ShareY);
			}
			else
			{
				script.AppendLine("fig, ax = subplots()");
			}

			// script.AppendFormat("pp = PdfPages('{0}.pdf')\n", figureName);

			// var legend = new List<string>();
			foreach (var line in Series)
			{
				// legend.Add("'" + line.Label + "'");
				if (Subplots != null)
				{
					if (Subplots.Rows > 1 && Subplots.Columns > 1)
					{
						script.AppendFormat("ax = axs[{0}, {1}]\n", line.Row, line.Column);
					}
					else if (Subplots.Rows == 1 && Subplots.Columns > 1)
					{
						script.AppendFormat("ax = axs[{0}]\n", line.Column);
					}
					else if (Subplots.Columns == 1 && Subplots.Rows > 1)
					{
						script.AppendFormat("ax = axs[{0}]\n", line.Row);
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

			script.AppendFormat("{0}title('{1}', fontsize=16)\n", Subplots == null ? "" : "fig.sup", Title);
			ConfigureAxis(script, false);
            
			// script.AppendFormat("ax.legend([{0}], loc={1})\n", string.Join(", ", legend), (int)LegendPosition);

			// script.AppendLine("pp.savefig()");
			// script.AppendLine("pp.close()");

			// script.AppendFormat("import os\nprint(os.path.abspath('{0}'))\n", figureName);

			if (Series.Any(ia => !string.IsNullOrEmpty(ia.Label)))
			{
				script.AppendFormat(
					"fig.savefig('{0}', format='pdf', bbox_extra_artists=(lgd,), bbox_inches='tight')\n",
					FigureName);
			}
			else
			{
				script.AppendFormat("fig.savefig('{0}', format='pdf')\n", FigureName);
			}
                 

			if (Show)
			{
				script.AppendFormat("show(block={0})\n", Block);
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
					script.AppendFormat("ax.set_xlabel('{0}', fontsize=16)\n", XLabel);
					script.AppendFormat("ax.set_ylabel('{0}', fontsize=16)\n", YLabel);
				}
				else
				{
					// Set common labels
					script.AppendFormat("fig.text(0.5, 0.04, '{0}', fontsize=16, ha='center', va='center')\n", XLabel);
					script.AppendFormat(
						"fig.text(0.04, 0.5, '{0}', fontsize=16, ha='center', va='center', rotation='vertical')\n",
						YLabel);
				}
                
				if (Series.Any(ia => !string.IsNullOrEmpty(ia.Label)))
				{
					script.AppendFormat("lgd = ax.legend(fontsize=14, loc={0})\n", (int)LegendPosition);
				}
			}
			else
			{
				if (Series.Count() == 1)
				{
					var label = Series.First().Label;
					if (!string.IsNullOrEmpty(label))
					{
						script.AppendFormat("ax.set_title('{0}')\n", label);
					}
				}
			}
            
			script.AppendLine("tick_params(axis='both', which='major', labelsize=12)");
			script.AppendLine("tick_params(axis='both', which='minor', labelsize=12)");
            
			if (XLim != null)
			{
				script.AppendFormat("ax.set_xlim([{0},{1}])\n", XLim.Item1, XLim.Item2);
			}

			if (YLim != null)
			{
				script.AppendFormat("ax.set_ylim([{0},{1}])\n", YLim.Item1, YLim.Item2);
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
					series = new[] { new LineSeries { X = x, Y = y } };
					break;
				case PlotType.ErrorLine:
					series = new[] { new ErrorLineSeries { X = x, Y = y } };
					break;
				case PlotType.Bar:
					series = new[] { new BarSeries<double> { DependentValues = x, IndependentValues = y } };
					break;
				default:
					series = new[] { new LineSeries { X = x, Y = y } };
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
			var series = new[] { new ErrorLineSeries { X = x, Y = y, ErrorValues = errorValues } };
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
					plotSeries = series.Select(ia => new LineSeries { Label = ia.Key, X = ia.Value }).ToArray();
					break;
				case PlotType.ErrorLine:
					plotSeries = series.Select(ia => new ErrorLineSeries { Label = ia.Key, X = ia.Value }).ToArray();
					break;
				case PlotType.Bar:
					plotSeries = series.Select(ia => new BarSeries<double> { Label = ia.Key, IndependentValues = ia.Value }).ToArray();
					break;
				default:
					plotSeries = series.Select(ia => new LineSeries { Label = ia.Key, X = ia.Value }).ToArray();
					break;
			}

			var plotter = new Plotter { Title = title, XLabel = xlabel, YLabel = ylabel, Series = plotSeries, Python = python };
			plotter.Plot();
		}

	}

	/// <summary>
	/// The series interface.
	/// </summary>
	public interface ISeries
	{
		/// <summary>
		/// Gets or sets the label.
		/// </summary>
		/// <value>The label.</value>
		string Label { get; set; }

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
		/// <param name="script">Script.</param>
		void Plot(StringBuilder script);

		/// <summary>
		/// Gets or sets the row index (for subplots)
		/// </summary>
		int Row { get; set; }

		/// <summary>
		/// Gets or sets the column index (for subplots)
		/// </summary>
		int Column { get; set; }
	}

	/// <summary>
	/// Line series.
	/// </summary>
	public class LineSeries : ISeries
	{
		/// <summary>
		/// Gets or sets the label.
		/// </summary>
		/// <value>The label.</value>
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets the x values.
		/// </summary>
		public IEnumerable<double> X { get; set; }

		/// <summary>
		/// Gets or sets the y values.
		/// </summary>
		public IEnumerable<double> Y { get; set; }

		/// <summary>
		/// Gets or sets the row index (for subplots)
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Gets or sets the column index (for subplots)
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
		/// <param name="script">Script.</param>
		public virtual void Plot(StringBuilder script)
		{
			if (Y == null)
			{
				if (string.IsNullOrEmpty(Label))
				{
					script.AppendFormat("ax.plot([{0}])\n", string.Join(", ", X));
				}
				else
				{
					script.AppendFormat("ax.plot([{0}], label='{1}')\n", string.Join(", ", X), Label);
				}
			}
			else
			{
				if (X == null)
				{
					throw new InvalidOperationException("X and Y should not both be null");
				}

				if (string.IsNullOrEmpty(Label))
				{
					script.AppendFormat("ax.plot([{0}], [{1}])\n", string.Join(", ", X), string.Join(", ", Y));
				}
				else
				{
					script.AppendFormat("ax.plot([{0}], [{1}], label='{2}')\n", string.Join(", ", X), string.Join(", ", Y), Label);
				}
			}
		}
	}

	/// <summary>
	/// Scatter Series
	/// </summary>
	public class ScatterSeries : LineSeries
	{
		/// <summary>
		/// Plot to the specified script
		/// </summary>
		/// <param name="script"></param>
		public override void Plot(StringBuilder script)
		{
			if (Y == null)
			{
				if (string.IsNullOrEmpty(Label))
				{
					script.AppendFormat("ax.scatter([{0}])\n", string.Join(", ", X));
				}
				else
				{
					script.AppendFormat("ax.scatter([{0}], label='{1}')\n", string.Join(", ", X), Label);
				}
			}
			else
			{
				if (X == null)
				{
					throw new InvalidOperationException("X and Y should not both be null");
				}

				if (string.IsNullOrEmpty(Label))
				{
					script.AppendFormat("ax.scatter([{0}], [{1}])\n", string.Join(", ", X), string.Join(", ", Y));
				}
				else
				{
					script.AppendFormat("ax.scatter([{0}], [{1}], label='{2}')\n", string.Join(", ", X), string.Join(", ", Y), Label);
				}
			}
		}
	}

	/// <summary>
	/// Bar series
	/// </summary>
	public class BarSeries<T> : ISeries
	{
		/// <summary>
		/// Gets or sets the label.
		/// </summary>
		/// <value>The label.</value>
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SphereEngine.BarSeries`1"/> is horizontal.
		/// </summary>
		/// <value><c>true</c> if horizontal; otherwise, <c>false</c>.</value>
		public bool Horizontal { get; set; }

		/// <summary>
		/// Gets or sets the row index (for subplots)
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Gets or sets the column index (for subplots)
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// Gets or sets the dependent values.
		/// </summary>
		/// <value>The dependent values.</value>
		public IEnumerable<double> DependentValues { get; set; }

		/// <summary>
		/// Gets or sets the independent values.
		/// </summary>
		/// <value>The independent values.</value>
		public IEnumerable<T> IndependentValues { get; set; }

		/// <summary>
		/// Gets or sets the error values.
		/// </summary>
		/// <value>The error values.</value>
		public IEnumerable<double> ErrorValues { get; set; }

		/// <summary>
		/// Gets or sets the X tick labels.
		/// </summary>
		/// <value>The X tick labels.</value>
		public IEnumerable<string> XTickLabels { get; set; }

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		public double Width { get; set; }

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public string Color { get; set; }

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
		/// <param name="script">Script.</param>
		public virtual void Plot(StringBuilder script)
		{
			if (DependentValues == null)
			{
				throw new InvalidOperationException("DependentValues should not be null");
			}

			string command = Horizontal ? "barh" : "bar";
			string errorString = Horizontal ? "xerr" : "yerr";
			string errorValues = ErrorValues == null 
                ? string.Empty 
                : string.Format(", {0}=[{1}]", errorString, string.Join(", ", ErrorValues));

			var independent = string.Join(", ", (typeof(T) == typeof(double)) 
                ? IndependentValues.Select(ia => ia.ToString()) 
                : DependentValues.Select((ia, i) => i.ToString("D")));

			if (string.IsNullOrEmpty(Label))
			{
				script.AppendFormat("ax.{0}([{1}], [{2}], {3}, color='{4}'{5})\n", command, independent, 
					string.Join(", ", DependentValues), Width == 0.0 ? 1.0 : Width, 
					string.IsNullOrEmpty(Color) ? "b" : Color, 
					errorValues);
			}
			else
			{
				script.AppendFormat("ax.{0}([{1}], [{2}], {3}, color='{4}'{5}, label='{6}')\n", command, independent, 
					string.Join(", ", DependentValues), Width == 0.0 ? 1.0 : Width, 
					string.IsNullOrEmpty(Color) ? "b" : Color, 
					errorValues, 
					Label);
			}
            
			if (IndependentValues != null && typeof(T) != typeof(double))
			{
				// script.AppendLine("ax = gca()");
				script.AppendFormat("ax.set_xticklabels(['{0}'])\n", string.Join("', '", IndependentValues));
			}
		}
	}

	/// <summary>
	/// Error line series.
	/// </summary>
	public class ErrorLineSeries : LineSeries
	{
		/// <summary>
		/// The alpha fill.
		/// </summary>
		private double alphaFill = 0.1;

		/// <summary>
		/// The label for the error values. 
		/// </summary>
		public string ErrorLabel { get; set; }

		/// <summary>
		/// Gets or sets the error values.
		/// </summary>
		/// <value>The error values.</value>
		public IEnumerable<double> ErrorValues { get; set; }

		/// <summary>
		/// Gets or sets the alpha fill.
		/// </summary>
		/// <value>The alpha fill.</value>
		public double AlphaFill
		{
			get
			{
				return alphaFill;
			}
			set
			{
				alphaFill = value;
			}
		}

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
		/// <param name="script">Script.</param>
		public override void Plot(StringBuilder script)
		{
			var x = X;
			var y = Y;
			if (Y == null)
			{
				if (X == null)
				{
					throw new InvalidOperationException("X and Y should not both be null");
				}

				x = Enumerable.Range(0, X.Count()).Select(ia => (double)ia);
				y = X;
			}

			//script.AppendFormat("special.errorfill(array([{0}]), array([{1}]), array([{2}]), alpha_fill={3})\n", 
			//	string.Join(", ", x), string.Join(", ", y), string.Join(", ", ErrorValues), AlphaFill);
			script.AppendFormat("x = array([{0}])\n", string.Join(", ", x));
			script.AppendFormat("y = array([{0}])\n", string.Join(", ", y));
			script.AppendFormat("e = array([{0}])\n", string.Join(", ", ErrorValues));
			if (string.IsNullOrEmpty(Label))
			{
				script.AppendLine("ax.plot(x, y)");
			}
			else
			{
				script.AppendFormat("ax.plot(x, y, label='{0}')\n", Label);
			}
            
			if (string.IsNullOrEmpty(ErrorLabel))
			{
				script.AppendFormat("ax.fill_between(x, y-e, y+e, alpha={0})\n", AlphaFill);
			}
			else
			{
				script.AppendFormat("ax.fill_between(x, y-e, y+e, alpha={0}, label='{1}')\n", AlphaFill, ErrorLabel);
			}
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

