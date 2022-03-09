using System;

namespace PythonPlotterTestsNet5
{
    using PythonPlotter;

    public class Program
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        private static void Main(string[] args)
        {
            var python = "/usr/bin/python";
            if (args is { Length: > 0 })
            {
                python = args[0];
            }
            Console.WriteLine($"Using Python executable {python}");
			
            Plotter.Demo(python);
            Tests.Hinton(python);
            Tests.MatShow(python);
            Tests.Subplots(python);
        }
    }
}