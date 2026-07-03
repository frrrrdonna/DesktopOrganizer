using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.App.ViewModels;

public sealed partial class BasketViewModel : BaseViewModel
{
    private const double SnapDistance = 40;
    private const double PanelHeightEstimate = 400;

    [ObservableProperty]
    private bool _isCollapsed;

    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private BasketDockEdge? _snapEdge;

    public BasketViewModel(Basket model, Func<string, Task>? onOpenItem = null)
    {
        Model = model;
        _isCollapsed = model.IsCollapsed;
        _x = model.X;
        _y = model.Y;
        _snapEdge = model.SnapEdge;
        Items = new ObservableCollection<DesktopItemViewModel>(
            model.Items.Select(item => new DesktopItemViewModel(item, onOpenItem)));
    }

    public Basket Model { get; }

    public string Name => Model.Name;

    public BasketDockEdge DockEdge => Model.DockEdge;

    public ObservableCollection<DesktopItemViewModel> Items { get; }

    public double PanelWidth => IsCollapsed ? 72 : 300;

    partial void OnIsCollapsedChanged(bool value)
    {
        Model.IsCollapsed = value;
        OnPropertyChanged(nameof(PanelWidth));
    }

    partial void OnXChanged(double value) => Model.X = value;

    partial void OnYChanged(double value) => Model.Y = value;

    partial void OnSnapEdgeChanged(BasketDockEdge? value) => Model.SnapEdge = value;

    [RelayCommand]
    private void ToggleCollapse()
    {
        IsCollapsed = !IsCollapsed;
    }

    public void ApplyDragDelta(double dx, double dy)
    {
        SnapEdge = null;
        X = ClampX(X + dx);
        Y = ClampY(Y + dy);
    }

    public void SnapToNearestEdge()
    {
        var screenW = SystemParameters.VirtualScreenWidth;
        var screenH = SystemParameters.VirtualScreenHeight;

        if (X <= SnapDistance)
        {
            X = 8;
            SnapEdge = BasketDockEdge.Left;
        }
        else if (X + PanelWidth >= screenW - SnapDistance)
        {
            X = screenW - PanelWidth - 8;
            SnapEdge = BasketDockEdge.Right;
        }
        else if (Y <= SnapDistance)
        {
            Y = 8;
            SnapEdge = BasketDockEdge.Top;
        }
        else if (Y + PanelHeightEstimate >= screenH - SnapDistance)
        {
            Y = screenH - PanelHeightEstimate - 8;
            SnapEdge = BasketDockEdge.Bottom;
        }
        else
        {
            SnapEdge = null;
        }

        X = ClampX(X);
        Y = ClampY(Y);
    }

    private static double ClampX(double x)
    {
        var maxX = SystemParameters.VirtualScreenWidth - 72;
        return Math.Max(0, Math.Min(x, maxX));
    }

    private static double ClampY(double y)
    {
        var maxY = SystemParameters.VirtualScreenHeight - 40;
        return Math.Max(0, Math.Min(y, maxY));
    }
}
