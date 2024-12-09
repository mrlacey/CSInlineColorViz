using System;
using System.Linq;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

[TestClass]
public class ColorListHelperTests
{
	[TestMethod]
	public void Ensure_ConsoleColor_AlphabeticalList_AsEnum()
	{
		var enumValues = Enum.GetValues(typeof(System.ConsoleColor)).Cast<object>().ToList().OrderBy(o => o.ToString()).Select(o => o.ToString()).ToList();

		var helperValues = ColorListHelper.ConsoleColorsNamesAlphabetical().ToList();

		Assert.AreEqual(enumValues.Count(), helperValues.Count);

		for (int i = 0; i < enumValues.Count(); i++)
		{
			Assert.AreEqual(enumValues[i], helperValues[i]);
		}
	}

	[TestMethod]
	public void SystemDrawingColors_AlphabeticalAndSpectrumLists_ContainSame()
	{
		var alphaValues = ColorListHelper.SystemDrawingColorsAlphabetical().ToList();

		var spectrumValues = ColorListHelper.SystemDrawingColorsSpectrum().ToList();

		Assert.AreEqual(alphaValues.Count(), spectrumValues.Count());
	}

	[TestMethod]
	public void SystemDrawingColors_Alphabetical_NoDuplicates()
	{
		var alphaValues = ColorListHelper.SystemDrawingColorsAlphabetical().ToList();
		var alphaValuesDistinct = ColorListHelper.SystemDrawingColorsAlphabetical().ToList().Distinct();

		Assert.AreEqual(alphaValues.Count(), alphaValuesDistinct.Count());
	}

	[TestMethod]
	public void SystemDrawingColors_SpectrumLists_NoDuplicates()
	{
		var spectrumValues = ColorListHelper.SystemDrawingColorsSpectrum().ToList();
		var spectrumValuesDistinct = ColorListHelper.SystemDrawingColorsSpectrum().ToList().Distinct();

		Assert.AreEqual(spectrumValues.Count(), spectrumValuesDistinct.Count());
	}
}
