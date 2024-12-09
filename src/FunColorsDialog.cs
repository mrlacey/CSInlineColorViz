using System.Windows.Controls;

namespace CsInlineColorViz;

class FunColorsDialog : ColorSelectionDialog
{
	public FunColorsDialog() : base()
	{
		var sp = new StackPanel();

		foreach (var (name, color) in ColorListHelper.FunColors())
		{
			sp.Children.Add(CreateButton(name, color));
		}

		var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };

		this.Content = sv;
	}
}
