using System.IO;
using System.Windows;
using DesktopOrganizer.App.Services;
using DesktopOrganizer.App.ViewModels;
using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.Core.Models;
using DesktopOrganizer.Infrastructure.Desktop;
using DesktopOrganizer.Infrastructure.Shell;

namespace DesktopOrganizer.App;

public partial class App : System.Windows.Application
{
    private IDesktopSurfaceController? _desktopSurfaceController;
    private DesktopHostSettings? _desktopHostSettings;
    private DesktopSessionSnapshot? _sessionSnapshot;
    private DesktopSessionSnapshotStore? _snapshotStore;
    private SettingsWindow? _settingsWindow;
    private DesktopHostViewModel? _desktopHostViewModel;
    private DesktopHostWindow? _hostWindow;
    private TrayIcon? _trayIcon;

    private static string SnapshotDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DesktopOrganizer");

    private static string SnapshotPath =>
        Path.Combine(SnapshotDirectory, "session-snapshot.json");

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _snapshotStore = new DesktopSessionSnapshotStore(SnapshotPath);

        // Recover from a previous abnormal exit: if a stale snapshot exists,
        // desktop icons may still be hidden — restore them.
        await RecoverFromPreviousSessionAsync();

        var launcher = new ShellLauncher();
        var desktopScanner = new DesktopScanner();
        var desktopClassifier = new DesktopClassifier();
        _desktopSurfaceController = new DesktopSurfaceController();
        _desktopHostSettings = new DesktopHostSettings();
        var settingsViewModel = new SettingsViewModel(_desktopHostSettings);
        _desktopHostViewModel = new DesktopHostViewModel(
            desktopScanner,
            desktopClassifier,
            launcher,
            settingsViewModel,
            OpenSettingsWindow);

        await _desktopHostViewModel.InitializeAsync();

        // Capture desktop icon state before any changes
        _sessionSnapshot = new DesktopSessionSnapshot
        {
            DesktopIconsWereVisible = _desktopSurfaceController.AreDesktopIconsVisible(),
        };

        if (_desktopHostSettings.HideDesktopIcons && _sessionSnapshot.DesktopIconsWereVisible)
        {
            _desktopSurfaceController.HideDesktopIcons();
            _sessionSnapshot.DesktopIconsAreHidden = true;
        }

        // Persist snapshot so abnormal exit can be recovered
        await _snapshotStore.SaveAsync(_sessionSnapshot);

        _hostWindow = new DesktopHostWindow(_desktopHostViewModel, _desktopSurfaceController);
        _hostWindow.Show();

        CreateTrayIcon();
    }

    private void CreateTrayIcon()
    {
        _trayIcon = new TrayIcon();
        _trayIcon.AddMenuItem("Show Desktop Organizer", ShowHostWindow, isBold: true);
        _trayIcon.AddSeparator();
        _trayIcon.AddMenuItem("Settings", OpenSettingsWindow);
        _trayIcon.AddMenuItem("Refresh Baskets", () => _desktopHostViewModel?.RefreshDesktopCommand.Execute(null));
        _trayIcon.AddSeparator();
        _trayIcon.AddMenuItem("Exit", ShutdownGracefully);
    }

    private void ShowHostWindow()
    {
        if (_hostWindow is null) return;

        _hostWindow.Show();
        _hostWindow.WindowState = WindowState.Normal;
        _hostWindow.Activate();
    }

    private void ShutdownGracefully()
    {
        _trayIcon?.Dispose();
        _trayIcon = null;
        Shutdown();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_desktopSurfaceController is not null &&
            _desktopHostSettings?.RestoreDesktopOnExit == true &&
            _sessionSnapshot?.DesktopIconsAreHidden == true)
        {
            _desktopSurfaceController.ShowDesktopIcons();
        }

        // Clean up snapshot on normal exit
        _snapshotStore?.Delete();

        _trayIcon?.Dispose();

        base.OnExit(e);
    }

    private async Task RecoverFromPreviousSessionAsync()
    {
        if (_snapshotStore is null)
        {
            return;
        }

        var staleSnapshot = await _snapshotStore.TryLoadAsync();

        if (staleSnapshot?.DesktopIconsAreHidden == true)
        {
            var controller = new DesktopSurfaceController();
            controller.ShowDesktopIcons();
        }

        _snapshotStore.Delete();
    }

    private void OpenSettingsWindow()
    {
        if (_settingsWindow is not null)
        {
            _settingsWindow.Activate();
            return;
        }

        if (Current.Windows.OfType<DesktopHostWindow>().FirstOrDefault() is not { } hostWindow ||
            hostWindow.DataContext is not DesktopHostViewModel desktopHostViewModel)
        {
            return;
        }

        _settingsWindow = new SettingsWindow(desktopHostViewModel.Settings)
        {
            Owner = hostWindow,
        };
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
    }
}
