using System.Windows;

namespace DesktopOrganizer.App.Services;

public static class UserPrompts
{
    public static bool ConfirmDeleteGroup(string groupName)
    {
        return Confirm(
            $"Delete group \"{groupName}\" and remove it from the layout?",
            "Delete Group");
    }

    public static bool ConfirmDeleteItem(string itemName)
    {
        return Confirm(
            $"Remove item \"{itemName}\" from this group?",
            "Remove Item");
    }

    private static bool Confirm(string message, string title)
    {
        return MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }
}
