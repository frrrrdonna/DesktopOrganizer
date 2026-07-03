namespace DesktopOrganizer.Core.Models;

public sealed class DesktopHostSettings
{
    public bool ReplaceDesktopOnStartup { get; set; } = true;

    public bool HideDesktopIcons { get; set; } = true;

    public bool AutoGenerateBaskets { get; set; } = true;

    public bool RestoreDesktopOnExit { get; set; } = true;
}
