using System.Text.RegularExpressions;

namespace CsInlineColorViz;

public interface ITestableRegexColorTagger
{
	public Regex ColorExpression { get; }
}