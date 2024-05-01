using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Tagging;

namespace CSInlineColorViz.Tests;

// TODO: Add lots of lovely tests
[TestClass]
public class UnityTaggerTests
{
	[TestMethod]
	public void CanMatchSingleColor()
	{
		var sut = new UnityTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut);

		var lineWithColor = "<Color=#FF0000>";

		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		// TODO: add support for line number, line start, and span start
		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

		Assert.IsNotNull(tag);

		// TODO: check that the color is correct
	}

	[TestMethod]
	public void DoesNotMatch_InconsequentialText()
	{
		var sut = new UnityTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut);

		var lineWithoutColor = "just some inconsequential text string";

		var matches = sut.ColorExpression.Matches(lineWithoutColor).Cast<Match>();

		Assert.AreEqual(0, matches.Count());

		// TODO: add support for line number, line start, and span start
		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithoutColor);

		Assert.IsNull(tag);
	}
}
