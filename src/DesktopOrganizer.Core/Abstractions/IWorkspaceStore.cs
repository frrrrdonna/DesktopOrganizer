using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Core.Abstractions;

public interface IWorkspaceStore
{
    Task<Workspace> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(Workspace workspace, CancellationToken cancellationToken = default);
}
