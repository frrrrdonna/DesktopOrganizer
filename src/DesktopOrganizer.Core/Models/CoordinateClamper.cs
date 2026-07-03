namespace DesktopOrganizer.Core.Models;

public static class CoordinateClamper
{
    public static double ClampX(double x, double screenWidth)
    {
        var maxX = screenWidth - 72;
        return Math.Max(0, Math.Min(x, maxX));
    }

    public static double ClampY(double y, double screenHeight)
    {
        var maxY = screenHeight - 40;
        return Math.Max(0, Math.Min(y, maxY));
    }
}
