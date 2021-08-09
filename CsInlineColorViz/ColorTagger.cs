using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;

namespace CsInlineColorViz
{
    internal sealed class ColorTagger : RegexTagger<ColorTag>
    {
        internal ColorTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex(@"(Color.)([a-z]{3,})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override ColorTag TryCreateTagForMatch(Match match, int lineStart, int spanStart, string snapshotText)
        {
            if (snapshotText.Contains(match.Value) && match.Groups.Count == 3)
            {
                var value = match.Groups[2].Value;

                int matchPos;

                if (spanStart > 0)
                {
                    // looking at a span that is smaller than the whole document
                    matchPos = snapshotText.IndexOf(match.Value, spanStart);
                }
                else
                {
                    matchPos = lineStart + match.Index;
                }

                if (matchPos >= 0)
                {
                    var openingTagPos = snapshotText.Substring(0, matchPos).LastIndexOf('<');

                    var closingTagPos = snapshotText.Substring(matchPos).IndexOf('>');

                    if (openingTagPos >= 0 && closingTagPos > 0)
                    {
                        var elementString = snapshotText.Substring(openingTagPos, closingTagPos + matchPos - openingTagPos + 1);

                        var elementName = elementString.Split(' ')[0].Split('<', ':').ToList().Last().Trim();

                                    //if (SymbolIconAdornment.KnownSymbols.ContainsKey(value))
                                    //{
                                    //    return new ColorTag(value);
                                    //}
                                    //else
                                    //{
                                    //    System.Diagnostics.Debug.WriteLine($"unknown value {value}");
                                    //}


                    }
                }
            }

            return null;
        }
    }
}
