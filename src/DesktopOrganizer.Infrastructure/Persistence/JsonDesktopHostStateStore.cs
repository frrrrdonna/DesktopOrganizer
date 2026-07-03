using System.IO;
using System.Text.Json;
using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Infrastructure.Persistence;

public sealed class JsonDesktopHostStateStore : IDesktopHostStateStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    private readonly string _filePath;

    public JsonDesktopHostStateStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<DesktopHostState> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return new DesktopHostState();
        }

        await using var stream = File.OpenRead(_filePath);
        var state = await JsonSerializer.DeserializeAsync<DesktopHostState>(stream, JsonOptions, cancellationToken);
        return state ?? new DesktopHostState();
    }

    public async Task SaveAsync(DesktopHostState state, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, state, JsonOptions, cancellationToken);
    }
}
