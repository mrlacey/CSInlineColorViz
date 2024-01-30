using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;

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

                    ThreadHelper.JoinableTaskFactory.Run(async () =>
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var dte = (DTE)Package.GetGlobalService(typeof(DTE));

                        if (dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
                        {
                            //if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
                            {
                                //var matches = TextDocumentHelper.FindMatches(txtDoc, ClrTag.Match.Groups);

                                //foreach (var matchPoint in matches)
                                //{
                                //    if (matchPoint.Line == lineNumber)
                                //    {
                                //        if (!TextDocumentHelper.MakeReplacements(txtDoc, matchPoint, find, replace))
                                //        {
                                //            System.Diagnostics.Debug.WriteLine($"Failed to find '{find}' on line {lineNumber}.");
                                //        }

                                //        break;
                                //    }
                                //}

                                //IFinder finder = GetFinder(patternString, replacementString, textBuffer);
                                //result = ReplaceAll(textBuffer, finder.FindForReplaceAll(GetSnapshotSpanForExtent(textBuffer.CurrentSnapshot, startPoint, patternString.Length)));
                            }
                        }
                    });
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

    internal static class TextDocumentHelper
    {
        /// <summary>
        /// The common set of options to be used for find and replace patterns.
        /// </summary>
        internal const FindOptions StandardFindOptions = FindOptions.MatchCase;

        /// <summary>
        /// Finds all matches of the specified pattern within the specified text document.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        /// <param name="patternString">The pattern string.</param>
        /// <returns>The set of matches.</returns>
        internal static IEnumerable<EditPoint> FindMatches(TextDocument textDocument, string patternString)
        {
            var matches = new List<EditPoint>();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, textBuffer);
                    var findMatches = finder.FindAll();
                    foreach (var match in findMatches)
                    {
                        matches.Add(GetEditPointForSnapshotPosition(textDocument, textBuffer.CurrentSnapshot, match.Start));
                    }
                }
            });

            return matches;
        }

        private static IFindService GetFindService()
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var findService = componentModel.GetService<IFindService>();
            return findService;
        }

        private static IFinder GetFinder(string findWhat, ITextBuffer textBuffer)
        {
            var finderFactory = GetFindService().CreateFinderFactory(findWhat, StandardFindOptions);
            return finderFactory.Create(textBuffer.CurrentSnapshot);
        }

        private static EditPoint GetEditPointForSnapshotPosition(TextDocument textDocument, ITextSnapshot textSnapshot, int position)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var editPoint = textDocument.CreateEditPoint();
            var textSnapshotLine = textSnapshot.GetLineFromPosition(position);
            editPoint.MoveToLineAndOffset(textSnapshotLine.LineNumber + 1, position - textSnapshotLine.Start.Position + 1);
            return editPoint;
        }

        private static bool TryGetTextBufferAt(string filePath, out ITextBuffer textBuffer)
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            if (VsShellUtilities.IsDocumentOpen(
              CsInlineColorVizPackage.Instance,
              filePath,
              Guid.Empty,
              out var _,
              out var _,
              out IVsWindowFrame windowFrame))
            {
                IVsTextView view = VsShellUtilities.GetTextView(windowFrame);

                if (view.GetBuffer(out IVsTextLines lines) == 0)
                {
                    if (lines is IVsTextBuffer buffer)
                    {
                        var editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                        textBuffer = editorAdapterFactoryService.GetDataBuffer(buffer);
                        return true;
                    }
                }
            }

            textBuffer = null;
            return false;
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
                    var cbtn = new Button();
                    cbtn.Content = color.Name;
                    cbtn.Background = new SolidColorBrush(clr);
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
