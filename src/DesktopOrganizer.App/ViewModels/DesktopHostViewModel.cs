using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.App.ViewModels;

public sealed partial class DesktopHostViewModel : BaseViewModel
{
    private const double PanelWidth = 300;
    private const double BasketSpacing = 340;

    private readonly IDesktopScanner _desktopScanner;
    private readonly IDesktopClassifier _desktopClassifier;
    private readonly ILauncher _launcher;
    private readonly Action _openSettings;

    private int _leftCount;
    private int _rightCount;
    private int _topCount;
    private int _bottomCount;

    [ObservableProperty]
    private string _statusText = "Preparing transparent desktop layer...";

    public DesktopHostViewModel(
        IDesktopScanner desktopScanner,
        IDesktopClassifier desktopClassifier,
        ILauncher launcher,
        SettingsViewModel settingsViewModel,
        Action openSettings)
    {
        _desktopScanner = desktopScanner;
        _desktopClassifier = desktopClassifier;
        _launcher = launcher;
        Settings = settingsViewModel;
        _openSettings = openSettings;
    }

    public SettingsViewModel Settings { get; }

    public ObservableCollection<BasketViewModel> AllBaskets { get; } = [];

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        StatusText = "Scanning desktop items...";
        var items = await _desktopScanner.ScanAsync(cancellationToken);
        var baskets = _desktopClassifier.Classify(items);

        AllBaskets.Clear();
        _leftCount = 0;
        _rightCount = 0;
        _topCount = 0;
        _bottomCount = 0;

        var screenW = SystemParameters.VirtualScreenWidth;
        var screenH = SystemParameters.VirtualScreenHeight;

        foreach (var basket in baskets)
        {
            var vm = CreateBasketViewModel(basket);
            PlaceBasket(vm, basket.DockEdge, screenW, screenH);
            AllBaskets.Add(vm);
        }

        StatusText = $"Desktop host active — {items.Count} item(s) in {baskets.Count} basket(s). Drag to move, scroll to browse. Wallpaper preserved.";
    }

    private void PlaceBasket(BasketViewModel vm, BasketDockEdge dockEdge, double screenW, double screenH)
    {
        double x, y;

        switch (dockEdge)
        {
            case BasketDockEdge.Left:
                x = 16;
                y = 120 + _leftCount * BasketSpacing;
                _leftCount++;
                break;
            case BasketDockEdge.Right:
                x = screenW - PanelWidth - 16;
                y = 120 + _rightCount * BasketSpacing;
                _rightCount++;
                break;
            case BasketDockEdge.Top:
                x = Math.Max(16, (screenW - PanelWidth) / 2) + _topCount * 40;
                y = 80;
                _topCount++;
                break;
            default:
                x = 300 + _bottomCount * BasketSpacing;
                y = screenH - 440;
                _bottomCount++;
                break;
        }

        vm.X = x;
        vm.Y = y;
        vm.SnapEdge = dockEdge;
    }

    [RelayCommand]
    private void OpenSettings()
    {
        _openSettings();
    }

    [RelayCommand]
    private async Task RefreshDesktopAsync()
    {
        await InitializeAsync();
    }

    private BasketViewModel CreateBasketViewModel(Basket basket)
    {
        return new BasketViewModel(basket, OpenItemAsync);
    }

    private async Task OpenItemAsync(string path)
    {
        await _launcher.LaunchAsync(path);
    }
}
