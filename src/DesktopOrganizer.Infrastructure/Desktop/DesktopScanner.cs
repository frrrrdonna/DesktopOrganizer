using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Infrastructure.Desktop;

public sealed class DesktopScanner : IDesktopScanner
{
    public Task<IReadOnlyList<DesktopItem>> ScanAsync(CancellationToken cancellationToken = default)
    {
        var items = new List<DesktopItem>();
        var roots = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
        };

        foreach (var root in roots.Where(static path => !string.IsNullOrWhiteSpace(path)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!Directory.Exists(root))
            {
                continue;
            }

            foreach (var entryPath in Directory.EnumerateFileSystemEntries(root))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var name = Path.GetFileName(entryPath);

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                items.Add(new DesktopItem
                {
                    DisplayName = name,
                    Path = entryPath,
                    Kind = ResolveKind(entryPath),
                });
            }
        }

        return Task.FromResult<IReadOnlyList<DesktopItem>>(
            items
                .GroupBy(static item => item.Path, StringComparer.OrdinalIgnoreCase)
                .Select(static group => group.First())
                .OrderBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList());
    }

    private static DesktopItemKind ResolveKind(string path)
    {
        if (Directory.Exists(path))
        {
            return DesktopItemKind.Folder;
        }

        var extension = Path.GetExtension(path);

        if (extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase) ||
            extension.Equals(".url", StringComparison.OrdinalIgnoreCase))
        {
            return DesktopItemKind.Shortcut;
        }

        return DesktopItemKind.File;
    }
}
