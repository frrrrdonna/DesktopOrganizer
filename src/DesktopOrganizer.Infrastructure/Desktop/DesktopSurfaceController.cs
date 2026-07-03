using System.Runtime.InteropServices;
using DesktopOrganizer.Core.Abstractions;

namespace DesktopOrganizer.Infrastructure.Desktop;

public sealed class DesktopSurfaceController : IDesktopSurfaceController
{
    private const int SwHide = 0;
    private const int SwShow = 5;
    private const uint SmtoNormal = 0x0000;
    private static readonly IntPtr ProgmanMessage = new(0x052C);

    public bool AreDesktopIconsVisible()
    {
        var listViewHandle = FindDesktopListViewHandle();
        return listViewHandle != IntPtr.Zero && IsWindowVisible(listViewHandle);
    }

    public bool HideDesktopIcons()
    {
        var listViewHandle = FindDesktopListViewHandle();

        if (listViewHandle == IntPtr.Zero)
        {
            return false;
        }

        ShowWindow(listViewHandle, SwHide);
        return true;
    }

    public bool ShowDesktopIcons()
    {
        var listViewHandle = FindDesktopListViewHandle();

        if (listViewHandle == IntPtr.Zero)
        {
            return false;
        }

        ShowWindow(listViewHandle, SwShow);
        return true;
    }

    public bool AttachWindowToDesktop(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        var progman = FindWindow("Progman", null);

        if (progman != IntPtr.Zero)
        {
            SendMessageTimeout(progman, ProgmanMessage, IntPtr.Zero, IntPtr.Zero, SmtoNormal, 1000, out _);
        }

        var workerW = FindWorkerW();

        if (workerW == IntPtr.Zero)
        {
            return false;
        }

        return SetParent(windowHandle, workerW) != IntPtr.Zero;
    }

    private static IntPtr FindDesktopListViewHandle()
    {
        IntPtr shellView = IntPtr.Zero;

        EnumWindows((topHandle, _) =>
        {
            var candidate = FindWindowEx(topHandle, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (candidate != IntPtr.Zero)
            {
                shellView = candidate;
                return false;
            }

            return true;
        }, IntPtr.Zero);

        if (shellView == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        return FindWindowEx(shellView, IntPtr.Zero, "SysListView32", "FolderView");
    }

    private static IntPtr FindWorkerW()
    {
        var progman = FindWindow("Progman", null);

        if (progman != IntPtr.Zero)
        {
            SendMessageTimeout(progman, ProgmanMessage, IntPtr.Zero, IntPtr.Zero, SmtoNormal, 1000, out _);
        }

        IntPtr workerW = IntPtr.Zero;

        EnumWindows((topHandle, _) =>
        {
            var shellView = FindWindowEx(topHandle, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (shellView == IntPtr.Zero)
            {
                return true;
            }

            workerW = FindWindowEx(IntPtr.Zero, topHandle, "WorkerW", null);
            return workerW == IntPtr.Zero;
        }, IntPtr.Zero);

        return workerW;
    }

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string? className, string? windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        IntPtr msg,
        IntPtr wParam,
        IntPtr lParam,
        uint fuFlags,
        uint uTimeout,
        out IntPtr lpdwResult);
}
