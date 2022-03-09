using System;
using PythonPlotter;

namespace NugetTestNet5
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var python = "/usr/bin/python";
            if (args is { Length: > 0 })
            {
                python = args[0];
            }
            Console.WriteLine($"Using Python executable {python}");
			
            Tests.Hinton(python);
            Tests.MatShow(python);
            Tests.Subplots(python);
        }
    }
}