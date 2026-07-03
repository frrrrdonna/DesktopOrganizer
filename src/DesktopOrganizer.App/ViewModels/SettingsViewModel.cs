using CommunityToolkit.Mvvm.ComponentModel;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.App.ViewModels;

public sealed partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _replaceDesktopOnStartup;

    [ObservableProperty]
    private bool _hideDesktopIcons;

    [ObservableProperty]
    private bool _autoGenerateBaskets;

    [ObservableProperty]
    private bool _restoreDesktopOnExit;

    public SettingsViewModel(DesktopHostSettings settings)
    {
        Settings = settings;
        _replaceDesktopOnStartup = settings.ReplaceDesktopOnStartup;
        _hideDesktopIcons = settings.HideDesktopIcons;
        _autoGenerateBaskets = settings.AutoGenerateBaskets;
        _restoreDesktopOnExit = settings.RestoreDesktopOnExit;
    }

    public DesktopHostSettings Settings { get; }

    partial void OnReplaceDesktopOnStartupChanged(bool value)
    {
        Settings.ReplaceDesktopOnStartup = value;
    }

    partial void OnHideDesktopIconsChanged(bool value)
    {
        Settings.HideDesktopIcons = value;
    }

    partial void OnAutoGenerateBasketsChanged(bool value)
    {
        Settings.AutoGenerateBaskets = value;
    }

    partial void OnRestoreDesktopOnExitChanged(bool value)
    {
        Settings.RestoreDesktopOnExit = value;
    }
}
