using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

// https://github.com/maddymontaquila/funcolors/blob/main/FunColor.cs
[TestClass]
public class FunColorTaggerTests : BaseTaggerTests
{
	[TestMethod]
	public void CanMatchKnownColor_Alphabetical()
	{
		var testLine = "var color = FunColor.BarbiePink; ";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatchKnownColor_AlphaNumeric()
	{
		var testLine = "var color = FunColor.DotNetPurple2024; ";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatchKnownColor_FullyQualified()
	{
		var testLine = "var color = FunColors.FunColor.ShrekGreen; ";

		CreatesOneMatchAndTag(testLine);
	}

	private void CreatesOneMatchAndTag(string lineOfCode)
	{
		var sut = new FunColorTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut, "Failed to create tagger");

		var matches = sut.ColorExpression.Matches(lineOfCode).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineOfCode);

		Assert.IsNotNull(tag, "Failed to create a tag from the match");
	}
}
