using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz;

internal sealed class ColorCtorTagger : RegexTagger<ColorTag>, ITestableRegexColorTagger
{
	internal static Regex regularExpression = new(@"(new Color\(|new UnityEngine.Color\(|new Microsoft.Maui.Graphics.Color\()([0-9.f]{1,6})(,{1} ?)([0-9.f]{1,6})(,{1} ?)([0-9.f]{1,6})([, ]*)([0-9.f]*)(\))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public Regex ColorExpression => regularExpression;

	public ColorCtorTagger(ITextBuffer buffer)
		: base(buffer, [regularExpression])
	{
	}

	internal override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
	{
		if (lineText.Contains(match.Value) && match.Groups.Count == 10)
		{
			var r = match.Groups[2].Value;
			var g = match.Groups[4].Value;
			var b = match.Groups[6].Value;
			var a = match.Groups[8].Value;

			// Alpha will be an empty string if not provided
			if (string.IsNullOrEmpty(a))
			{
				a = "1";
			}

			try
			{
				static byte ParseToByte(string value) => ColorHelper.FloatToByte(float.Parse(value.Replace("f", string.Empty).Replace("F", string.Empty)));

				if (r.Contains(".") || g.Contains(".") || b.Contains(".") || a.Contains("."))
				{
					return new ColorTag(Color.FromArgb(ParseToByte(a), ParseToByte(r), ParseToByte(g), ParseToByte(b)), match, lineNumber, lineStart, PopupType.None);
				}

				return new ColorTag(Color.FromRgb(byte.Parse(r), byte.Parse(g), byte.Parse(b)), match, lineNumber, lineStart, PopupType.None);
			}
			catch (System.Exception)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to create color from r='{r}' g='{g}' b='{b}'");
			}
		}

		return null;
	}
}
