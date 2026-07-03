using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DesktopOrganizer.App.ViewModels;

namespace DesktopOrganizer.App.Views;

public partial class BasketView : UserControl
{
    public BasketView()
    {
        InitializeComponent();
    }

    private void OnDragDelta(object sender, DragDeltaEventArgs e)
    {
        if (DataContext is not BasketViewModel vm) return;
        vm.ApplyDragDelta(e.HorizontalChange, e.VerticalChange);
    }

    private void OnDragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (DataContext is not BasketViewModel vm) return;
        vm.SnapToNearestEdge();
    }

    private void OnTitleBarDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not BasketViewModel vm) return;
        vm.ToggleCollapseCommand.Execute(null);
        e.Handled = true;
    }
}
