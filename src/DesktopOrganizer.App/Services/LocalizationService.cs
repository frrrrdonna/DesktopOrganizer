using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopOrganizer.App.Services;

public sealed class LocalizationService : INotifyPropertyChanged
{
    private static readonly Dictionary<string, Dictionary<string, string>> StringTables = new()
    {
        ["en-US"] = new Dictionary<string, string>
        {
            ["Settings.Title"] = "Settings",
            ["Settings.ReplaceDesktopOnStartup"] = "Replace desktop on startup",
            ["Settings.HideDesktopIcons"] = "Hide original desktop icons while host is running",
            ["Settings.AutoGenerateBaskets"] = "Auto-generate baskets from detected desktop content",
            ["Settings.RestoreDesktopOnExit"] = "Restore original desktop icon visibility on exit",
            ["Settings.Language"] = "Language",
            ["Settings.PhysicallyOrganize"] = "Physically organize desktop files into managed folders (coming soon)",
            ["Settings.Close"] = "Close",
            ["Settings.Footer"] = "v0.3.2 adds draggable baskets with uniform sizing, double-click collapse, and multi-language support.",
            ["Tray.Show"] = "Show Desktop Organizer",
            ["Tray.Settings"] = "Settings",
            ["Tray.Refresh"] = "Refresh Baskets",
            ["Tray.Exit"] = "Exit",
            ["Host.Status.Scanning"] = "Scanning desktop items...",
            ["Host.Status.Ready"] = "Desktop host active — {0} item(s) in {1} basket(s). Drag to move, scroll to browse. Wallpaper preserved.",
            ["Host.Refresh"] = "Refresh",
            ["Host.Settings"] = "Settings",
            ["Host.Title"] = "Desktop Organizer",
        },
        ["zh-CN"] = new Dictionary<string, string>
        {
            ["Settings.Title"] = "设置",
            ["Settings.ReplaceDesktopOnStartup"] = "启动时替换桌面",
            ["Settings.HideDesktopIcons"] = "运行时隐藏原始桌面图标",
            ["Settings.AutoGenerateBaskets"] = "自动从检测到的桌面内容生成篮子",
            ["Settings.RestoreDesktopOnExit"] = "退出时恢复原始桌面图标可见性",
            ["Settings.Language"] = "语言",
            ["Settings.PhysicallyOrganize"] = "将桌面文件物理整理到托管文件夹（即将推出）",
            ["Settings.Close"] = "关闭",
            ["Settings.Footer"] = "v0.3.2 新增可拖动篮子（统一尺寸）、双击折叠、多语言支持。",
            ["Tray.Show"] = "显示桌面整理器",
            ["Tray.Settings"] = "设置",
            ["Tray.Refresh"] = "刷新篮子",
            ["Tray.Exit"] = "退出",
            ["Host.Status.Scanning"] = "正在扫描桌面项目...",
            ["Host.Status.Ready"] = "桌面托管已激活 — {0} 个项目，{1} 个篮子。拖动移动，滚动浏览。壁纸已保留。",
            ["Host.Refresh"] = "刷新",
            ["Host.Settings"] = "设置",
            ["Host.Title"] = "桌面整理器",
        },
    };

    public const string DefaultLanguage = "en-US";

    public static readonly string[] SupportedLanguages = ["en-US", "zh-CN"];

    private static string _currentLanguage = DefaultLanguage;

    public static LocalizationService Instance { get; } = new();

    public static string CurrentLanguage
    {
        get => _currentLanguage;
        private set
        {
            if (_currentLanguage == value) return;
            _currentLanguage = value;
            Instance.RefreshAll();
        }
    }

    // Static properties for XAML {x:Static} bindings
    public static string SettingsTitle => Get("Settings.Title");
    public static string SettingsReplaceDesktopOnStartup => Get("Settings.ReplaceDesktopOnStartup");
    public static string SettingsHideDesktopIcons => Get("Settings.HideDesktopIcons");
    public static string SettingsAutoGenerateBaskets => Get("Settings.AutoGenerateBaskets");
    public static string SettingsRestoreDesktopOnExit => Get("Settings.RestoreDesktopOnExit");
    public static string SettingsLanguage => Get("Settings.Language");
    public static string SettingsPhysicallyOrganize => Get("Settings.PhysicallyOrganize");
    public static string SettingsClose => Get("Settings.Close");
    public static string SettingsFooter => Get("Settings.Footer");

    // Instance properties for {Binding} dynamic updates
    public string Title => Get("Settings.Title");
    public string ReplaceDesktopOnStartup => Get("Settings.ReplaceDesktopOnStartup");
    public string HideDesktopIcons => Get("Settings.HideDesktopIcons");
    public string AutoGenerateBaskets => Get("Settings.AutoGenerateBaskets");
    public string RestoreDesktopOnExit => Get("Settings.RestoreDesktopOnExit");
    public string Language => Get("Settings.Language");
    public string PhysicallyOrganize => Get("Settings.PhysicallyOrganize");
    public string Close => Get("Settings.Close");
    public string Footer => Get("Settings.Footer");
    public string HostTitle => Get("Host.Title");
    public string HostRefresh => Get("Host.Refresh");
    public string HostSettings => Get("Host.Settings");

    public static string Get(string key)
    {
        if (StringTables.TryGetValue(CurrentLanguage, out var table) &&
            table.TryGetValue(key, out var value))
        {
            return value;
        }

        if (CurrentLanguage != "en-US" &&
            StringTables.TryGetValue("en-US", out var fallback) &&
            fallback.TryGetValue(key, out var fallbackValue))
        {
            return fallbackValue;
        }

        return key;
    }

    public static string Format(string key, params object[] args)
    {
        return string.Format(Get(key), args);
    }

    public static void SetLanguage(string language)
    {
        if (StringTables.ContainsKey(language))
        {
            CurrentLanguage = language;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void RefreshAll()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(ReplaceDesktopOnStartup));
        OnPropertyChanged(nameof(HideDesktopIcons));
        OnPropertyChanged(nameof(AutoGenerateBaskets));
        OnPropertyChanged(nameof(RestoreDesktopOnExit));
        OnPropertyChanged(nameof(Language));
        OnPropertyChanged(nameof(PhysicallyOrganize));
        OnPropertyChanged(nameof(Close));
        OnPropertyChanged(nameof(Footer));
        OnPropertyChanged(nameof(HostTitle));
        OnPropertyChanged(nameof(HostRefresh));
        OnPropertyChanged(nameof(HostSettings));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
