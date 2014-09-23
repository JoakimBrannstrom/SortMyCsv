using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SortMyCsv.UnitTests
{
	[TestClass]
	public class CsvTestRunner
	{
		string _rowSeparator;
		CsvSorter _csvSorter;

		public CsvTestRunner(int sortingColumnIndex, char columnSeparator, string rowSeparator)
		{
			_rowSeparator = rowSeparator;

			_csvSorter = new CsvSorter(sortingColumnIndex, columnSeparator, _rowSeparator);
		}

		public string[] RunTest(params string[] rows)
		{
			using(var input = new MemoryStream())
			{
				using (var sw = new StreamWriter(input))
				{
					foreach (var row in rows)
					{
						sw.Write(row);
						sw.Write(_rowSeparator);
					}

					sw.Flush();
					input.Seek(0, SeekOrigin.Begin);

					// Act
					return _csvSorter.Sort(input).ToArray();
				}
			}
		}
	}

	[TestClass]
	public class CsvSorterInitialTests
	{
		CsvTestRunner _testRunner;

		[TestInitialize]
		public void Setup()
		{
			_testRunner = new CsvTestRunner(0, ',', Environment.NewLine);
		}

		[TestMethod]
		public void GivenEmptyStream_WhenSorting_ThenTheResultShouldBeAnEmptyStream()
		{
			// Act
			var result = _testRunner.RunTest();

			// Assert
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void GivenStreamWithOneLine_WhenSorting_ThenTheOutputShouldContainThatLine()
		{
			// Arrange
			var firstRow = "1,first,row";

			// Act
			var result = _testRunner.RunTest(firstRow);

			// Assert
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual(firstRow, result[0]);
		}

		[TestMethod]
		public void GivenStreamWithTwoLines_WhenSorting_ThenTheOutputShouldContainBothLines()
		{
			// Arrange
			var firstRow = "1,first,row";
			var secondRow = "2,second,row";

			// Act
			var result = _testRunner.RunTest(firstRow, secondRow);

			// Assert
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void GivenColumnsAreSeparatedByPipes_WhenSorting_ThenTheOutputShouldHaveCorrectSorting()
		{
			// Arrange
			var first = "1|C|b";
			var second = "2|B|a";
			var third = "3|A|c";
			_testRunner = new CsvTestRunner(2, '|', Environment.NewLine);

			// Act
			var result = _testRunner.RunTest(first, second, third);

			// Assert
			Assert.AreEqual(second, result[0]);
			Assert.AreEqual(first, result[1]);
			Assert.AreEqual(third, result[2]);
		}

		[TestMethod]
		public void GivenStreamWithTwoLines_WhenSorting_ThenTheOutputShouldHaveCorrectSorting()
		{
			// Arrange
			var firstRow = "2,first,row";
			var secondRow = "1,second,row";

			// Act
			var result = _testRunner.RunTest(firstRow, secondRow);

			// Assert
			Assert.AreEqual(secondRow, result[0]);
			Assert.AreEqual(firstRow, result[1]);
		}
	}

	[TestClass]
	public class CsvSorterColumnIndexTests
	{
		const string first = "A,5,e";
		const string second = "B,3,d";
		const string third = "C,2,c";
		const string fourth = "D,1,b";
		const string fifth = "E,4,a";

		[TestMethod]
		public void GivenSortOnFirstColumn_WhenSortingIsDone_ThenOrderShouldBe12345()
		{
			// Arrange
			var testRunner = new CsvTestRunner(0, ',', Environment.NewLine);

			// Act
			var result = testRunner.RunTest(first, second, third, fourth, fifth);

			// Assert
			Assert.AreEqual(first, result[0]);
			Assert.AreEqual(second, result[1]);
			Assert.AreEqual(third, result[2]);
			Assert.AreEqual(fourth, result[3]);
			Assert.AreEqual(fifth, result[4]);
		}

		[TestMethod]
		public void GivenSortOnSecondColumn_WhenSortingIsDone_ThenOrderShouldBe43251()
		{
			// Arrange
			var testRunner = new CsvTestRunner(1, ',', Environment.NewLine);

			// Act
			var result = testRunner.RunTest(first, second, third, fourth, fifth);

			// Assert
			Assert.AreEqual(fourth, result[0]);
			Assert.AreEqual(third, result[1]);
			Assert.AreEqual(second, result[2]);
			Assert.AreEqual(fifth, result[3]);
			Assert.AreEqual(first, result[4]);
		}

		[TestMethod]
		public void GivenSortOnThirdColumn_WhenSortingIsDone_ThenOrderShouldBe54321()
		{
			// Arrange
			var testRunner = new CsvTestRunner(2, ',', Environment.NewLine);

			// Act
			var result = testRunner.RunTest(first, second, third, fourth, fifth);

			// Assert
			Assert.AreEqual(fifth, result[0]);
			Assert.AreEqual(fourth, result[1]);
			Assert.AreEqual(third, result[2]);
			Assert.AreEqual(second, result[3]);
			Assert.AreEqual(first, result[4]);
		}
	}
}
