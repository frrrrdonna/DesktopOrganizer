using System.Diagnostics;
using DesktopOrganizer.Core.Abstractions;

namespace DesktopOrganizer.Infrastructure.Shell;

public sealed class ShellLauncher : ILauncher
{
    public Task LaunchAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var startInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true,
        };

        Process.Start(startInfo);
        return Task.CompletedTask;
    }
}
