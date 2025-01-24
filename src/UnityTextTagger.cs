using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using WpfColorHelper;

namespace CsInlineColorViz;

internal sealed class UnityTextTagger : RegexTagger<ColorTag>, ITestableRegexColorTagger
{
	internal static Regex regularExpression = new("(<Color)(=)([a-z]{3,})(?=>)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public Regex ColorExpression => regularExpression;

	public UnityTextTagger(ITextBuffer buffer)
		: base(buffer, [regularExpression])
	{
	}

	internal override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
	{
		if (lineText.Contains(match.Value) && match.Groups.Count == 4)
		{
			var value = match.Groups[3].Value;

			if (ColorHelper.TryGetColor($"{value}", out Color clr))
			{
				return new ColorTag(clr, match, lineNumber, lineStart, PopupType.UnityColors);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Failed to understand '{value}' as a valid color.");
			}
		}

		return null;
	}
}
