using System.Text.Json;
using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Infrastructure.Persistence;

public sealed class JsonWorkspaceStore : IWorkspaceStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    private readonly string _filePath;

    public JsonWorkspaceStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<Workspace> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return CreateDefaultWorkspace();
        }

        await using var stream = File.OpenRead(_filePath);
        var workspace = await JsonSerializer.DeserializeAsync<Workspace>(stream, SerializerOptions, cancellationToken);

        return workspace ?? CreateDefaultWorkspace();
    }

    public async Task SaveAsync(Workspace workspace, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, workspace, SerializerOptions, cancellationToken);
    }

    private static Workspace CreateDefaultWorkspace()
    {
        return new Workspace
        {
            Groups =
            [
                new FenceGroup
                {
                    Name = "Pinned",
                },
            ],
        };
    }
}
