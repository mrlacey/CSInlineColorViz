using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CsInlineColorViz;

internal sealed class ColorAdornment : Border
{
	private static readonly SolidColorBrush _borderColor = (SolidColorBrush)Application.Current.Resources[VsBrushes.CaptionTextKey];

	public ColorAdornment(ColorTag tag)
	{
		ThreadHelper.ThrowIfNotOnUIThread();

		ClrTag = tag;

		Padding = new Thickness(0);
		BorderThickness = new Thickness(1);
		BorderBrush = _borderColor;
		Height = GetFontSize() + 2; ;
		Width = Height;
		VerticalAlignment = System.Windows.VerticalAlignment.Center;
		Margin = new System.Windows.Thickness(0, 0, 2, 3);
		SetBackground();

		this.MouseLeftButtonDown += OnMouseLeftButtonDown;
	}

	private ColorSelectionDialog CreateDialogForPopupType(PopupType popupType)
		=> popupType switch
		{
			PopupType.NamedColors => new NamedColorDialog(),
			PopupType.ConsoleColors => new ConsoleColorDialog(),
			PopupType.KnownColors => new KnownColorDialog(),
			PopupType.SystemColors => new SystemColorsDialog(),
			PopupType.FunColors => new FunColorsDialog(),
			_ => throw new NotImplementedException(),
		};

#pragma warning disable VSTHRD100 // Avoid async void methods - It's from a button click!
	private async void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
	{
		if (e.ClickCount == 2 && ClrTag.PopupType != PopupType.None)
		{
			var dlg = CreateDialogForPopupType(ClrTag.PopupType);
			var dlgResult = dlg.ShowModal();

			if (dlgResult == true)
			{
				// We always should be on the UI thread here as the user just clicked on the dialog.
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				await CsInlineColorVizPackage.EnsureInstanceLoadedAsync();

				var dte = (DTE)Package.GetGlobalService(typeof(DTE));

				if (dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
				{
					var find = ClrTag.Match.Groups[0].Value;
					var replace = $"{ClrTag.Match.Groups[1].Value}{ClrTag.Match.Groups[2].Value}{dlg.SelectedName}";

					// System.Windows.Colors end with the suffix, but System.Drawing.Colors do not
					// The dialog doesn't include the suffix so add it if needed
					if (find.EndsWith("Color") && !replace.EndsWith("Color"))
					{
						replace += "Color";
					}

					var matches = await TextDocumentHelper.FindMatches(txtDoc, find);

					if (!dte.UndoContext.IsOpen)
					{
						dte.UndoContext.Open($"Changing color to: {replace}");
					}

					foreach (var matchPoint in matches)
					{
						// TODO: Need to account for the line number in the tag not having been updated even if the line has changed
						// Add one to the line number as the EditPoint is 1-based
						if (matchPoint.Line == ClrTag.LineNumber + 1)
						{
							var replacementMade = await TextDocumentHelper.MakeReplacements(txtDoc, matchPoint, find, replace);

							if (!replacementMade)
							{
								System.Diagnostics.Debug.WriteLine($"Failed to find '{find}' on line {ClrTag.LineNumber}.");
								await OutputPane.Instance.WriteAsync($"Failed to find '{find}' on line {ClrTag.LineNumber}.");
							}
						}
					}

					dte.UndoContext.Close();
				}
			}
		}
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

	private static int GetFontSize()
	{
		ThreadHelper.ThrowIfNotOnUIThread();

		try
		{
			IVsFontAndColorStorage storage = (IVsFontAndColorStorage)Package.GetGlobalService(typeof(IVsFontAndColorStorage));
			Guid guid = new("A27B4E24-A735-4d1d-B8E7-9716E1E3D8E0");
			if (storage != null && storage.OpenCategory(ref guid, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS)) == VSConstants.S_OK)
			{
				LOGFONTW[] Fnt = new LOGFONTW[] { new() };
				FontInfo[] Info = new FontInfo[] { new() };
				storage.GetFont(Fnt, Info);
				return Info[0].wPointSize;
			}

		}
		catch { }

		return 12;
	}
}
