using CommunityToolkit.Mvvm.Input;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.App.ViewModels;

public sealed partial class FenceItemViewModel : BaseViewModel
{
    private readonly Action<FenceItemViewModel>? _onOpen;
    private readonly Action<FenceItemViewModel>? _onRemove;

    public FenceItemViewModel(
        FenceItem model,
        Action<FenceItemViewModel>? onOpen = null,
        Action<FenceItemViewModel>? onRemove = null)
    {
        Model = model;
        _onOpen = onOpen;
        _onRemove = onRemove;
    }

    public FenceItem Model { get; }

    public string DisplayName => Model.DisplayName;

    public string Path => Model.Path;

    [RelayCommand]
    private void Open()
    {
        _onOpen?.Invoke(this);
    }

    [RelayCommand]
    private void Remove()
    {
        _onRemove?.Invoke(this);
    }
}
