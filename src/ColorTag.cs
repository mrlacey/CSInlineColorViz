using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Tagging;

namespace CsInlineColorViz
{
    internal class ColorTag : ITag
    {
        public ColorTag(Color clr, Match match, PopupType popupType)
        {
            Clr = clr;
            Match = match;
            PopupType = popupType;
        }

        public Color Clr { get; }
        public Match Match { get; }
        public PopupType PopupType { get; }
    }
}
