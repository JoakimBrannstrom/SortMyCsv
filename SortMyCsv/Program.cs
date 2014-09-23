using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;

namespace SortMyCsv
{
	class Program
	{
		static int sortingColumnIndex;
		static char columnSeparator;
		static bool overwrite;

		static void Main(string[] args)
		{
			ReadAppSettings();

			var inputPath = GetParameter("/in:", args);
			var outputPath = GetParameter("/out:", args);

			CheckArguments(inputPath, outputPath);
			CheckInputFile(inputPath);
			CheckOutputFile(outputPath);

			Sort(inputPath, outputPath);

			Console.WriteLine("Csv has been sorted! Press <enter> to close the application.");
			Console.ReadLine();
		}

		private static void ReadAppSettings()
		{
			sortingColumnIndex = int.Parse(ConfigurationManager.AppSettings["SortingColumnIndex"]);
			columnSeparator = char.Parse(ConfigurationManager.AppSettings["ColumnSeparator"]);
			overwrite = bool.Parse(ConfigurationManager.AppSettings["OverwriteOutput"]);
		}

		private static string GetParameter(string prefix, string[] args)
		{
			var parameter = args
							.Where(a => a.StartsWith(prefix))
							.FirstOrDefault();

			if(parameter == null)
				return null;

			return parameter.Substring(prefix.Length);
		}

		private static void CheckArguments(string inputPath, string outputPath)
		{
			if (string.IsNullOrEmpty(inputPath) || string.IsNullOrEmpty(outputPath))
			{
				Console.WriteLine("Usage:");
				var cmd = Environment.CommandLine;
				var dirLength = cmd.LastIndexOf('\\') + 1;
				var process = cmd.Substring(dirLength, cmd.Length - dirLength - 1);
				Console.WriteLine(process + " /in:<path to csv input> /out:<path to csv output>");
				Environment.Exit(1);
			}
		}

		private static void CheckInputFile(string inputPath)
		{
			if (!File.Exists(inputPath))
			{
				Console.WriteLine("Could not find file: " + inputPath);
				Environment.Exit(1);
			}
		}

		private static void CheckOutputFile(string outputPath)
		{
			if (File.Exists(outputPath) && !overwrite)
			{
				Console.WriteLine("File already exists: " + outputPath);
				Environment.Exit(1);
			}
		}

		private static void Sort(string inputPath, string outputPath)
		{
			var sorter = new CsvSorter(sortingColumnIndex, columnSeparator, Environment.NewLine);

			var result = sorter.Sort(File.OpenRead(inputPath));
			using (var sw = new StreamWriter(File.Create(outputPath)))
			{
				foreach (var row in result)
				{
					sw.Write(row);
					sw.Write(Environment.NewLine);
				}
				sw.Flush();
			}
		}
	}
}
