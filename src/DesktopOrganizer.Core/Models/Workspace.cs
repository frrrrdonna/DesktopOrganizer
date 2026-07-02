namespace DesktopOrganizer.Core.Models;

public sealed class Workspace
{
    public string Name { get; set; } = "Default";

    public List<FenceGroup> Groups { get; set; } = [];
}
