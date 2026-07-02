using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.App.ViewModels;

public sealed partial class MainWindowViewModel : BaseViewModel
{
    private readonly IWorkspaceStore _workspaceStore;
    private readonly ILauncher _launcher;
    private CancellationTokenSource? _saveDebounceCts;

    public MainWindowViewModel(IWorkspaceStore workspaceStore, ILauncher launcher)
    {
        _workspaceStore = workspaceStore;
        _launcher = launcher;
    }

    public ObservableCollection<FenceGroupViewModel> Groups { get; } = [];

    public Workspace? Workspace { get; private set; }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Workspace = await _workspaceStore.LoadAsync();

        Groups.Clear();

        foreach (var group in Workspace.Groups)
        {
            Groups.Add(CreateGroupViewModel(group));
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Workspace is null)
        {
            return;
        }

        await _workspaceStore.SaveAsync(Workspace);
    }

    [RelayCommand]
    private async Task CreateGroupAsync()
    {
        if (Workspace is null)
        {
            return;
        }

        var group = new FenceGroup { Name = "New Group" };
        Workspace.Groups.Add(group);
        Groups.Add(CreateGroupViewModel(group));
        await SaveAsync();
    }

    private async Task RemoveGroupAsync(FenceGroupViewModel vm)
    {
        if (Workspace is null)
        {
            return;
        }

        Workspace.Groups.Remove(vm.Model);
        Groups.Remove(vm);
        await SaveAsync();
    }

    private async Task OpenItemAsync(string path)
    {
        await _launcher.LaunchAsync(path);
    }

    private void OnGroupChanged()
    {
        DebounceSave();
    }

    private FenceGroupViewModel CreateGroupViewModel(FenceGroup group)
    {
        return new FenceGroupViewModel(group, RemoveGroupAsync, OpenItemAsync, OnGroupChanged);
    }

    private void DebounceSave()
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
            await SaveAsync();
        }
        catch (OperationCanceledException)
        {
        }
    }
}
