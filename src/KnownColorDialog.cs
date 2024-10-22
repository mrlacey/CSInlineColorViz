using System;
using System.Linq;
using System.Windows.Controls;

namespace CsInlineColorViz;

class KnownColorDialog : ColorSelectionDialog
{
	public KnownColorDialog() : base()
	{
		var sp = new StackPanel();

		// TODO: consider separating "colors" and interpreted system values
		foreach (var colorName in Enum.GetValues(typeof(System.Drawing.KnownColor)).Cast<object>().ToList().OrderBy(o => o.ToString()).Select(o => o.ToString()))
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
