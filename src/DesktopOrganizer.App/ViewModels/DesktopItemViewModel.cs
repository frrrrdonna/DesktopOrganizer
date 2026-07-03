using CommunityToolkit.Mvvm.Input;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.App.ViewModels;

public sealed partial class DesktopItemViewModel : BaseViewModel
{
    private readonly Func<string, Task>? _onOpen;

    public DesktopItemViewModel(DesktopItem model, Func<string, Task>? onOpen = null)
    {
        Model = model;
        _onOpen = onOpen;
    }

    public DesktopItem Model { get; }

    public string DisplayName => Model.DisplayName;

    public string Path => Model.Path;

    [RelayCommand]
    private async Task OpenAsync()
    {
        if (_onOpen is not null)
        {
            await _onOpen.Invoke(Path);
        }
    }
}
