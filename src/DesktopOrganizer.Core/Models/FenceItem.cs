namespace DesktopOrganizer.Core.Models;

public sealed class FenceItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DisplayName { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public FenceItemType Type { get; set; }
}
