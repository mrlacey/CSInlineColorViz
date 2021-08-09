using Microsoft.VisualStudio.Text.Tagging;
using System.Windows.Media;

namespace CsInlineColorViz
{
    internal class ColorTag : ITag
    {
        public ColorTag(Color clr)
        {
            Clr = clr;
        }

        public Color Clr { get; }
    }
}
