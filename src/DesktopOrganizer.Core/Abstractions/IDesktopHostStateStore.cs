using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Core.Abstractions;

public interface IDesktopHostStateStore
{
    Task<DesktopHostState> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(DesktopHostState state, CancellationToken cancellationToken = default);
}
