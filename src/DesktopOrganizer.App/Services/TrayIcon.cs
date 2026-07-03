using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace DesktopOrganizer.App.Services;

public sealed class TrayIcon : IDisposable
{
    private const int WmTrayCallback = 0x0400;
    private const uint NimAdd = 0x00000000;
    private const uint NimModify = 0x00000001;
    private const uint NimDelete = 0x00000002;
    private const uint NifMessage = 0x00000001;
    private const uint NifIcon = 0x00000002;
    private const uint NifTip = 0x00000004;
    private const uint NifGuid = 0x00000020;
    private const uint WmLButtonUp = 0x0202;
    private const uint WmRButtonUp = 0x0205;

    private readonly HwndSource _hwndSource;
    private readonly ContextMenu _contextMenu;
    private string _tooltip;
    private bool _disposed;

    public TrayIcon(string tooltip = "Desktop Organizer", UIElement? contextMenuTarget = null)
    {
        _tooltip = tooltip;
        _contextMenu = new ContextMenu { PlacementTarget = contextMenuTarget, Placement = PlacementMode.MousePoint };
        _hwndSource = CreateMessageWindow();
        AddIcon();
    }

    public void AddMenuItem(string header, Action action, bool isBold = false)
    {
        var item = new MenuItem { Header = header };
        if (isBold) item.FontWeight = FontWeights.Bold;
        item.Click += (_, _) => action();
        _contextMenu.Items.Add(item);
    }

    public void AddSeparator()
    {
        _contextMenu.Items.Add(new Separator());
    }

    public void ClearAllItems()
    {
        _contextMenu.Items.Clear();
    }

    public void UpdateTooltip(string text)
    {
        _tooltip = text;
        var data = CreateNotifyIconData(NimModify);
        Shell_NotifyIcon(NimModify, ref data);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        DeleteIcon();
        _hwndSource.Dispose();
    }

    private HwndSource CreateMessageWindow()
    {
        var parameters = new HwndSourceParameters("TrayIconWindow")
        {
            Width = 0,
            Height = 0,
            WindowStyle = 0,
            ParentWindow = IntPtr.Zero,
        };

        var source = new HwndSource(parameters);
        source.AddHook(WndProc);
        return source;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WmTrayCallback) return IntPtr.Zero;

        var mouseMsg = (uint)lParam;

        if (mouseMsg == WmLButtonUp)
        {
            var firstItem = _contextMenu.Items.OfType<MenuItem>().FirstOrDefault();
            firstItem?.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
        }
        else if (mouseMsg == WmRButtonUp)
        {
            _contextMenu.IsOpen = true;
        }

        handled = true;
        return IntPtr.Zero;
    }

    private void AddIcon()
    {
        var data = CreateNotifyIconData(NimAdd);
        Shell_NotifyIcon(NimAdd, ref data);
    }

    private void DeleteIcon()
    {
        var data = CreateNotifyIconData(NimDelete);
        Shell_NotifyIcon(NimDelete, ref data);
    }

    private NOTIFYICONDATA CreateNotifyIconData(uint message)
    {
        var iconHandle = LoadIcon(IntPtr.Zero, (IntPtr)32512); // IDI_APPLICATION
        return new NOTIFYICONDATA
        {
            cbSize = Marshal.SizeOf<NOTIFYICONDATA>(),
            hWnd = _hwndSource.Handle,
            uID = 1,
            uFlags = NifMessage | NifIcon | NifTip,
            uCallbackMessage = WmTrayCallback,
            hIcon = iconHandle,
            szTip = _tooltip,
        };
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public uint dwState;
        public uint dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint uVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public uint dwInfoFlags;
        public Guid guidItem;
        public IntPtr hBalloonIcon;
    }

    [DllImport("shell32.dll", SetLastError = true)]
    private static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);
}
