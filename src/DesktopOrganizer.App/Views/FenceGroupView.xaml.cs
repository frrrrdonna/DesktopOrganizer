using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DesktopOrganizer.App.ViewModels;

namespace DesktopOrganizer.App.Views;

public partial class FenceGroupView : UserControl
{
    private static readonly Brush DefaultBorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
    private static readonly Brush ActiveBorderBrush = new SolidColorBrush(Color.FromRgb(59, 130, 246));

    public FenceGroupView()
    {
        InitializeComponent();
    }

    private void OnRenameTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is FenceGroupViewModel vm && vm.IsEditing)
        {
            vm.CommitRenameCommand.Execute(null);
        }
    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            GroupBorder.BorderBrush = ActiveBorderBrush;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        GroupBorder.BorderBrush = DefaultBorderBrush;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        GroupBorder.BorderBrush = DefaultBorderBrush;

        if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
            e.Data.GetData(DataFormats.FileDrop) is string[] files &&
            DataContext is FenceGroupViewModel vm)
        {
            var skippedCount = 0;

            foreach (var file in files)
            {
                if (!vm.AddItem(file))
                {
                    skippedCount++;
                }
            }

            if (skippedCount > 0)
            {
                MessageBox.Show(
                    $"{skippedCount} item(s) were skipped because they were invalid or already added.",
                    "Desktop Organizer",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
