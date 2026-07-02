using System.Windows;
using DesktopOrganizer.App.Services;
using DesktopOrganizer.App.ViewModels;
using DesktopOrganizer.Infrastructure.Persistence;
using DesktopOrganizer.Infrastructure.Shell;

namespace DesktopOrganizer.App;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var workspaceStore = new JsonWorkspaceStore(WorkspacePathProvider.GetDefaultPath());
        var launcher = new ShellLauncher();
        var viewModel = new MainWindowViewModel(workspaceStore, launcher);
        await viewModel.LoadCommand.ExecuteAsync(null);

        var window = new MainWindow
        {
            DataContext = viewModel,
        };

        window.Show();
    }
}
