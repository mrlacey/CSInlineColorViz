using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz
{
    internal sealed class MauiProjTagger : RegexTagger<ColorTag>
    {
        internal MauiProjTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex("( Color=\"#)([0-9A-F]{3,8})(\")", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override ColorTag TryCreateTagForMatch(Match match, int lineStart, int spanStart, string lineText)
        {
            if (lineText.Contains(match.Value) && match.Groups.Count == 4)
            {
                var value = match.Groups[2].Value;

                if (ColorHelper.TryGetHexColor($"#{value}", out Color clr))
                {
                    // TODO: Update this when have the line number
                    return new ColorTag(clr, match, -1, lineStart, PopupType.None);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to understand '{value}' as a valid color.");
                }
            }

            return null;
        }
    }
}
