namespace DesktopOrganizer.Core.Models;

public sealed class DesktopHostState
{
    public DesktopHostSettings Settings { get; set; } = new();

    public List<PersistedBasketLayout> BasketLayouts { get; set; } = [];
}
