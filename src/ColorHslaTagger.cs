﻿using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz
{
    internal sealed class ColorHslaTagger : RegexTagger<ColorTag>
    {
        internal ColorHslaTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex(@"(Microsoft.Maui.Graphics.Color.FromHsla\(|Color.FromHsla\()([0-9fFDd., ]{5,})(\))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
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
                    if (ColorHelper.TryGetHslaColor(value, out Color clr))
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
    internal sealed class ColorHsvaTagger : RegexTagger<ColorTag>
    {
        internal ColorHsvaTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex(@"(Microsoft.Maui.Graphics.Color.FromHsva\(|Color.FromHsva\(|Microsoft.Maui.Graphics.Color.FromHsv\(|Color.FromHsv\()([0-9fFDd., ]{5,})(\))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
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
                    if (ColorHelper.TryGetHsvaColor(value, out Color clr))
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
}
