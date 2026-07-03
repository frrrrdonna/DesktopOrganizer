using DesktopOrganizer.Core.Models;
using DesktopOrganizer.Infrastructure.Desktop;

namespace DesktopOrganizer.Core.Tests;

public sealed class DesktopClassifierTests
{
    [Fact]
    public void Classify_AssignsStableKeys()
    {
        var items = new List<DesktopItem>
        {
            new() { DisplayName = "Firefox", Path = "firefox.lnk", Kind = DesktopItemKind.Shortcut },
            new() { DisplayName = "Documents", Path = @"C:\Users\Test\Desktop\Documents", Kind = DesktopItemKind.Folder },
            new() { DisplayName = "Readme", Path = "readme.txt", Kind = DesktopItemKind.File },
            new() { DisplayName = "Photo", Path = "photo.png", Kind = DesktopItemKind.File },
            new() { DisplayName = "Unknown", Path = "data.bin", Kind = DesktopItemKind.File },
        };

        var classifier = new DesktopClassifier();
        var baskets = classifier.Classify(items);

        Assert.Equal(5, baskets.Count);

        var apps = baskets.First(b => b.Name == "Applications");
        Assert.Equal("applications", apps.Key);

        var folders = baskets.First(b => b.Name == "Folders");
        Assert.Equal("folders", folders.Key);

        var documents = baskets.First(b => b.Name == "Documents");
        Assert.Equal("documents", documents.Key);

        var media = baskets.First(b => b.Name == "Media");
        Assert.Equal("media", media.Key);

        var other = baskets.First(b => b.Name == "Other");
        Assert.Equal("other", other.Key);
    }

    [Fact]
    public void Classify_FiltersEmptyBaskets()
    {
        var items = new List<DesktopItem>
        {
            new() { DisplayName = "App", Path = "app.lnk", Kind = DesktopItemKind.Shortcut },
        };

        var classifier = new DesktopClassifier();
        var baskets = classifier.Classify(items);

        Assert.Single(baskets);
        Assert.Equal("Applications", baskets[0].Name);
    }

    [Fact]
    public void Classify_RoutesShortcutsToApplications()
    {
        var items = new List<DesktopItem>
        {
            new() { DisplayName = "Chrome", Path = "chrome.lnk", Kind = DesktopItemKind.Shortcut },
        };

        var classifier = new DesktopClassifier();
        var baskets = classifier.Classify(items);

        var apps = baskets.Single();
        Assert.Equal("Applications", apps.Name);
        Assert.Equal(BasketDockEdge.Left, apps.DockEdge);
        Assert.Single(apps.Items);
    }

    [Fact]
    public void Classify_RoutesFoldersToFoldersBasket()
    {
        var items = new List<DesktopItem>
        {
            new() { DisplayName = "MyFolder", Path = @"C:\Users\Test\Desktop\MyFolder", Kind = DesktopItemKind.Folder },
        };

        var classifier = new DesktopClassifier();
        var baskets = classifier.Classify(items);

        var folders = baskets.Single();
        Assert.Equal("Folders", folders.Name);
        Assert.Equal(BasketDockEdge.Right, folders.DockEdge);
    }

    [Fact]
    public void Classify_RoutesUnrecognizedExtensionToOther()
    {
        var items = new List<DesktopItem>
        {
            new() { DisplayName = "Mystery", Path = "mystery.xyz", Kind = DesktopItemKind.File },
        };

        var classifier = new DesktopClassifier();
        var baskets = classifier.Classify(items);

        var other = baskets.Single();
        Assert.Equal("Other", other.Name);
    }
}
