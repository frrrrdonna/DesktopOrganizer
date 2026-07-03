namespace DesktopOrganizer.Core.Models;

public sealed class DesktopItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DisplayName { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public DesktopItemKind Kind { get; set; }
}
