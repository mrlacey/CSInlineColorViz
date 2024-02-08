using System;
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

#pragma warning disable VSTHRD100 // Avoid async void methods - It's from a button click!
        private async void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if (e.ClickCount == 2 && ClrTag.PopupType != PopupType.None)
            {
                // TODO: need to tell the dialog which color options to show
                var dlg = new NamedColorDialog();
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

                        // TODO: need to create better undo stack entries
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

    class NamedColorDialog : DialogWindow
    {
        public string SelectedName { get; set; }

        // TODO: Need to differentiate between Color, ConsoleColor and KnownColor
        public NamedColorDialog()
        {
            this.HasMaximizeButton = false;
            this.HasMinimizeButton = false;

            this.Width = 500;
            this.Height = 400;
            this.MinHeight = 200;
            this.MinWidth = 200;

            var tabs = new TabControl();

            var tab1 = new TabItem { Header = "  A-Z  " };

            var sp = new StackPanel();

            //foreach (var color in Enum.GetValues(typeof(System.ConsoleColor)))
            //foreach (var color in Enum.GetValues(typeof(System.Drawing.KnownColor)))
            foreach (var color in ColorHelper.SystemDrawingColorsAlphabetical())
            {
                //if (ColorHelper.TryGetFromName(color.ToString(), out Color clr))
                if (ColorHelper.TryGetFromName(color.Name, out Color clr))
                {
                    var cbtn = new Button();
                    //cbtn.Content = color.ToString();
                    cbtn.Content = color.Name;
                    cbtn.Background = new SolidColorBrush(clr);
                    cbtn.Click += (s, e) => { SelectedName = color.Name; this.DialogResult = true; };
                    sp.Children.Add(cbtn);
                }
            }

            var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };
            tab1.Content = sv;

            var tab2 = new TabItem { Header = "   🌈   " };

            var sp2 = new StackPanel();

            foreach (var color in ColorHelper.SystemDrawingColorsSpectrum())
            {
                if (ColorHelper.TryGetFromName(color.Name, out Color clr))
                {
                    var cbtn = new Button
                    {
                        Content = color.Name,
                        Background = new SolidColorBrush(clr),
                    };
                    cbtn.Click += (s, e) => { SelectedName = color.Name; this.DialogResult = true; };
                    sp2.Children.Add(cbtn);
                }
            }

            var sv2 = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp2 };
            tab2.Content = sv2;

            tabs.Items.Add(tab1);
            tabs.Items.Add(tab2);

            // TODO: need to distinguish the currently selected option
            // - duplicate at the top?
            // - highlight the selected?
            // - scroll into view?

            // TODO: need a cancel button?
            //var btn = new Button { Content = "Cancel", Margin = new Thickness(8) };
            //btn.Click += (s, e) => this.DialogResult = false;
            //sp.Children.Add(btn);

            this.Content = tabs;
        }
    }
}
