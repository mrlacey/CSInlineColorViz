using System.Windows.Controls;

namespace CsInlineColorViz
{
    class ConsoleColorDialog : ColorSelectionDialog
    {
        public ConsoleColorDialog() : base()
        {
            var sp = new StackPanel();

            foreach (var colorName in ColorHelper.ConsoleColorsNamesAlphabetical())
            {
                if (ColorHelper.TryGetColor(colorName, out System.Windows.Media.Color clr))
                {
                    sp.Children.Add(CreateButton(colorName, clr));
                }
            }

            var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };

            this.Content = sv;
        }
    }
}
