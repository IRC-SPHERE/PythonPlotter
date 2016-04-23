//
// Series.cs
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
    using System.Linq;
    using System.Text;

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
        /// Gets or sets the row index (for subplots)
        /// </summary>
        int Row { get; set; }

        /// <summary>
        /// Gets or sets the column index (for subplots)
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        string Color { get; set; }

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
        /// <param name="ax">The axis to plot to.</param>
		/// <param name="script">Script.</param>
		void Plot(string ax, StringBuilder script);
	}

    /// <summary>
    /// Base class for series.
    /// </summary>
    public abstract class BaseSeries : ISeries
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the row index (for subplots)
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column index (for subplots)
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Plot to the specified script.
        /// </summary>
        /// <param name="ax">The axis to plot to.</param>
        /// <param name="script">Script.</param>
        public virtual void Plot(string ax, StringBuilder script)
        {
            throw new NotImplementedException();
        }
    }

	/// <summary>
	/// Line series.
	/// </summary>
	public class LineSeries : BaseSeries
	{
		/// <summary>
		/// Gets or sets the x values.
		/// </summary>
		public IEnumerable<double> X { get; set; }

		/// <summary>
		/// Gets or sets the y values.
		/// </summary>
		public IEnumerable<double> Y { get; set; }

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
        /// <param name="ax">The axis to plot to.</param>
        /// <param name="script">Script.</param>
        public override void Plot(string ax, StringBuilder script)
		{
            var label = string.IsNullOrEmpty(Label) ? "" : $", label='{Label}'";
            var color = string.IsNullOrEmpty(Color) ? "" : $", color={Color}";

            if (Y == null)
			{
                script.AppendLine($"lines.extend({ax}.plot([{string.Join(", ", X)}]{label}{color}))");
			}
			else
			{
			    if (X == null)
				{
					throw new InvalidOperationException("X and Y should not both be null");
				}

                script.AppendLine($"lines.extend({ax}.plot([{string.Join(", ", X)}], [{string.Join(", ", Y)}]{label}{color}))");
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
        /// <param name="ax">The axis to plot to.</param>
        /// <param name="script"></param>
        public override void Plot(string ax, StringBuilder script)
		{
            var label = string.IsNullOrEmpty(Label) ? "" : $", label='{Label}'";
            var color = string.IsNullOrEmpty(Color) ? "" : $", color={Color}";

            if (Y == null)
			{
                script.AppendLine($"lines.extend({ax}.scatter([{string.Join(", ", X)}]{label}{color}))");
			}
			else
			{
			    if (X == null)
				{
					throw new InvalidOperationException("X and Y should not both be null");
				}

                script.AppendLine($"lines.extend({ax}.scatter([{string.Join(", ", X)}], [{string.Join(", ", Y)}]{label}{color}))");
			}
		}
	}

	/// <summary>
	/// Bar series
	/// </summary>
	public class BarSeries<T> : BaseSeries
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BarSeries"/> is horizontal.
		/// </summary>
		/// <value><c>true</c> if horizontal; otherwise, <c>false</c>.</value>
		public bool Horizontal { get; set; }

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
		/// Plot to the specified script.
		/// </summary>
        /// <param name="ax">The axis to plot to.</param>
        /// <param name="script">Script.</param>
        public override void Plot(string ax, StringBuilder script)
		{
			if (DependentValues == null)
			{
				throw new InvalidOperationException("DependentValues should not be null");
			}

			var command = Horizontal ? "barh" : "bar";
			var errorString = Horizontal ? "xerr" : "yerr";
			var errorValues = ErrorValues == null
                ? string.Empty
                : $", {errorString}=[{string.Join(", ", ErrorValues)}]";

			var independent = string.Join(", ", (typeof(T) == typeof(double))
                ? IndependentValues.Select(ia => ia.ToString())
                : DependentValues.Select((ia, i) => i.ToString("D")));

		    var dependent = string.Join(", ", DependentValues);
			var width = Math.Abs(Width) < double.Epsilon ? 1.0 : Width;
			var color = string.IsNullOrEmpty(Color) ? "'b'" : Color;
		    var label = string.IsNullOrEmpty(Label) ? "" : $", label='{Label}'";

		    script.AppendLine(
		        $"lines.extend({ax}.{command}([{independent}], [{dependent}], {width}, color={color}{errorValues}{label}))");

		    if (IndependentValues != null && typeof(T) != typeof(double))
			{
				// script.AppendLine("ax = gca()");
			    script.AppendLine($"lines.extend({ax}.set_xticklabels(['{string.Join("', '", IndependentValues)}']))");
			}
		}
	}

	/// <summary>
	/// Error line series.
	/// </summary>
	public class ErrorLineSeries : LineSeries
	{
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
	    public double AlphaFill { get; set; } = 0.1;

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
        /// <param name="ax">The axis to plot to.</param>
        /// <param name="script">Script.</param>
        public override void Plot(string ax, StringBuilder script)
		{
            var label = string.IsNullOrEmpty(Label) ? "" : $", label='{Label}'";
            var errorLabel = string.IsNullOrEmpty(Label) ? "" : $", label='{ErrorLabel}'";
            var color = string.IsNullOrEmpty(Color) ? "" : $", color=c";

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

		    script.AppendLine($"x = array([{string.Join(", ", x)}])");
		    script.AppendLine($"y = array([{string.Join(", ", y)}])");
		    script.AppendLine($"e = array([{string.Join(", ", ErrorValues)}])");
            script.AppendLine($"c = next(palette)");

            script.AppendLine($"lines.extend({ax}.plot(x, y{label}{color}))");
            script.AppendLine($"{ax}.fill_between(x, y-e, y+e, alpha={AlphaFill}{errorLabel}{color})");
		}
	}

	/// <summary>
	/// For use with matshow
	/// </summary>
	public class MatrixSeries : BaseSeries
	{

		/// <summary>
		/// Gets or sets the values.
		/// </summary>
		public double[][] Values { get; set; }

		/// <summary>
		/// Gets or sets the color map.
		/// </summary>
		public string ColorMap { get; set; } = "gray";

		/// <summary>
		/// Gets or sets the values as a string.
		/// </summary>
		protected string ValuesAsString
		{
			get { return "[[" + string.Join("], [", Values.Select(ia => string.Join(", ", ia))) + "]]"; }
		}

		/// <summary>
		/// Plot to the specified script.
		/// </summary>
        /// <param name="ax">The axis to plot to.</param>
        /// <param name="script">Script.</param>
        public override void Plot(string ax, StringBuilder script)
		{
			script.AppendLine($"x = array({ValuesAsString})");
			script.AppendLine($"{ax}.matshow(x, cmap='{ColorMap}')");
		}
	}

    /// <summary>
   	/// Hinton diagram. See http://tonysyu.github.io/mpltools/auto_examples/special/plot_hinton.html
    /// </summary>
    public class HintonSeries : MatrixSeries
    {
        /// <summary>
        /// Plot to the specified script.
        /// </summary>
        /// <param name="ax">The axis to plot to.</param>
        /// <param name="script">Script.</param>
        public override void Plot(string ax, StringBuilder script)
        {
			// Note that the color map is ignored
            script.AppendLine("from mpltools import special");
            script.AppendLine($"x = array({ValuesAsString})");
			script.AppendLine($"special.hinton(x)");
        }
    }
}