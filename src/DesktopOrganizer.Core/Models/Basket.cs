namespace DesktopOrganizer.Core.Models;

public sealed class Basket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public BasketDockEdge DockEdge { get; set; }

    public bool IsCollapsed { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public BasketDockEdge? SnapEdge { get; set; }

    public List<DesktopItem> Items { get; set; } = [];
}
