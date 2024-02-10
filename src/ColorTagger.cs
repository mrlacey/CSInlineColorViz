using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz
{
    internal sealed class ColorTagger : RegexTagger<ColorTag>
    {
        private bool haveLoggedUseCount = false;

        // TODO: Add support for System.Drawing.SystemColors and System.Windows.SystemColors?
        internal ColorTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex(@"(Color|Colors|ConsoleColor|System.Windows.Media.Colors|System.Drawing.Color|KnownColor|System.Drawing.KnownColor|Microsoft.UI.Colors|SystemColors|System.Drawing.SystemColors|System.Windows.SystemColors)([\.]{1})(?!From)([a-zA-Z]{3,})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override ColorTag TryCreateTagForMatch(Match match, int lineNumber, int lineStart, int spanStart, string lineText)
        {
            if (lineText.Contains(match.Value) && match.Groups.Count == 4)
            {
                var value = match.Groups[3].Value;
                var precedingChar = match.Index > 0 ? lineText[match.Index - 1] : ' ';

                // Do this check here rather than as part of the RegEx so don't have to adjust the insertion point for the adornment
                if (new[] { ' ', ',', '(', '\t' }.Contains(precedingChar))
                {
                    if (match.Groups[1].Value.EndsWith("KnownColor") || match.Groups[1].Value.EndsWith("SystemColors"))
                    {
                        if (Enum.TryParse(value, out System.Drawing.KnownColor knownColor))
                        {
                            value = ColorHelper.ToHex(System.Drawing.Color.FromKnownColor(knownColor));
                        }
                    }

                    if (ColorHelper.TryGetColor(value, out Color clr))
                    {
                        if (!haveLoggedUseCount)
                        {
                            // This Tagger is loaded once per open document
                            // Only record once per document regardless of how many times an adorner is created
                            haveLoggedUseCount = true;

                            _ = System.Threading.Tasks.Task.Run(async () =>
                            {
                                var settings = await InternalSettings.GetLiveInstanceAsync();
                                settings.UseCount += 1;
                                await settings.SaveAsync();
                            });
                        }

                        // TODO: Need to handle all the different popup types to support
                        if (match.Groups[1].Value.EndsWith(".Colors") || match.Groups[1].Value.EndsWith(".Color") || match.Groups[1].Value == "Colors" || match.Groups[1].Value == "Color")
                        {
                            return new ColorTag(clr, match, lineNumber, lineStart, PopupType.NamedColors);
                        }
                        else if (match.Groups[1].Value.EndsWith("ConsoleColor"))
                        {
                            return new ColorTag(clr, match, lineNumber, lineStart, PopupType.ConsoleColors);
                        }
                        else if (match.Groups[1].Value.EndsWith("KnownColor"))
                        {
                            return new ColorTag(clr, match, lineNumber, lineStart, PopupType.KnownColors);
                        }
                        else if (match.Groups[1].Value.EndsWith("SystemColors"))
                        {
                            return new ColorTag(clr, match, lineNumber, lineStart, PopupType.SystemColors);
                        }
                        else
                        {
                            return new ColorTag(clr, match, lineNumber, lineStart, PopupType.None);
                        }
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
