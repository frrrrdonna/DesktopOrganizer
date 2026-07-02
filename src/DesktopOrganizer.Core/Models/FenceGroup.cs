namespace DesktopOrganizer.Core.Models;

public sealed class FenceGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "New Group";

    public double X { get; set; }

    public double Y { get; set; }

    public double Width { get; set; } = 280;

    public double Height { get; set; } = 360;

    public bool IsCollapsed { get; set; }

    public FolderPortal? Portal { get; set; }

    public List<FenceItem> Items { get; set; } = [];
}
