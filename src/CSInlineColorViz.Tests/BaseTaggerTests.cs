using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

public class BaseTaggerTests
{
	public void DoesNotMatch_Text(Regex regex, string text)
	{
		var matches = regex.Matches(text).Cast<Match>();

		Assert.AreEqual(0, matches.Count());
	}
}
