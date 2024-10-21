using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using static Microsoft.VisualStudio.VSConstants;
using Task = System.Threading.Tasks.Task;

namespace CsInlineColorViz;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(CsInlineColorVizPackage.PackageGuidString)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideAutoLoad(UICONTEXT.CSharpProject_string, PackageAutoLoadFlags.BackgroundLoad)]
public sealed class CsInlineColorVizPackage : AsyncPackage
{
	public const string PackageGuidString = "df7e1d03-27f6-44ba-875a-c9f732e6ad65";

	public static AsyncPackage Instance { get; private set; }

	protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
	{
		Instance = this;

		await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

		await SponsorRequestHelper.CheckIfNeedToShowAsync();

		await base.InitializeAsync(cancellationToken, progress);
	}

	internal static async Task EnsureInstanceLoadedAsync()
	{
		await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

		if (CsInlineColorVizPackage.Instance == null)
		{
			// Try and force load the project if it hasn't already loaded
			// so can access the configured options.
			if (ServiceProvider.GlobalProvider.GetService(typeof(SVsShell)) is IVsShell shell)
			{
				Guid PackageToBeLoadedGuid = new Guid(CsInlineColorVizPackage.PackageGuidString);
				shell.LoadPackage(ref PackageToBeLoadedGuid, out _);
			}
		}
	}
}
