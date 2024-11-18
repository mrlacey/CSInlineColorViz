using System.Windows.Controls;
using WpfColorHelper;

namespace CsInlineColorViz;

class NamedColorDialog : ColorSelectionDialog
{
	public NamedColorDialog() : base()
	{
		var tabs = new TabControl();

		var tab1 = new TabItem { Header = "  A-Z  " };

		var sp = new StackPanel();

		foreach (var color in ColorListHelper.SystemDrawingColorsAlphabetical())
		{
			if (ColorHelper.TryGetColor(color.Name, out System.Windows.Media.Color clr))
			{
				sp.Children.Add(CreateButton(color.Name, clr));
			}
		}

		var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };
		tab1.Content = sv;

		var tab2 = new TabItem { Header = "   🌈   " };

		var sp2 = new StackPanel();

		foreach (var color in ColorListHelper.SystemDrawingColorsSpectrum())
		{
			if (ColorHelper.TryGetColor(color.Name, out System.Windows.Media.Color clr))
			{
				sp2.Children.Add(CreateButton(color.Name, clr));
			}
		}

		var sv2 = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp2 };
		tab2.Content = sv2;

		tabs.Items.Add(tab1);
		tabs.Items.Add(tab2);

		this.Content = tabs;
	}
}
