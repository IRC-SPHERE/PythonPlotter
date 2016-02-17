//
// Utils.cs
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
// AAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace PythonPlotter
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// The utils.
    /// </summary>
    public static class Utils
    {
		/// <summary>
		/// Runs the python script. Currently this is *IX only
		/// </summary>
		/// <param name="script">Script.</param>
		/// <param name="filename">Script filename.</param>
		/// <param name="python">Python executable.</param>
		public static void RunPythonScript(string script, string filename = "script.py", string python = "/usr/local/bin/python")
		{
      		filename = filename.Replace( ":", "-" ); 

			using (var streamWriter = new StreamWriter(new FileStream(filename, FileMode.Create)))
			{
				// Make this executable
				streamWriter.WriteLine("#!" + python);
                streamWriter.WriteLine("# -*- coding: utf-8 -*-");
                streamWriter.WriteLine("from __future__ import unicode_literals");
				streamWriter.Write(script);
			}
		
			File.SetAttributes (filename, (FileAttributes)((uint)File.GetAttributes(filename) | 0x80000000)); 

			var processInfo = new ProcessStartInfo
				{
					FileName = filename,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					WorkingDirectory = Directory.GetCurrentDirectory()
				};

			try 
			{
				var process = Process.Start( processInfo );
				process.WaitForExit();
				Console.Write( process.StandardOutput.ReadToEnd() );
				process.Close();
			} 
			catch ( Exception ex ) 
			{
				Console.WriteLine("Error in spawning plotting process: " + ex.Message); 
			}
		}
  }
}