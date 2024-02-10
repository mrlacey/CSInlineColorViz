using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CsInlineColorViz
{
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
        internal static async Task<IEnumerable<EditPoint>> FindMatches(TextDocument textDocument, string patternString)
        {
            var matches = new List<EditPoint>();

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

            return matches;
        }

        internal static async Task<bool> MakeReplacements(TextDocument textDocument, EditPoint startPoint, string patternString, string replacementString)
        {
            var result = false;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
            {
                IFinder finder = GetFinder(patternString, replacementString, textBuffer);
                result = ReplaceAll(textBuffer, finder.FindForReplaceAll(GetSnapshotSpanForExtent(textBuffer.CurrentSnapshot, startPoint, patternString.Length)));
            }

            return result;
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

        private static IFinder GetFinder(string findWhat, string replaceWith, ITextBuffer textBuffer)
        {
            var finderFactory = GetFindService().CreateFinderFactory(findWhat, replaceWith, StandardFindOptions);
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

        private static Span GetSnapshotSpanForExtent(ITextSnapshot textSnapshot, EditPoint startPoint, int length)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var startPosition = GetSnapshotPositionForTextPoint(textSnapshot, startPoint);
            var endPosition = startPosition + length;

            if (startPosition <= endPosition)
            {
                return new Span(startPosition, endPosition - startPosition);
            }
            else
            {
                return new Span(endPosition, startPosition - endPosition);
            }
        }

        private static int GetSnapshotPositionForTextPoint(ITextSnapshot textSnapshot, TextPoint textPoint)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var textSnapshotLine = textSnapshot.GetLineFromLineNumber(textPoint.Line - 1);
            return textSnapshotLine.Start.Position + textPoint.LineCharOffset - 1;
        }


        private static bool ReplaceAll(ITextBuffer textBuffer, IEnumerable<FinderReplacement> replacements)
        {
            var result = false;

            if (replacements.Any())
            {
                using var edit = textBuffer.CreateEdit();
                foreach (var match in replacements)
                {
                    result = true;
                    edit.Replace(match.Match, match.Replace);
                }

                edit.Apply();
            }

            return result;
        }

        private static bool TryGetTextBufferAt(string filePath, out ITextBuffer textBuffer)
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            if (CsInlineColorVizPackage.Instance is not null
                && VsShellUtilities.IsDocumentOpen(
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
}
