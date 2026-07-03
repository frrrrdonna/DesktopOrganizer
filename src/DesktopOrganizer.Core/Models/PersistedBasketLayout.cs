namespace DesktopOrganizer.Core.Models;

public sealed class PersistedBasketLayout
{
    public string BasketKey { get; set; } = string.Empty;

    public double X { get; set; }

    public double Y { get; set; }

    public BasketDockEdge? SnapEdge { get; set; }

    public bool IsCollapsed { get; set; }
}
