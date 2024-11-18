using System.Windows.Controls;
using WpfColorHelper;

namespace CsInlineColorViz;

class ConsoleColorDialog : ColorSelectionDialog
{
	public ConsoleColorDialog() : base()
	{
		var sp = new StackPanel();

		foreach (var colorName in ColorListHelper.ConsoleColorsNamesAlphabetical())
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
