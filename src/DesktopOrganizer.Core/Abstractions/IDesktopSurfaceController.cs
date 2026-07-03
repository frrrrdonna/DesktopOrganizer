namespace DesktopOrganizer.Core.Abstractions;

public interface IDesktopSurfaceController
{
    bool AreDesktopIconsVisible();

    bool HideDesktopIcons();

    bool ShowDesktopIcons();

    bool AttachWindowToDesktop(IntPtr windowHandle);
}
