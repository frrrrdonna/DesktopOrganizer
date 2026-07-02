using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopOrganizer.App.Services;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.App.ViewModels;

public sealed partial class FenceGroupViewModel : BaseViewModel
{
    private readonly Func<FenceGroupViewModel, Task>? _onRemove;
    private readonly Func<string, Task>? _onOpenItem;
    private readonly Action? _onChanged;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isCollapsed;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _editName = string.Empty;

    public FenceGroupViewModel(
        FenceGroup model,
        Func<FenceGroupViewModel, Task>? onRemove = null,
        Func<string, Task>? onOpenItem = null,
        Action? onChanged = null)
    {
        Model = model;
        _name = model.Name;
        _isCollapsed = model.IsCollapsed;
        _onRemove = onRemove;
        _onOpenItem = onOpenItem;
        _onChanged = onChanged;
        Items = new ObservableCollection<FenceItemViewModel>(
            model.Items.Select(item => CreateItemViewModel(item)));
    }

    public FenceGroup Model { get; }

    public ObservableCollection<FenceItemViewModel> Items { get; }

    partial void OnNameChanged(string value)
    {
        Model.Name = value;
        _onChanged?.Invoke();
    }

    partial void OnIsCollapsedChanged(bool value)
    {
        Model.IsCollapsed = value;
        _onChanged?.Invoke();
    }

    [RelayCommand]
    private void ToggleCollapse()
    {
        IsCollapsed = !IsCollapsed;
    }

    [RelayCommand]
    private void BeginRename()
    {
        EditName = Name;
        IsEditing = true;
    }

    [RelayCommand]
    private void CommitRename()
    {
        var trimmedName = EditName.Trim();

        if (!string.IsNullOrWhiteSpace(trimmedName))
        {
            Name = trimmedName;
        }

        EditName = Name;
        IsEditing = false;
    }

    [RelayCommand]
    private void CancelRename()
    {
        EditName = Name;
        IsEditing = false;
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        if (!UserPrompts.ConfirmDeleteGroup(Name))
        {
            return;
        }

        await (_onRemove?.Invoke(this) ?? Task.CompletedTask);
    }

    public bool AddItem(string path)
    {
        if (!ItemExists(path))
        {
            return false;
        }

        if (Model.Items.Any(item => string.Equals(item.Path, path, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var type = ResolveItemType(path);
        var displayName = Path.GetFileName(path);

        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = path;
        }

        var item = new FenceItem
        {
            DisplayName = displayName,
            Path = path,
            Type = type,
        };

        Model.Items.Add(item);
        Items.Add(CreateItemViewModel(item));
        _onChanged?.Invoke();
        return true;
    }

    private FenceItemViewModel CreateItemViewModel(FenceItem item)
    {
        return new FenceItemViewModel(item, OnItemOpen, OnItemRemove);
    }

    private async void OnItemOpen(FenceItemViewModel itemVm)
    {
        if (_onOpenItem is not null)
        {
            await _onOpenItem.Invoke(itemVm.Path);
        }
    }

    private void OnItemRemove(FenceItemViewModel itemVm)
    {
        if (!UserPrompts.ConfirmDeleteItem(itemVm.DisplayName))
        {
            return;
        }

        Model.Items.Remove(itemVm.Model);
        Items.Remove(itemVm);
        _onChanged?.Invoke();
    }

    private static FenceItemType ResolveItemType(string path)
    {
        if (Path.GetExtension(path).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
        {
            return FenceItemType.Shortcut;
        }

        if (Directory.Exists(path))
        {
            return FenceItemType.Folder;
        }

        return FenceItemType.File;
    }

    private static bool ItemExists(string path)
    {
        return File.Exists(path) || Directory.Exists(path);
    }
}
