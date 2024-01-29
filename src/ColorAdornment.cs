using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && ClrTag.PopupType != PopupType.None)
            {
                // TODO: need to tell the dialog which color options to show
                var dlg = new NamedColorDialog();
                var dlgResult = dlg.ShowModal();

                if (dlgResult == true)
                {
                    // TODO: update the color in the source from dlg.SelectedName
                    // Need to edit the current document: replace the current name from the match, with the new name.
                    // TODO: investigate if need to pass the document reference to the adornment so can get it here, or if need to look at a different approach.
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

        public NamedColorDialog()
        {
            this.HasMaximizeButton = false;
            this.HasMinimizeButton = false;

            // TODO: review resizability
            this.Width = 500;
            this.Height = 400;

            var sp = new StackPanel();

            // TODO: Need to differentiate between Color and KnownColor

            // TODO: add option to sort colors alphabetically or by hue
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

            // TODO: need to distinguish the currently selected option
            // - duplicate at the top?
            // - highlight the selected?

            // TODO: need a better cancel button
            var btn = new Button { Content = "Cancel", Margin = new Thickness(8) };
            btn.Click += (s, e) => this.DialogResult = false;
            sp.Children.Add(btn);

            var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = sp };
            // Scroll selected into view?

            this.Content = sv;
        }
    }
}
