using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Core.Abstractions;

public interface IDesktopClassifier
{
    IReadOnlyList<Basket> Classify(IReadOnlyList<DesktopItem> items);
}
