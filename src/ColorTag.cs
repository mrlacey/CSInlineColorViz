using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Tagging;

namespace CsInlineColorViz
{
    internal class ColorTag : ITag
    {
        public ColorTag(Color clr, Match match, int lineNumber, int lineCharOffset, PopupType popupType)
        {
            Clr = clr;
            Match = match;
            LineNumber = lineNumber;
            LineCharOffset = lineCharOffset;
            PopupType = popupType;
        }

        public Color Clr { get; }
        public Match Match { get; }
        public int LineNumber { get; }
        public int LineCharOffset { get; }
        public PopupType PopupType { get; }
    }
}
