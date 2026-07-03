using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopOrganizer.App.Services;
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
    private readonly IDesktopHostStateStore _hostStateStore;

    private int _leftCount;
    private int _rightCount;
    private int _topCount;
    private int _bottomCount;
    private int _lastItemCount;
    private int _lastBasketCount;
    private bool _isReady;
    private CancellationTokenSource? _saveDebounceCts;

    [ObservableProperty]
    private string _statusText = LocalizationService.Get("Host.Status.Scanning");

    public DesktopHostViewModel(
        IDesktopScanner desktopScanner,
        IDesktopClassifier desktopClassifier,
        ILauncher launcher,
        SettingsViewModel settingsViewModel,
        Action openSettings,
        IDesktopHostStateStore hostStateStore)
    {
        _desktopScanner = desktopScanner;
        _desktopClassifier = desktopClassifier;
        _launcher = launcher;
        Settings = settingsViewModel;
        _openSettings = openSettings;
        _hostStateStore = hostStateStore;
        HostState = new DesktopHostState { Settings = settingsViewModel.Settings };

        LocalizationService.Instance.PropertyChanged += (_, _) => RefreshStatusText();
    }

    public SettingsViewModel Settings { get; }

    public DesktopHostState HostState { get; }

    public LocalizationService Loc => LocalizationService.Instance;

    public ObservableCollection<BasketViewModel> AllBaskets { get; } = [];

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _isReady = false;
        RefreshStatusText();
        var items = await _desktopScanner.ScanAsync(cancellationToken);
        var baskets = _desktopClassifier.Classify(items);

        _lastItemCount = items.Count;
        _lastBasketCount = baskets.Count;

        AllBaskets.Clear();
        _leftCount = 0;
        _rightCount = 0;
        _topCount = 0;
        _bottomCount = 0;

        var screenW = SystemParameters.VirtualScreenWidth;
        var screenH = SystemParameters.VirtualScreenHeight;
        var savedLayouts = HostState.BasketLayouts;

        foreach (var basket in baskets)
        {
            var vm = CreateBasketViewModel(basket);
            var saved = savedLayouts.Find(l => l.BasketKey == basket.Key);

            if (saved is not null)
            {
                vm.X = CoordinateClamper.ClampX(saved.X, screenW);
                vm.Y = CoordinateClamper.ClampY(saved.Y, screenH);
                vm.SnapEdge = saved.SnapEdge;
                vm.IsCollapsed = saved.IsCollapsed;
            }
            else
            {
                PlaceBasket(vm, basket.DockEdge, screenW, screenH);
            }

            AllBaskets.Add(vm);
        }

        // Remove orphan layouts for baskets that no longer exist
        var currentKeys = baskets.Select(b => b.Key).ToHashSet();
        HostState.BasketLayouts.RemoveAll(l => !currentKeys.Contains(l.BasketKey));

        // Persist initial state (ensures new baskets are saved)
        await SaveHostStateAsync();

        _isReady = true;
        RefreshStatusText();
    }

    public async Task SaveHostStateAsync()
    {
        // Sync current VM state into HostState
        HostState.BasketLayouts.Clear();
        foreach (var vm in AllBaskets)
        {
            HostState.BasketLayouts.Add(new PersistedBasketLayout
            {
                BasketKey = vm.Model.Key,
                X = vm.X,
                Y = vm.Y,
                SnapEdge = vm.SnapEdge,
                IsCollapsed = vm.IsCollapsed,
            });
        }

        await _hostStateStore.SaveAsync(HostState);
    }

    public void DebounceSave()
    {
        _saveDebounceCts?.Cancel();
        _saveDebounceCts = new CancellationTokenSource();
        _ = SaveAfterDelayAsync(_saveDebounceCts.Token);
    }

    private async Task SaveAfterDelayAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(400, cancellationToken);
            await SaveHostStateAsync();
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer DebounceSave call
        }
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
        return new BasketViewModel(basket, OpenItemAsync, DebounceSave);
    }

    private async Task OpenItemAsync(string path)
    {
        await _launcher.LaunchAsync(path);
    }

    private void RefreshStatusText()
    {
        if (_isReady)
        {
            StatusText = LocalizationService.Format("Host.Status.Ready", _lastItemCount, _lastBasketCount);
        }
        else
        {
            StatusText = LocalizationService.Get("Host.Status.Scanning");
        }
    }
}
