using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using WpfColorHelper;

namespace CsInlineColorViz;

internal sealed class FunColorTagger : RegexTagger<ColorTag>, ITestableRegexColorTagger
{
	// Note this contains an empty group, so that the output groups match ColorTagger,
	// as the adornment expects values in specific groups when making changes with the dialog
	internal static Regex regularExpression = new(@"(FunColors.FunColor.|FunColor.)()([a-z,A-Z0-9]{5,})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public Regex ColorExpression => regularExpression;

	internal FunColorTagger(ITextBuffer buffer)
		: base(buffer, [regularExpression])
	{
	}

	internal override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
	{
		if (lineText.Contains(match.Value) && match.Groups.Count == 4)
		{
			var value = match.Groups[3].Value;
			var precedingChar = match.Index > 0 ? lineText[match.Index - 1] : ' ';

			// Do this check here rather than as part of the RegEx so don't have to adjust the insertion point for the adornment
			if (new[] { ' ', ',', '(' }.Contains(precedingChar))
			{
				if (ColorHelper.TryGetColor(value, out Color clr))
				{
					return new ColorTag(clr, match, lineNumber, lineStart, PopupType.FunColors);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine($"Failed to understand '{value}' as a valid color name.");
				}
			}
		}

		return null;
	}
}
