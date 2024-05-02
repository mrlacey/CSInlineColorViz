using System;
using System.Linq;
using System.Text.RegularExpressions;
using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

public class BaseTaggerTests
{
	public void DoesNotMatch_InconsequentialText<T>()
		where T : RegexTagger<ColorTag>, ITestableRegexColorTagger
	{
		T sut = (T)Activator.CreateInstance(typeof(T), new FakeTextBuffer());

		Assert.IsNotNull(sut);

		// TODO: repeat this with multiple other misc strings

		var lineWithoutColor = "just some inconsequential text string";

		var matches = sut.ColorExpression.Matches(lineWithoutColor).Cast<Match>();

		Assert.AreEqual(0, matches.Count());
	}
}