using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Tagging;

namespace CSInlineColorViz.Tests;

// https://docs.unity3d.com/ScriptReference/Color32-ctor.html
[TestClass]
public class Color32TaggerTests : BaseTaggerTests
{
	[TestMethod]
	public void CanMatchSingleColor()
	{
		var sut = new Color32Tagger(new FakeTextBuffer());

		Assert.IsNotNull(sut);

		var lineWithColor = "var testValue = new Color32(255, 0, 0, 255)";

		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

		Assert.IsNotNull(tag);
	}

	[TestMethod]
	public void CanMatchSingleColor_NoSpaces()
	{
		var sut = new Color32Tagger(new FakeTextBuffer());

		Assert.IsNotNull(sut);

		var lineWithColor = "var testValue = new Color32(255,0,0,255)";

		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

		Assert.IsNotNull(tag);
	}

	// Not yet supported. Can be added later if people really write code like this.
	//[TestMethod]
	//public void CanMatchSingleColor_SpacesBeforeCommas()
	//{
	//	var sut = new Color32Tagger(new FakeTextBuffer());

	//	Assert.IsNotNull(sut);

	//	var lineWithColor = "var testValue = new Color32(255 ,0 ,0 ,255)";

	//	var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

	//	Assert.AreEqual(1, matches.Count());

	//	var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

	//	Assert.IsNotNull(tag);
	//}
}
