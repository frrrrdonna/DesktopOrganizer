namespace DesktopOrganizer.Core.Models;

public sealed class DesktopSessionSnapshot
{
    public bool DesktopIconsWereVisible { get; set; }

    public bool DesktopIconsAreHidden { get; set; }

    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}
