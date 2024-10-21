using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz;

internal sealed class UnityTagger : RegexTagger<ColorTag>, ITestableRegexColorTagger
{
	internal static Regex regularExpression = new Regex("(<Color=#)([0-9A-F]{3,8})(>)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public Regex ColorExpression => regularExpression;

	public UnityTagger(ITextBuffer buffer)
		: base(buffer, [regularExpression])
	{
	}

	internal override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
	{
		if (lineText.Contains(match.Value) && match.Groups.Count == 4)
		{
			var value = match.Groups[2].Value;

			if (ColorHelper.TryGetHexColor($"#{value}", out Color clr))
			{
				return new ColorTag(clr, match, lineNumber, lineStart, PopupType.None);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Failed to understand '{value}' as a valid color.");
			}
		}

		return null;
	}
}

public interface ITestableRegexColorTagger
{
	public Regex ColorExpression { get; }
}