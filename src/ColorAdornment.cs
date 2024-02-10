using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CsInlineColorViz
{
    internal sealed class ColorAdornment : Border
    {
        private static readonly SolidColorBrush _borderColor = (SolidColorBrush)Application.Current.Resources[VsBrushes.CaptionTextKey];

        public ColorAdornment(ColorTag tag)
        {
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
                _ => throw new NotImplementedException(),
            };

#pragma warning disable VSTHRD100 // Avoid async void methods - It's from a button click!
        private async void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            // TODO: for preview releases only make this available to sponsors
            //if (await SponsorDetector.IsSponsorAsync())

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
                                    //   TODO: Log any issues to the output pane, not the debug window
                                    System.Diagnostics.Debug.WriteLine($"Failed to find '{find}' on line {ClrTag.LineNumber}.");
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

    class NamedColorDialog : ColorSelectionDialog
    {
        public NamedColorDialog() : base()
        {
            var tabs = new TabControl();

            var tab1 = new TabItem { Header = "  A-Z  " };

            var sp = new StackPanel();

            foreach (var color in ColorHelper.SystemDrawingColorsAlphabetical())
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

            foreach (var color in ColorHelper.SystemDrawingColorsSpectrum())
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
    class KnownColorDialog : ColorSelectionDialog
    {
        public KnownColorDialog() : base()
        {
            var sp = new StackPanel();

            // TODO: consider separating "colors" and interpreted system values
            //foreach (var color in Enum.GetValues(typeof(System.Drawing.KnownColor)))
            foreach (var colorName in Enum.GetValues(typeof(System.Drawing.KnownColor)).Cast<object>().ToList().OrderBy(o => o.ToString()).Select(o => o.ToString()))
            {
                //if (ColorHelper.TryGetColor(color.ToString(), out System.Windows.Media.Color clr))
                if (ColorHelper.TryGetColor(colorName, out System.Windows.Media.Color clr))
                {
                    //sp.Children.Add(CreateButton(color.ToString(), clr));
                    sp.Children.Add(CreateButton(colorName, clr));
                }
            }

            var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };

            this.Content = sv;
        }
    }
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
