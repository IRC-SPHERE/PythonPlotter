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
		Bar,
		ErrorBar,
	    Hinton
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
        /// Gets or sets the twin x.
        /// </summary>
        /// <value>The twin x.</value>
        public bool TwinX { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Plotter"/> should show a grid.
		/// </summary>
		/// <value><c>true</c> if grid; otherwise, <c>false</c>.</value>
		public bool Grid { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Plotter"/> is dark.
		/// </summary>
		/// <value><c>true</c> if dark; otherwise, <c>false</c>.</value>
		public bool Dark { get; set; } = true;

	    /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Plotter"/> should use tight fitting.
        /// </summary>
        /// <value><c>true</c> if dark; otherwise, <c>false</c>.</value>
        public bool Tight { get; set; }

		/// <summary>
		/// Gets the seaborn style.
		/// </summary>
		/// <value>The seaborn style.</value>
		public string SeabornStyle => (Dark ? "dark" : "white") + (Grid ? "grid" : string.Empty);

	    /// <summary>
		/// Gets or sets the name of the script.
		/// </summary>
		/// <value>The name of the script.</value>
		public string ScriptName { get; set; } = "/tmp/script.py";

        /// <summary>
        /// Gets the name of the valid script.
        /// </summary>
        /// <value>The name of the valid script.</value>
        public string ValidScriptName
        {
            get
            {
                string scriptName = string.IsNullOrEmpty(ScriptName)
                    ? "script.py"
                    : (ScriptName.EndsWith(".py", StringComparison.Ordinal) ? ScriptName : ScriptName + ".py");
                scriptName = scriptName.Replace(":", "-");
                return scriptName;
            }
        }

	    /// <summary>
        /// Gets or sets the name of the pdf figure.
        /// </summary>
        /// <value>The name of the pdf figure.</value>
        public string FigureName { get; set; } = "/tmp/script.pdf";

	    /// <summary>
	    /// Gets or sets the figure size (in inches)
	    /// </summary>
	    public Tuple<double, double> FigureSize { get; set; }

		/// <summary>
		/// Gets or sets the valid name of the pdf figure.
		/// </summary>
		/// <value>The valid name of the pdf figure.</value>
		public string ValidFigureName
		{ 
			get
			{
			    string figureName = string.IsNullOrEmpty(FigureName)
			        ? "script.pdf"
			        : (FigureName.EndsWith(".pdf", StringComparison.Ordinal) ? FigureName : FigureName + ".pdf");
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
        /// Gets or sets the script.
        /// </summary>
        /// <value>The script.</value>
        public StringBuilder Script { get; set; } = new StringBuilder();

		/// <summary>
		/// Do the plotting.
		/// </summary>
        public void Plot(StringBuilder additional = null)
		{
			Script = new StringBuilder();
			
            BuildPreamble();
            BuildScript();

            if (additional != null)
            {
                Script.Append(additional);
            }

            BuildPostamble();

			Utils.RunPythonScript(Script.ToString(), ValidScriptName, Python);
		}

        /// <summary>
        /// Builds the preamble.
        /// </summary>
        /// <returns>The preamble.</returns>
        public void BuildPreamble()
        {
            Script.AppendLine("import warnings");
            #if FALSE
                                                {
                // Disable warnings for pylab only
                script.AppendLine("with warnings.catch_warnings():");
                script.AppendLine("    warnings.simplefilter('ignore')");
                script.AppendLine("    from pylab import *");
            }
            #else
            {
                // Globally disable warnings 
                Script.AppendLine("warnings.simplefilter('ignore')");
            }
            #endif

            Script.AppendLine("try:");
            Script.AppendLine("    from pylab import *");
            Script.AppendLine("except TypeError as e:");
            Script.AppendLine("    print(str(e))");
            Script.AppendLine("    print('Falling back to Agg backend')");
            Script.AppendLine("    import matplotlib");
            Script.AppendLine("    matplotlib.use('Agg')");
            Script.AppendLine("    from pylab import *");

            Script.AppendLine("import itertools");

            if (SeaBorn)
            {
                Script.AppendLine("import seaborn as sns");
                Script.AppendLine($"sns.set(style='{SeabornStyle}')");
                Script.AppendLine("sns.set_context('paper')");
                Script.AppendLine("palette = itertools.cycle(sns.color_palette())");
            }
			//else
            //{
            //    Script.AppendLine("palette = itertools.cycle(cm.viridis)");
            //}
			
			Script.AppendLine("lines = []");
        }

        /// <summary>
        /// Builds the script.
        /// </summary>
        /// <returns>The script.</returns>
        public void BuildScript()
        {
            string ax = "ax";

            if (Subplots == null)
            {
                if (TwinX)
                {
                    Script.AppendLine("ax2 = ax.twinx()");
                    ax = "ax2";
                }
                else
                {
                    Script.AppendLine("fig, ax = subplots()");
                }
            }
            else
            {
                if (TwinX)
                {
                    throw new InvalidOperationException("Simultaneous Subplots and TwinX are currently not supported");
                }

                Script.AppendLine(
                    $"fig, axs = subplots({Subplots.Rows}, {Subplots.Columns}, " +
                    $"sharex={Subplots.ShareX}, sharey={Subplots.ShareY})");
            }

            if (FigureSize != null)
            {
                Script.AppendLine($"fig.set_size_inches({FigureSize.Item1}, {FigureSize.Item2}, forward=True)");
            }

            if (Subplots != null && TwinX)
            {
                throw new InvalidOperationException("Simultaneous Subplots and TwinX are currently not supported");
            }

            // var legend = new List<string>();
            foreach (var line in Series)
            {
                // legend.Add("'" + line.Label + "'");
                if (Subplots != null)
                {
                    if (Subplots.Rows > 1 && Subplots.Columns > 1)
                    {
                        Script.AppendLine($"ax = axs[{line.Row}, {line.Column}]");
                    }
                    else if (Subplots.Rows == 1 && Subplots.Columns > 1)
                    {
                        Script.AppendLine($"ax = axs[{line.Column}]");
                    }
                    else if (Subplots.Columns == 1 && Subplots.Rows > 1)
                    {
                        Script.AppendLine($"ax = axs[{line.Row}]");
                    }
                    else if (Subplots.Rows == 1 && Subplots.Columns == 1)
                    {
                        // Just in case someone specificies subplots(1, 1) for some reason
                        Script.AppendLine($"ax = axs");
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid row/column specificiation for subplots");
                    }

                    ConfigureAxis(ax, true);
                }

                line.Plot(ax, Script);

                if (Series.Any(ia => !string.IsNullOrEmpty(ia.Label)))
                {
					if (!TwinX)
					{
                    	Script.AppendLine($"lgd = {ax}.legend(fontsize=14, loc={(int) LegendPosition})");
					}
                }
            }

            Script.AppendLine($"{(Subplots == null ? "" : "fig.sup")}title('{Title}', fontsize=16)");
            ConfigureAxis(ax, false);

			if (TwinX)
			{
				Script.AppendLine("labels = [l.get_label() for l in lines]");
				Script.AppendLine($"{ax}.legend(lines, labels, fontsize=14, loc={(int)LegendPosition})");
			}
        }

        /// <summary>
        /// Builds the postamble.
        /// </summary>
        /// <returns>The postamble.</returns>
        public void BuildPostamble()
        {
            //var ls = string.Join(", ", Legend);
            //script.AppendLine("ax.legend([{ls}], loc={(int)LegendPosition})");
            //script.AppendLine("ax.legend()");

            string tight = Tight ? ", bbox_inches='tight'" : string.Empty;

            Script.AppendLine(
                Series.Any(ia => !string.IsNullOrEmpty(ia.Label))
                ? $"fig.savefig('{ValidFigureName}', format='pdf', bbox_extra_artists=(lgd,){tight})"
                : $"fig.savefig('{ValidFigureName}', format='pdf')");


            if (Show)
            {
                Script.AppendLine($"show(block={Block})");
            }
        }

	    /// <summary>
	    /// Configures the axis.
	    /// </summary>
	    /// <param name="ax">The axis to plot to.</param>
	    /// <param name="isSubPlot">If set to <c>true</c> is sub plot.</param>
	    public void ConfigureAxis(string ax, bool isSubPlot)
		{
			if (!isSubPlot)
			{
				if (Subplots == null)
				{
                    Script.AppendLine($"{ax}.set_xlabel('{XLabel}', fontsize=16)");
                    Script.AppendLine($"{ax}.set_ylabel('{YLabel}', fontsize=16)");
				}
				else
				{
					// Set common labels
				    Script.AppendLine($"fig.text(0.5, 0.04, '{XLabel}', fontsize=16, ha='center', va='center')");
				    Script.AppendLine(
				        $"fig.text(0.04, 0.5, '{YLabel}', fontsize=16, ha='center', va='center', rotation='vertical')");
				}
			}
			else
			{
				if (Series.Count() == 1)
				{
					var label = Series.First().Label;
					if (!string.IsNullOrEmpty(label))
					{
                        Script.AppendLine($"{ax}.set_title('{label}')");
					}
				}
			}
            
			Script.AppendLine("tick_params(axis='both', which='major')");
			Script.AppendLine("tick_params(axis='both', which='minor')");
            
			if (XLim != null)
			{
                Script.AppendLine($"{ax}.set_xlim([{XLim.Item1},{XLim.Item2}])");
			}

			if (YLim != null)
			{
                Script.AppendLine($"{ax}.set_ylim([{YLim.Item1},{YLim.Item2}])");
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
			Plot(x, null, null, title, xlabel, ylabel, plotType, python);
		}

	    /// <summary>
	    /// Plot the specified x, y, title, xlabel, ylabel and plotType.
	    /// </summary>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="errorValues">The error values.</param>
	    /// <param name="title">Title.</param>
	    /// <param name="xlabel">Xlabel.</param>
	    /// <param name="ylabel">Ylabel.</param>
	    /// <param name="plotType">Plot type.</param>
	    /// <param name="python">Path of python executable.</param>
	    public static void Plot(IEnumerable<double> x,
		                        IEnumerable<double> y = null,
		                        IEnumerable<double> errorValues = null,
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
		            series = new ISeries[] {new LineSeries {X = x, Y = y}};
		            break;
		        case PlotType.ErrorLine:
		            series = new ISeries[] {new ErrorLineSeries {X = x, Y = y}};
		            break;
		        case PlotType.Bar:
		        case PlotType.ErrorBar:
		            series = new ISeries[]
		            {
		                new BarSeries<double> {DependentValues = x, IndependentValues = y, ErrorValues = errorValues}
		            };
		            break;
		        case PlotType.Hinton:
		            throw new ArgumentOutOfRangeException(nameof(plotType));
		        default:
					series = new ISeries[] { new LineSeries { X = x, Y = y } };
					break;
			}

		    var plotter = new Plotter
		    {
		        Title = title, XLabel = xlabel, YLabel = ylabel, Series = series, Python = python
		    };

			plotter.Plot();
		}

		/// <summary>
        /// Twin x-axis plots 
        /// </summary>
        /// <param name="x">The x variables.</param>
        /// <param name="y1">The first y variables.</param>
        /// <param name="y2">The second y variables.</param>
        /// <param name="title">The title.</param>
        /// <param name="xlabel">The x-axis label.</param>
        /// <param name="y1Label">The first y-axis label.</param>
        /// <param name="y2Label">The second y-axis label.</param>
        /// <param name="plotType1">The first plot type.</param>
        /// <param name="plotType2">The second plot type.</param>
        /// <param name="python">The python executable.</param>
        public static void TwinPlot(IEnumerable<double> x,
                                    IEnumerable<double> y1,
                                    IEnumerable<double> y2,
                                    string title = "",
                                    string xlabel = "",
                                    string y1Label = "",
                                    string y2Label = "",
                                    PlotType plotType1 = PlotType.Line,
                                    PlotType plotType2 = PlotType.Line,
                                    string python = "/usr/bin/python")
        {
            ISeries[] series1, series2;
		    var xx = x as double[] ?? x.ToArray();

		    switch (plotType1)
            {
                case PlotType.Line:
                    series1 = new ISeries[] { new LineSeries { X = xx, Y = y1 } };
                break;
                case PlotType.ErrorLine:
                    series1 = new ISeries[] { new ErrorLineSeries { X = xx, Y = y1 } };
                break;
                case PlotType.Bar:
                case PlotType.ErrorBar:
                    series1 = new ISeries[] { new BarSeries<double> { DependentValues = xx, IndependentValues = y1 } };
                break;
                default:
                    series1 = new ISeries[] { new LineSeries { X = xx, Y = y1 } };
                break;
            }

            switch (plotType2)
            {
                case PlotType.Line:
                    series2 = new ISeries[] { new LineSeries { X = xx, Y = y2 } };
                break;
                case PlotType.ErrorLine:
                    series2 = new ISeries[] { new ErrorLineSeries { X = xx, Y = y2 } };
                break;
                case PlotType.Bar:
                case PlotType.ErrorBar:
                    series2 = new ISeries[] { new BarSeries<double> { DependentValues = xx, IndependentValues = y2 } };
                break;
                default:
                    series2 = new ISeries[] { new LineSeries { X = xx, Y = y2 } };
                break;
            }
			
			// Turn on color cycling for both series
			series1[0].Color = "next(palette)";
			series2[0].Color = "next(palette)";

			// Here we build the plotting script for the second plot (without the pre/postamble), 
			// so we can append it to the script for the first plot
            var plotter2 = new Plotter { XLabel = xlabel, YLabel = y2Label, Series = series2, TwinX = true };
            plotter2.BuildScript();

            // TODO: http://matplotlib.org/examples/api/two_scales.html

            var plotter1 = new Plotter
            {
                Title = title, XLabel = xlabel, YLabel = y1Label, Series = series1, Python = python
            };

            plotter1.Plot(plotter2.Script);
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
			var plotter = new Plotter
			{
			    Title = title, XLabel = xLabel, YLabel = yLabel, Series = series, Python = python
			};

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
		            plotSeries = series.Select(ia => (ISeries) new LineSeries {Label = ia.Key, X = ia.Value}).ToArray();
		            break;
		        case PlotType.ErrorLine:
		            plotSeries = series.Select(ia => (ISeries) new ErrorLineSeries {Label = ia.Key, X = ia.Value}).ToArray();
		            break;
		        case PlotType.Bar:
		        case PlotType.ErrorBar:
		            plotSeries = series
		                .Select(ia => (ISeries) new BarSeries<double> {Label = ia.Key, IndependentValues = ia.Value})
		                .ToArray();
		            break;
		        default:
		            plotSeries = series.Select(ia => (ISeries) new LineSeries {Label = ia.Key, X = ia.Value}).ToArray();
					break;
			}

		    var plotter = new Plotter
		    {
		        Title = title, XLabel = xlabel, YLabel = ylabel, Series = plotSeries, Python = python
		    };

			plotter.Plot();
		}

	    /// <summary>
	    /// Plot the matrix using matshow.
	    /// </summary>
	    /// <param name="matrix">Matrix.</param>
	    /// <param name="title">Title.</param>
	    /// <param name="xlabel">Xlabel.</param>
	    /// <param name="ylabel">Ylabel.</param>
	    /// <param name="python">Path of python executable.</param>
	    public static void MatShow(double[][] matrix, string title = "", string xlabel = "", string ylabel = "",
	        string python = "/usr/lib/python")
	    {
	        var plotter = new Plotter
	        {
	            Title = title,
	            XLabel = xlabel,
	            YLabel = ylabel,
	            Python = python,
	            Series = new ISeries[] {new MatrixSeries {Values = matrix}},
	            Grid = false
	        };
	        plotter.Plot();
	    }

	    /// <summary>
	    /// Hinton diagram.
	    /// </summary>
	    /// <param name="matrix">Matrix.</param>
	    /// <param name="title">Title.</param>
	    /// <param name="xlabel">Xlabel.</param>
	    /// <param name="ylabel">Ylabel.</param>
	    /// <param name="python">Path of python executable.</param>
	    public static void Hinton(double[][] matrix, string title = "", string xlabel = "", string ylabel = "",
	        string python = "/usr/lib/python")
	    {
	        var plotter = new Plotter
	        {
	            Title = title,
	            XLabel = xlabel,
	            YLabel = ylabel,
	            Python = python,
	            Series = new ISeries[] {new HintonSeries {Values = matrix}},
	            Grid = false
	        };
	        plotter.Plot();
	    }

	    /// <summary>
        /// Demo plot.
        /// </summary>
        public static void Demo(string python = "/usr/bin/python")
        {
            var x = Enumerable.Range(0, 200).Select(ia => ia / 100.0).ToArray();
            var y = x.Select(ia => Math.Sin(2.0 * ia * Math.PI));
            Plot(x, y, null, "Test figure", "$x$", @"$\sin(2 \pi x)$", PlotType.Line, python);
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

