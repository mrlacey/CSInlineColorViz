using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

[TestClass]
public class SvgTaggerTests : BaseTaggerTests
{
	[TestMethod]
	public void CanMatch_Attribute_SixChar()
	{
		var testLine = "<path fill=\"#663399\" d=\"M25.927979,12.675003L25.927979,0,18.458z\"/>";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatch_Attribute_SixChar_LowerCaseAlpha()
	{
		var testLine = "<path fill=\"#aa00ee\" d=\"M25.927979,12.675003L25.927979,0,18.458z\"/>";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatch_Attribute_EightChar()
	{
		var testLine = "<path fill=\"#FF663399\" d=\"M25.927979,12.675003L25.927979,0,18.458z\"/>";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatch_Style_SixChar()
	{
		var testLine = "<path style=\"fill:#ffffff;fill-opacity:1;\" />";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatch_Style_Multiples()
	{
		var testLine = "<path style=\"fill:#ffffff;fill-opacity:1;stroke:#ffffff;\" />";

		CreatesExpectedMatchAndTag(testLine, 2);
	}

	private void CreatesOneMatchAndTag(string lineOfCode)
	{
		CreatesExpectedMatchAndTag(lineOfCode, numberExpected:1);
	}

	private void CreatesExpectedMatchAndTag(string lineOfCode, int numberExpected)
	{
		var sut = new SvgTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut, "Failed to create tagger");

		var matches = sut.ColorExpression.Matches(lineOfCode).Cast<Match>();

		Assert.AreEqual(numberExpected, matches.Count());

		var matchList = matches.ToList();

		for (int i = 0; i < numberExpected; i++)
		{
			var tag = sut.TryCreateTagForMatch(matchList[i], 0, 0, 0, lineOfCode);

			Assert.IsNotNull(tag, $"Failed to create a tag from the match: {matchList[i]}");
		}
	}
}
