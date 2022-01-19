using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using static Microsoft.VisualStudio.VSConstants;
using Task = System.Threading.Tasks.Task;

namespace CsInlineColorViz
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CsInlineColorVizPackage.PackageGuidString)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideAutoLoad(UICONTEXT.CSharpProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class CsInlineColorVizPackage: AsyncPackage
    {
        public const string PackageGuidString = "df7e1d03-27f6-44ba-875a-c9f732e6ad65";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await SponsorRequestHelper.CheckIfNeedToShowAsync();

            await base.InitializeAsync(cancellationToken, progress);
        }
    }
}
