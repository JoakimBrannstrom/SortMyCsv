using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace SortMyCsv
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputPath = GetParameter("/in:", args);
			var outputPath = GetParameter("/out:", args);

			if(string.IsNullOrEmpty(inputPath) || string.IsNullOrEmpty(outputPath))
			{
				Console.WriteLine("Usage:");
				var cmd = Environment.CommandLine;
				var dirLength = cmd.LastIndexOf('\\') + 1;
				var process = cmd.Substring(dirLength, cmd.Length - dirLength - 1);
				Console.WriteLine(process + " /in:<path to csv input> /out:<path to csv output>");
			}
			else if (!File.Exists(inputPath))
				Console.WriteLine("Could not find file: " + inputPath);
			else if (File.Exists(outputPath))
				Console.WriteLine("File already exists: " + outputPath);

			Console.WriteLine("Csv has been sorted! Press <enter> to close the application.");
			Console.ReadLine();
		}

		private static string GetParameter(string prefix, string[] args)
		{
			var parameter = args.Where(a => a.StartsWith(prefix)).FirstOrDefault();
			if(parameter == null)
				return null;

			return parameter.Substring(prefix.Length);
		}
	}
}
