using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using DesktopOrganizer.Core.Abstractions;
using DesktopOrganizer.App.ViewModels;

namespace DesktopOrganizer.App;

public partial class DesktopHostWindow : Window
{
    private readonly IDesktopSurfaceController _desktopSurfaceController;
    private bool _allowRealClose;

    public DesktopHostWindow(DesktopHostViewModel viewModel, IDesktopSurfaceController desktopSurfaceController)
    {
        _desktopSurfaceController = desktopSurfaceController;
        DataContext = viewModel;
        InitializeComponent();
        SourceInitialized += OnSourceInitialized;
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
        Width = SystemParameters.VirtualScreenWidth;
        Height = SystemParameters.VirtualScreenHeight;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        _desktopSurfaceController.AttachWindowToDesktop(handle);
    }

    public void PrepareForShutdown()
    {
        _allowRealClose = true;
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_allowRealClose)
        {
            return;
        }

        // Minimize to system tray instead of closing.
        // Only "Exit" from the tray context menu truly exits.
        e.Cancel = true;
        Hide();
    }
}
