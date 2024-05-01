using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz;

internal sealed class ColorArgbTagger : RegexTagger<ColorTag>, ITestableRegexColorTagger
{
	internal static Regex regularExpression = new(@"(System.Drawing.Color.FromArgb\(|Color.FromArgb\()([0-9, ]{1,}|[0-9, ]{2,}Color.[a-zA-Z]{3,})(\))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public Regex ColorExpression => regularExpression;

	internal ColorArgbTagger(ITextBuffer buffer)
		: base(buffer, [regularExpression])
	{
	}

	internal override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
	{
		if (lineText.Contains(match.Value) && match.Groups.Count == 4)
		{
			var value = match.Groups[2].Value;
			var precedingChar = match.Index > 0 ? lineText[match.Index - 1] : ' ';

			// Do this check here rather than as part of the RegEx so don't have to adjust the insertion point for the adornment
			if (new[] { ' ', ',', '(' }.Contains(precedingChar))
			{
				if (ColorHelper.TryGetArgbColor(value, out Color clr))
				{
					return new ColorTag(clr, match, lineNumber, lineStart, PopupType.None);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine($"Failed to understand '{value}' as a valid color.");
				}
			}
		}

		return null;
	}
}
