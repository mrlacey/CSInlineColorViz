using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz;

internal sealed class Color32Tagger : RegexTagger<ColorTag>, ITestableRegexColorTagger
{
	internal static Regex regularExpression = new(@"(new Color32\(|new UnityEngine.Color32\()([0-9]{1,3})(,{1} ?)([0-9]{1,3})(,{1} ?)([0-9]{1,3})(,{1} ?)([0-9]{1,3})(\))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public Regex ColorExpression => regularExpression;

	public Color32Tagger(ITextBuffer buffer)
		: base(buffer, [regularExpression])
	{
	}

	internal override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
	{
		if (lineText.Contains(match.Value) && match.Groups.Count == 10)
		{
			try
			{
				var r = byte.Parse(match.Groups[2].Value);
				var g = byte.Parse(match.Groups[4].Value);
				var b = byte.Parse(match.Groups[6].Value);
				var a = byte.Parse(match.Groups[8].Value);

				try
				{
					return new ColorTag(Color.FromArgb(a, r, g, b), match, lineNumber, lineStart, PopupType.None);
				}
				catch (Exception)
				{
					System.Diagnostics.Debug.WriteLine($"Failed to create color from r='{r}' g='{g}' b='{b}' a='{a}'");
				}
			}
			catch (Exception exc)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to create color from '{match.Value}' Exception:'{exc.Message}'");
			}
		}

		return null;
	}
}
