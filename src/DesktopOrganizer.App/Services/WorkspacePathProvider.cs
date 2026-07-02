using System.IO;

namespace DesktopOrganizer.App.Services;

public static class WorkspacePathProvider
{
    public static string GetDefaultPath()
    {
        var basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DesktopOrganizer");

        return Path.Combine(basePath, "workspace.json");
    }
}
