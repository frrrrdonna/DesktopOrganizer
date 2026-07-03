using DesktopOrganizer.Core.Models;
using DesktopOrganizer.Infrastructure.Persistence;

namespace DesktopOrganizer.Core.Tests;

public sealed class JsonDesktopHostStateStoreTests : IDisposable
{
    private readonly string _tempFilePath;

    public JsonDesktopHostStateStoreTests()
    {
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"desktop-host-state-test-{Guid.NewGuid()}.json");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }

    [Fact]
    public async Task LoadAsync_ReturnsDefault_WhenFileMissing()
    {
        var store = new JsonDesktopHostStateStore(_tempFilePath);
        var state = await store.LoadAsync();

        Assert.NotNull(state);
        Assert.NotNull(state.Settings);
        Assert.True(state.Settings.AutoGenerateBaskets);
        Assert.True(state.Settings.RestoreDesktopOnExit);
        Assert.Empty(state.BasketLayouts);
    }

    [Fact]
    public async Task SaveAndLoad_RoundTrips_SettingsAndLayouts()
    {
        var original = new DesktopHostState
        {
            Settings = new DesktopHostSettings
            {
                ReplaceDesktopOnStartup = false,
                HideDesktopIcons = true,
                AutoGenerateBaskets = false,
                RestoreDesktopOnExit = true,
                Language = "zh-CN",
            },
            BasketLayouts = new List<PersistedBasketLayout>
            {
                new()
                {
                    BasketKey = "applications",
                    X = 50,
                    Y = 120,
                    SnapEdge = BasketDockEdge.Left,
                    IsCollapsed = true,
                },
                new()
                {
                    BasketKey = "folders",
                    X = 1500,
                    Y = 80,
                    SnapEdge = BasketDockEdge.Right,
                    IsCollapsed = false,
                },
            },
        };

        var store = new JsonDesktopHostStateStore(_tempFilePath);
        await store.SaveAsync(original);

        var loaded = await store.LoadAsync();

        Assert.NotNull(loaded);
        Assert.False(loaded.Settings.ReplaceDesktopOnStartup);
        Assert.True(loaded.Settings.HideDesktopIcons);
        Assert.False(loaded.Settings.AutoGenerateBaskets);
        Assert.Equal("zh-CN", loaded.Settings.Language);

        Assert.Equal(2, loaded.BasketLayouts.Count);

        var apps = loaded.BasketLayouts[0];
        Assert.Equal("applications", apps.BasketKey);
        Assert.Equal(50, apps.X);
        Assert.Equal(120, apps.Y);
        Assert.Equal(BasketDockEdge.Left, apps.SnapEdge);
        Assert.True(apps.IsCollapsed);

        var folders = loaded.BasketLayouts[1];
        Assert.Equal("folders", folders.BasketKey);
        Assert.Equal(1500, folders.X);
    }
}
