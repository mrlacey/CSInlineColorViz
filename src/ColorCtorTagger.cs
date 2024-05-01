using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz
{
	internal sealed class ColorCtorTagger : RegexTagger<ColorTag>
	{
		internal ColorCtorTagger(ITextBuffer buffer)
			: base(buffer, new[] { new Regex(@"(new Color\(|new Microsoft.Maui.Graphics.Color\()([0-9]{1,3})(,{1} ?)([0-9]{1,3})(,{1} ?)([0-9]{1,3})(\))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
		{
		}

		protected override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
		{
			if (lineText.Contains(match.Value) && match.Groups.Count == 8)
			{
				var r = match.Groups[2].Value;
				var g = match.Groups[4].Value;
				var b = match.Groups[6].Value;

				try
				{
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
}
