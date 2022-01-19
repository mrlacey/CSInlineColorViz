using System.Windows.Controls;

namespace CsInlineColorViz
{
    internal sealed class ColorAdornment : Grid
    {
        public ColorAdornment(ColorTag tag)
        {
            ClrTag = tag;
            Height = 12;
            Width = 12;
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Margin = new System.Windows.Thickness(1, 1, 1, 3);
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
