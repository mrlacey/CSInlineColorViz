using Microsoft.VisualStudio.Text;
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

        protected override ColorTag TryCreateTagForMatch(Match match, int lineStart, int spanStart, string snapshotText)
        {
            if (snapshotText.Contains(match.Value) && match.Groups.Count == 4)
            {
                var value = match.Groups[3].Value;

                // TODO: check that the group 1 match has a non-alphanumeric (or underscore) immediately before it.
                if (ColorHelper.TryGetColor(value, out Color clr))
                {
                    return new ColorTag(clr);
                }
            }

            return null;
        }
    }
}
