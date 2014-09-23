using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SortMyCsv
{
	public class CsvSorter
	{
		int _sortingColumnIndex;
		char _columnSeparator;
		string _rowSeparator;

		public CsvSorter(int sortingColumnIndex, char columnSeparator = ',', string rowSeparator = "\r\n")
		{
			_sortingColumnIndex = sortingColumnIndex;
			_columnSeparator = columnSeparator;
			_rowSeparator = rowSeparator;
		}

		public IEnumerable<string> Sort(Stream input)
		{
			var comp = new CsvComparer(_sortingColumnIndex, _columnSeparator);
			var sortedValues = new StreamToStrings(input, _rowSeparator)
								.Values
								.OrderBy(v => v, comp);

			foreach (var value in sortedValues)
			{
				yield return value;
			}
		}
	}

	public class CsvComparer : IComparer<string>
	{
		int _sortingColumnIndex;
		char _columnSeparator;

		public CsvComparer(int sortingColumnIndex, char columnSeparator)
		{
			_sortingColumnIndex = sortingColumnIndex;
			_columnSeparator = columnSeparator;
		}

		public int Compare(string x, string y)
		{
			var xKey = x.Split(_columnSeparator).Skip(_sortingColumnIndex).FirstOrDefault();
			var yKey = y.Split(_columnSeparator).Skip(_sortingColumnIndex).FirstOrDefault();

			if (xKey == null || yKey == null)
			{
				var message = string.Format("Could not find column(s) using the separator '{0}' and columnIndex '{1}', are all rows valid?",
											_columnSeparator, _sortingColumnIndex);
				throw new Exception(message);
			}

			return xKey.CompareTo(yKey);
		}
	}

	public class StreamToStrings
	{
		public string[] Values { get; private set; }

		public StreamToStrings(Stream s, string rowSeparator)
		{
			s.Seek(0, SeekOrigin.Begin);
			using(StreamReader sr = new StreamReader(s))
			{
				var content = sr.ReadToEnd();
				Values = content.Split(new[] { rowSeparator }, StringSplitOptions.RemoveEmptyEntries);
			}
		}
	}
}
