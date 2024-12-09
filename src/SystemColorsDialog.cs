using System.Windows.Controls;
using WpfColorHelper;

namespace CsInlineColorViz;

class SystemColorsDialog : ColorSelectionDialog
{
	public SystemColorsDialog() : base()
	{
		var sp = new StackPanel();

		foreach (var color in ColorListHelper.SystemColorsAlphabetically())
		{
			if (ColorHelper.TryGetColor(color.Name, out System.Windows.Media.Color clr))
			{
				sp.Children.Add(CreateButton(color.Name, clr));
			}
		}

		var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };

		this.Content = sv;
	}
}
