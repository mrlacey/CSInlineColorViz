using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

[TestClass]
public class HexIntTaggerTests : BaseTaggerTests
{
	[TestMethod]
	public void CanMatch_Attribute_ThreeChar()
	{
		var testLine = " 0x123 ";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatch_Attribute_SixChar()
	{
		var testLine = " 0x123456 ";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void CanMatch_Attribute_EightChar()
	{
		var testLine = " 0x12345678 ";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void DoesNotMatch_WhenNoWordBoundary()
	{
		var testLine = " // windows size = 800x600 ";

		CreatesNoMatches(testLine);
	}

	private void CreatesNoMatches(string lineOfCode)
	{
		CreatesExpectedMatchAndTag(lineOfCode, numberExpected: 0);
	}

	private void CreatesOneMatchAndTag(string lineOfCode)
	{
		CreatesExpectedMatchAndTag(lineOfCode, numberExpected: 1);
	}

	private void CreatesExpectedMatchAndTag(string lineOfCode, int numberExpected)
	{
		var sut = new HexIntTagger(new FakeTextBuffer());

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
