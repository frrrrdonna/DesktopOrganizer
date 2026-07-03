using System.Text.Json;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Infrastructure.Desktop;

public sealed class DesktopSessionSnapshotStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly string _filePath;

    public DesktopSessionSnapshotStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task SaveAsync(DesktopSessionSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, snapshot, SerializerOptions, cancellationToken);
    }

    public async Task<DesktopSessionSnapshot?> TryLoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<DesktopSessionSnapshot>(stream, SerializerOptions, cancellationToken);
    }

    public void Delete()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
