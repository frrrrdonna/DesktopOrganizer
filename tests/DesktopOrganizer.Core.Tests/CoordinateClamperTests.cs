using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Core.Tests;

public sealed class CoordinateClamperTests
{
    [Fact]
    public void ClampX_ReturnsSameValue_WhenWithinBounds()
    {
        var result = CoordinateClamper.ClampX(100, 1920);
        Assert.Equal(100, result);
    }

    [Fact]
    public void ClampX_ClampsToZero_WhenBelowZero()
    {
        var result = CoordinateClamper.ClampX(-50, 1920);
        Assert.Equal(0, result);
    }

    [Fact]
    public void ClampX_ClampsToMax_WhenAboveScreenWidth()
    {
        var result = CoordinateClamper.ClampX(2000, 1920);
        Assert.Equal(1920 - 72, result);
    }

    [Fact]
    public void ClampX_ClampsToMax_WhenAtExactBoundary()
    {
        var maxX = 1920 - 72;
        var result = CoordinateClamper.ClampX(maxX, 1920);
        Assert.Equal(maxX, result);
    }

    [Fact]
    public void ClampY_ReturnsSameValue_WhenWithinBounds()
    {
        var result = CoordinateClamper.ClampY(200, 1080);
        Assert.Equal(200, result);
    }

    [Fact]
    public void ClampY_ClampsToZero_WhenBelowZero()
    {
        var result = CoordinateClamper.ClampY(-30, 1080);
        Assert.Equal(0, result);
    }

    [Fact]
    public void ClampY_ClampsToMax_WhenAboveScreenHeight()
    {
        var result = CoordinateClamper.ClampY(1200, 1080);
        Assert.Equal(1080 - 40, result);
    }
}
