using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz
{
    internal sealed class ColorFromIntTagger : RegexTagger<ColorTag>
    {
        internal ColorFromIntTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex(@"(Microsoft.Maui.Graphics.Color.FromInt\(|Color.FromInt\(|Microsoft.Maui.Graphics.Color.FromUint\(|Color.FromUint\()([0-9]{1,})(\))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
        {
            if (lineText.Contains(match.Value) && match.Groups.Count == 4)
            {
                var value = match.Groups[2].Value;
                var precedingChar = match.Index > 0 ? lineText[match.Index - 1] : ' ';

                // Do this check here rather than as part of the RegEx so don't have to adjust the insertion point for the adornment
                if (new[] { ' ', ',', '(' }.Contains(precedingChar))
                {
                    if (match.Groups[0].Value.Contains("Uint"))
                    {
                        if (ColorHelper.TryGetFromUint(value, out Color clr))
                        {
                            return new ColorTag(clr, match, lineNumber, lineStart, PopupType.None);
                        }
                    }
                    else
                    {
                        if (ColorHelper.TryGetFromInt(value, out Color clr))
                        {
                            return new ColorTag(clr, match, lineNumber, lineStart, PopupType.None);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Failed to understand '{value}' as a valid color name.");
                }
            }

            return null;
        }
    }
}
