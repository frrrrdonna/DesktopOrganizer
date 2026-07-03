using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Core.Abstractions;

public interface IDesktopScanner
{
    Task<IReadOnlyList<DesktopItem>> ScanAsync(CancellationToken cancellationToken = default);
}
