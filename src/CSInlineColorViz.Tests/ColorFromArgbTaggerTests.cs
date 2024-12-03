using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

[TestClass]
public class ColorFromArgbTaggerTests : BaseTaggerTests
{
	[TestMethod]
	public void NoNameSpace()
	{
		var testLine = "var color = Color.FromArgb(255, 125, 125, 125); ";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void WithNameSpace_System_Drawing()
	{
		var testLine = "var color = System.Drawing.Color.FromArgb(255, 255, 125, 0); ";

		CreatesOneMatchAndTag(testLine);
	}

	[TestMethod]
	public void WithNameSpace_Windows_UI()
	{
		var testLine = "var color = Windows.UI.Color.FromArgb(255, 125, 125, 125); ";

		CreatesOneMatchAndTag(testLine);
	}

	private void CreatesOneMatchAndTag(string lineOfCode)
	{
		var sut = new ColorArgbTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut, "Failed to create tagger");

		var matches = sut.ColorExpression.Matches(lineOfCode).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineOfCode);

		Assert.IsNotNull(tag, "Failed to create a tag from the match");
	}
}
