namespace DesktopOrganizer.Core.Abstractions;

public interface ILauncher
{
    Task LaunchAsync(string path, CancellationToken cancellationToken = default);
}
