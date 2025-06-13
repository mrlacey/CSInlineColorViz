﻿using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using WpfColorHelper;

namespace CsInlineColorViz;

internal sealed class HexIntTagger : RegexTagger<ColorTag>, ITestableRegexColorTagger
{
	internal static Regex regularExpression = new(@"(?:\b|\s)(0x)([0-9A-F]{3,8})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public Regex ColorExpression => regularExpression;

	internal HexIntTagger(ITextBuffer buffer)
		: base(buffer, [regularExpression])
	{
	}

	internal override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
	{
		if (lineText.Contains(match.Value) && match.Groups.Count == 3)
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
