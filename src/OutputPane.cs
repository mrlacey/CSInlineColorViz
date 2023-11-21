using System;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace CsInlineColorViz
{
    public class OutputPane
    {
        private static Guid csicvPaneGuid = new Guid("D5024E0F-63F2-431D-A68B-924CE419B8D3");

        private static OutputPane instance;

        private readonly IVsOutputWindowPane pane;

        private OutputPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outWindow
             && (ErrorHandler.Failed(outWindow.GetPane(ref csicvPaneGuid, out pane)) || pane == null))
            {
                if (ErrorHandler.Failed(outWindow.CreatePane(ref csicvPaneGuid, Vsix.Name, 1, 0)))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create the Output window pane.");
                    return;
                }

                if (ErrorHandler.Failed(outWindow.GetPane(ref csicvPaneGuid, out pane)) || (pane == null))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get access to the Output window pane.");
                }
            }
        }

        public static OutputPane Instance => instance ??= new OutputPane();

        public async Task ActivateAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            pane?.Activate();
        }

        public async Task WriteAsync(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            _ = (pane?.OutputStringThreadSafe($"{message}{Environment.NewLine}"));
        }
    }
}
