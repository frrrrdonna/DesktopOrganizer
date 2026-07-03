using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DesktopOrganizer.App.Services;
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

    [ObservableProperty]
    private string _selectedLanguage;

    private readonly Action? _onSettingsChanged;

    public SettingsViewModel(DesktopHostSettings settings, Action? onSettingsChanged = null)
    {
        _onSettingsChanged = onSettingsChanged;
        Settings = settings;
        _replaceDesktopOnStartup = settings.ReplaceDesktopOnStartup;
        _hideDesktopIcons = settings.HideDesktopIcons;
        _autoGenerateBaskets = settings.AutoGenerateBaskets;
        _restoreDesktopOnExit = settings.RestoreDesktopOnExit;
        _selectedLanguage = settings.Language;
    }

    public DesktopHostSettings Settings { get; }

    public LocalizationService Loc => LocalizationService.Instance;

    public ObservableCollection<string> SupportedLanguages { get; } =
        new(LocalizationService.SupportedLanguages);

    partial void OnReplaceDesktopOnStartupChanged(bool value)
    {
        Settings.ReplaceDesktopOnStartup = value;
        _onSettingsChanged?.Invoke();
    }

    partial void OnHideDesktopIconsChanged(bool value)
    {
        Settings.HideDesktopIcons = value;
        _onSettingsChanged?.Invoke();
    }

    partial void OnAutoGenerateBasketsChanged(bool value)
    {
        Settings.AutoGenerateBaskets = value;
        _onSettingsChanged?.Invoke();
    }

    partial void OnRestoreDesktopOnExitChanged(bool value)
    {
        Settings.RestoreDesktopOnExit = value;
        _onSettingsChanged?.Invoke();
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        Settings.Language = value;
        LocalizationService.SetLanguage(value);
        _onSettingsChanged?.Invoke();
    }
}
