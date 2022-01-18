using Microsoft.VisualStudio.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace CsInlineColorViz
{
    internal sealed class ColorTagger : RegexTagger<ColorTag>
    {
        internal ColorTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex(@"(Color)([\.]{1})([a-zA-Z]{3,})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override ColorTag TryCreateTagForMatch(Match match, int lineStart, int spanStart, string lineText)
        {
            if (lineText.Contains(match.Value) && match.Groups.Count == 4)
            {
                var value = match.Groups[3].Value;
                var precedingChar = match.Index > 0 ? lineText[match.Index - 1] : ' ';

                if (new[] { ' ', ',', '(' }.Contains(precedingChar))
                {
                    if (ColorHelper.TryGetColor(value, out Color clr))
                    {
                        return new ColorTag(clr);
                    }
                }
            }

            return null;
        }
    }
}
