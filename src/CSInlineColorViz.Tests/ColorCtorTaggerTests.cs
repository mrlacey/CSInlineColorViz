using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

// https://docs.unity3d.com/ScriptReference/Color-ctor.html
[TestClass]
public class ColorCtorTaggerTests : BaseTaggerTests
{
	[TestMethod]
	public void CanMatchSingleColor_3Bytes()
	{
		var sut = new ColorCtorTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut);

		var lineWithColor = "var testValue = new Color(255, 0, 0);";

		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

		Assert.IsNotNull(tag);
	}

	[TestMethod]
	public void CanMatchSingleColor_3Floats()
	{
		var sut = new ColorCtorTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut);

		var lineWithColor = "var testValue = new Color(1.0f, 0.0f, 0.0f)";

		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

		Assert.IsNotNull(tag);
	}

	[TestMethod]
	public void CanMatchSingleColor_4Floats()
	{
		var sut = new ColorCtorTagger(new FakeTextBuffer());

		Assert.IsNotNull(sut);

		var lineWithColor = "var testValue = new Color(0.0f, 0.0f, 0.0f, 0.5f)";

		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

		Assert.AreEqual(1, matches.Count());

		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

		Assert.IsNotNull(tag);
	}
}

//[TestClass]
//public class FloatColor32TaggerTests : BaseTaggerTests
//{
//	[TestMethod]
//	public void CanMatchSingleColor_NoDecimals()
//	{
//		var sut = new Color32Tagger(new FakeTextBuffer());

//		Assert.IsNotNull(sut);

//		var lineWithColor = "var testValue = new Color32(1, 0, 0, 1)";

//		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

//		Assert.AreEqual(1, matches.Count());

//		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

//		Assert.IsNotNull(tag);
//	}

//	[TestMethod]
//	public void CanMatchSingleColor_WithDecimals()
//	{
//		var sut = new Color32Tagger(new FakeTextBuffer());

//		Assert.IsNotNull(sut);

//		var lineWithColor = "var testValue = new Color32(1.0, 1.0, 0.0, 1.0)";

//		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

//		Assert.AreEqual(1, matches.Count());

//		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

//		Assert.IsNotNull(tag);
//	}

//	[TestMethod]
//	public void CanMatchSingleColor_WithDecimalsAndF()
//	{
//		var sut = new Color32Tagger(new FakeTextBuffer());

//		Assert.IsNotNull(sut);

//		var lineWithColor = "var testValue = new Color32(0.0f, 1.0f, 1.0f, 1.0f)";

//		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

//		Assert.AreEqual(1, matches.Count());

//		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

//		Assert.IsNotNull(tag);
//	}

//	[TestMethod]
//	public void CanMatchSingleColor_WithDecimalsAndFButNoSpaces()
//	{
//		var sut = new Color32Tagger(new FakeTextBuffer());

//		Assert.IsNotNull(sut);

//		var lineWithColor = "var testValue = new Color32(0.0f,1.0f,1.0f,1.0f)";

//		var matches = sut.ColorExpression.Matches(lineWithColor).Cast<Match>();

//		Assert.AreEqual(1, matches.Count());

//		var tag = sut.TryCreateTagForMatch(matches.First(), 0, 0, 0, lineWithColor);

//		Assert.IsNotNull(tag);
//	}

//	[TestMethod]
//	public void DoesNotMatch_InconsequentialText()
//	{
//		base.DoesNotMatch_InconsequentialText<Color32Tagger>();
//	}
//}

