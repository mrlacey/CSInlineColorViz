using System;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace CsInlineColorViz
{
    public class GeneralOutputPane
    {
        private static GeneralOutputPane instance;

        private readonly IVsOutputWindowPane generalPane;

        private GeneralOutputPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;

            if (ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outWindow
             && (ErrorHandler.Failed(outWindow.GetPane(ref generalPaneGuid, out generalPane)) || generalPane == null))
            {
                if (ErrorHandler.Failed(outWindow.CreatePane(ref generalPaneGuid, "General", 1, 0)))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create the Output window pane.");
                    return;
                }

                if (ErrorHandler.Failed(outWindow.GetPane(ref generalPaneGuid, out generalPane)) || (generalPane == null))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get access to the Output window pane.");
                }
            }
        }

        public static GeneralOutputPane Instance => instance ??= new GeneralOutputPane();

        public async Task ActivateAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            generalPane?.Activate();
        }

        public async Task WriteAsync(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            _ = (generalPane?.OutputStringThreadSafe($"{message}{Environment.NewLine}"));
        }
    }
}
