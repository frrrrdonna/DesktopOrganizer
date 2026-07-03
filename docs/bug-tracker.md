# Bug Tracker

## #1 — 强制终止进程导致桌面图标永久隐藏

**版本:** v0.2.1  
**发现日期:** 2026-07-03  
**严重程度:** 高（用户桌面图标消失）

### 现象

通过 `taskkill /f` 或 `Stop-Process -Force` 强制终止 `DesktopOrganizer.exe` 后，Windows 桌面图标消失且不会自动恢复。

### 根因

`DesktopSurfaceController.HideDesktopIcons()` 通过 Win32 `ShowWindow(hWnd, SW_HIDE)` 隐藏桌面 SysListView32 窗口。正常退出时 `App.OnExit` 会调用 `ShowDesktopIcons()` 恢复。但 `Stop-Process -Force` 会立即终止进程，不触发 `OnExit`，图标保持隐藏状态。

### 解决方案

引入 `DesktopSessionSnapshot` + `DesktopSessionSnapshotStore`：

1. **启动时** — 先检查是否存在残留快照文件。如果存在且 `DesktopIconsAreHidden == true`，说明上次异常退出，立即调用 `ShowDesktopIcons()` 恢复。
2. **正常运行时** — 在隐藏图标前捕获初始状态，持久化快照到 `%LocalAppData%/DesktopOrganizer/session-snapshot.json`。
3. **正常退出时** — 恢复图标，删除快照文件。
4. **异常退出时** — 快照文件残留在磁盘，下次启动时被步骤 1 检测并恢复。

### 关键文件

- `Core/Models/DesktopSessionSnapshot.cs`
- `Infrastructure/Desktop/DesktopSessionSnapshotStore.cs`
- `App.xaml.cs` — `RecoverFromPreviousSessionAsync()`

---

## #2 — UseWindowsForms 导致全项目命名空间污染

**版本:** v0.2.1  
**发现日期:** 2026-07-03  
**严重程度:** 中（阻塞编译）

### 现象

在 WPF 项目的 `.csproj` 中添加 `<UseWindowsForms>true</UseWindowsForms>` 后，大量文件出现 `CS0104` 歧义错误：

| 冲突类型 | WPF 命名空间 | WinForms 命名空间 |
|----------|-------------|-------------------|
| `Application` | `System.Windows.Application` | `System.Windows.Forms.Application` |
| `UserControl` | `System.Windows.Controls.UserControl` | `System.Windows.Forms.UserControl` |
| `MessageBox` | `System.Windows.MessageBox` | `System.Windows.Forms.MessageBox` |
| `DragEventArgs` | `System.Windows.DragEventArgs` | `System.Windows.Forms.DragEventArgs` |
| `DataFormats` | `System.Windows.DataFormats` | `System.Windows.Forms.DataFormats` |
| `DragDropEffects` | `System.Windows.DragDropEffects` | `System.Windows.Forms.DragDropEffects` |
| `Brush` | `System.Windows.Media.Brush` | `System.Drawing.Brush` |

### 根因

`<UseWindowsForms>true</UseWindowsForms>` 不仅添加 WinForms SDK 引用，还会在项目级别注入隐式全局 using（`System.Windows.Forms`、`System.Drawing` 等），与 WPF 的隐式 using 产生大范围命名冲突。逐个文件添加别名无法根治——每新增一个文件都可能触发新的冲突。

### 解决方案

**不使用 `<UseWindowsForms>`**。需要托盘图标时，直接用 P/Invoke 调用 `Shell_NotifyIcon` + `HwndSource` 消息窗口 + WPF `ContextMenu`，完全不依赖 WinForms。

### 关键文件

- `Services/TrayIcon.cs` — 纯 WPF 托盘图标实现（P/Invoke `Shell_NotifyIcon`）

### 教训

**WPF 项目中永远不要添加 `<UseWindowsForms>`。** 如果需要 WinForms 控件（如 `NotifyIcon`），使用 P/Invoke 或寻找 WPF 原生替代方案。

---

## #3 — WindowStyle="None" 窗口无法关闭

**版本:** v0.2.1  
**发现日期:** 2026-07-03  
**严重程度:** 中（用户体验）

### 现象

`DesktopHostWindow` 设置了 `WindowStyle="None"`（透明桌面叠加层需要无边框窗口），但这也移除了标题栏和关闭按钮。用户无法用常规方式关闭应用，只能通过任务管理器或命令行强制终止——这会触发 Bug #1。

### 解决方案

1. **系统托盘图标** — 提供 "Exit" 菜单项作为唯一的安全退出路径。
2. **OnClosing 拦截** — `DesktopHostWindow.OnClosing` 中设置 `e.Cancel = true; Hide()`，将 Alt+F4 / 关闭手势转为"最小化到托盘"而非退出。

### 关键文件

- `DesktopHostWindow.xaml.cs` — `OnClosing` 拦截
- `App.xaml.cs` — `TrayIcon` 创建和 `ShutdownGracefully()`

### 教训

任何 `WindowStyle="None"` 的 WPF 窗口必须提供替代的关闭机制（托盘图标、全局热键、或至少一个可见的退出按钮）。

---

## #4 — WPF Button 不支持 CornerRadius 属性

**版本:** v0.1.0  
**发现日期:** 2026-07-02  
**严重程度:** 低（编译错误）

### 现象

在 `MainWindow.xaml` 的 `<Button>` 上设置 `CornerRadius="6"` 导致编译错误：
```
MC3072: 属性 "CornerRadius" 在命名空间中不存在
```

### 根因

WPF 的 `Button` 类没有 `CornerRadius` 依赖属性。该属性存在于 UWP/WinUI 的 `Button` 上，但不在 WPF 中。WPF 中圆角需要通过 `Button` 的 `ControlTemplate` 中的 `Border.CornerRadius` 实现。

### 解决方案

移除 `Button` 上的 `CornerRadius`。对于平面按钮（`BorderThickness="0"`），无需圆角即可获得干净的视觉效果。

---

## #5 — AllowsTransparency 的性能与限制

**版本:** v0.2.1  
**发现日期:** 2026-07-03  
**严重程度:** 信息（设计约束）

### 约束

当 WPF 窗口使用 `AllowsTransparency="True"` 时：

1. **必须同时设置 `WindowStyle="None"`** — 否则会抛出异常。
2. **必须同时设置 `ResizeMode="NoResize"`** — 透明窗口不支持 WPF 标准缩放。
3. **GPU 渲染路径变更** — WPF 改用软件渲染层，在低端硬件上可能出现性能下降。
4. **无法使用 WPF 内置的窗口动画** — `WindowChrome` 与 `AllowsTransparency` 互斥。

### 未来改进方向

Windows 10/11 提供 `SetWindowCompositionAttribute` API 实现真正的 Acrylic/Mica 模糊效果，性能远优于 WPF 的 `AllowsTransparency`。后续版本可考虑通过 P/Invoke 使用该 API 替代。
---

## #6 - Tray exit hidden by close-to-tray interception

**Version:** v0.3.1  
**Found:** 2026-07-03  
**Severity:** High

### Symptom

When the user clicked `Exit` from the tray icon menu, Desktop Organizer appeared to exit, but desktop icons stayed hidden.

### Root Cause

`DesktopHostWindow.OnClosing` always canceled the close request and hid the window to tray.  
The tray exit path called `Shutdown()`, but the host window still intercepted closing, so the normal shutdown path that restores desktop icons in `App.OnExit` could be bypassed.

### Solution

Add an explicit shutdown bypass:

1. `DesktopHostWindow` exposes `PrepareForShutdown()`.
2. `PrepareForShutdown()` sets an internal flag allowing the next close request through.
3. `App.ShutdownGracefully()` closes settings, marks the host window for real shutdown, closes it, and then exits the application.

### Key Files

- `src/DesktopOrganizer.App/DesktopHostWindow.xaml.cs`
- `src/DesktopOrganizer.App/App.xaml.cs`
- `AGENT.md`

### Lesson

Any tray-based exit path must bypass close-to-tray interception explicitly, otherwise cleanup code like desktop-state restore may never run.
