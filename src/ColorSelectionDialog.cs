using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;

namespace CsInlineColorViz;

class ColorSelectionDialog : DialogWindow
{
	public string SelectedName { get; set; }

	public ColorSelectionDialog()
	{
		this.HasMaximizeButton = false;
		this.HasMinimizeButton = false;

		this.Width = 500;
		this.Height = 400;
		this.MinHeight = 200;
		this.MinWidth = 200;
	}

	internal Button CreateButton(string name, System.Windows.Media.Color clr)
	{
		var cbtn = new Button
		{
			Content = name,
			Background = new SolidColorBrush(clr),
			Height = 30,
		};
		cbtn.Click += (s, e) => { SelectedName = name; this.DialogResult = true; };
		return cbtn;
	}

}
