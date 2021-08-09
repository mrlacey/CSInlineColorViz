using System.Windows.Controls;

namespace CsInlineColorViz
{
    internal sealed class ColorAdornment : Grid
    {
        public ColorAdornment(ColorTag tag)
        {
            ClrTag = tag;
            Height = 16;
            Width = 16;
            SetBackground();
        }

        public ColorTag ClrTag { get; private set; }

        internal void Update(ColorTag dataTag)
        {
            ClrTag = dataTag;
            SetBackground();
        }

        private void SetBackground()
        {
            this.Background = new System.Windows.Media.SolidColorBrush(ClrTag.Clr);
        }
    }
}
