using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Infrastructure.Desktop;

public sealed class DesktopClassifier : IDesktopClassifier
{
    private static readonly HashSet<string> DocumentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".ppt", ".pptx", ".md", ".rtf",
    };

    private static readonly HashSet<string> MediaExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".mp3", ".wav", ".mp4", ".mov",
    };

    public IReadOnlyList<Basket> Classify(IReadOnlyList<DesktopItem> items)
    {
        var apps = CreateBasket("Applications", BasketDockEdge.Left);
        var folders = CreateBasket("Folders", BasketDockEdge.Right);
        var documents = CreateBasket("Documents", BasketDockEdge.Left);
        var media = CreateBasket("Media", BasketDockEdge.Bottom);
        var other = CreateBasket("Other", BasketDockEdge.Right);

        foreach (var item in items)
        {
            SelectBasket(item, apps, folders, documents, media, other).Items.Add(item);
        }

        return new[] { apps, documents, folders, media, other }
            .Where(static basket => basket.Items.Count > 0)
            .ToList();
    }

    private static Basket CreateBasket(string name, BasketDockEdge dockEdge)
    {
        return new Basket
        {
            Name = name,
            DockEdge = dockEdge,
        };
    }

    private static Basket SelectBasket(
        DesktopItem item,
        Basket apps,
        Basket folders,
        Basket documents,
        Basket media,
        Basket other)
    {
        if (item.Kind == DesktopItemKind.Shortcut)
        {
            return apps;
        }

        if (item.Kind == DesktopItemKind.Folder)
        {
            return folders;
        }

        var extension = Path.GetExtension(item.Path);

        if (DocumentExtensions.Contains(extension))
        {
            return documents;
        }

        if (MediaExtensions.Contains(extension))
        {
            return media;
        }

        return other;
    }
}
