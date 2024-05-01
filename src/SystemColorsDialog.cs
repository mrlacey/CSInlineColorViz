using System.Windows.Controls;

namespace CsInlineColorViz
{
	// TODO: if the value returned from here is replacing a string that ends with "Color", this needs to be appended on the replacement too.
	class SystemColorsDialog : ColorSelectionDialog
	{
		public SystemColorsDialog() : base()
		{
			var sp = new StackPanel();

			foreach (var color in ColorHelper.SystemColorsAlphabetically())
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
}
