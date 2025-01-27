using System.Windows.Controls;
using WpfColorHelper;

namespace CsInlineColorViz;

class UnityColorsDialog : ColorSelectionDialog
{
	public UnityColorsDialog() : base()
	{
		var sp = new StackPanel();

		foreach (var color in ColorListHelper.UnityColorsAlphabetical())
		{
			if (ColorHelper.TryGetColor(color, out System.Windows.Media.Color clr))
			{
				sp.Children.Add(CreateButton(color, clr));
			}
		}

		var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };

		this.Content = sv;
	}
}
