# v0.2.1 — 2026-07-03

## 版本概述

将 Desktop Organizer 从不透明桌面宿主窗口升级为透明桌面叠加层，实现壁纸保留、浮动半透明篮子面板、安全的图标隐藏/恢复机制，以及异常退出恢复能力。

> **前期工作 (codex):** 本版本的 spec 文档 `docs/v0.2.1-transparent-desktop-spec.md` 在此版本开始时作为纯文档提交。

---

## 新增 (Added)

| 文件 | 说明 |
|------|------|
| `src/DesktopOrganizer.Core/Models/DesktopSessionSnapshot.cs` | 桌面会话快照模型：记录图标初始可见性、当前隐藏状态、捕获时间戳，用于安全恢复。 |
| `src/DesktopOrganizer.Infrastructure/Desktop/DesktopSessionSnapshotStore.cs` | 快照持久化存储：异步 JSON 读写 + 删除，用于异常退出后的桌面恢复。 |

---

## 修改 (Changed)

| 文件 | 变更内容 |
|------|----------|
| `src/DesktopOrganizer.App/DesktopHostWindow.xaml` | 完全重构为透明叠加层：`AllowsTransparency="True"`、`Background="Transparent"`；移除不透明背景色、中心占位面板、底部状态栏；工具栏改为居中浮动半透明条（`Background="#CCF0ECE2"`）。 |
| `src/DesktopOrganizer.App/Views/BasketView.xaml` | 重新设计为浮动面板：半透明背景（`#CCF7F3E8`）、`DropShadowEffect` 阴影（BlurRadius=20, Opacity=0.25）、标题栏带分隔线、项目卡片使用 `#DDFFFDF8` 确保在任意壁纸上可读。 |
| `src/DesktopOrganizer.App/SettingsWindow.xaml` | 新增禁用状态的 "Physically organize desktop files into managed folders (coming soon)" 占位复选框；更新版本文本为 v0.2.1。 |
| `src/DesktopOrganizer.App/ViewModels/DesktopHostViewModel.cs` | 状态文本更新："Preparing transparent desktop layer..." → "Desktop host active — N item(s) in M basket(s). Wallpaper preserved." |
| `src/DesktopOrganizer.App/App.xaml.cs` | 引入 `DesktopSessionSnapshot` + `DesktopSessionSnapshotStore` 生命周期管理：启动时先恢复上一次异常退出遗留的隐藏图标（`RecoverFromPreviousSessionAsync`）；捕获初始图标状态→隐藏→持久化快照；正常退出时恢复图标并删除快照文件。 |

---

## 删除 (Removed)

无文件删除。

以下内容从 `DesktopHostWindow.xaml` 中移除：
- 不透明全屏背景色 `#C9B79C`
- 中心 "Desktop Layer Host" 占位面板
- 底部长文本状态栏

---

## 已知问题 (Known Issues)

1. **WorkerW 附加仍然尽力而为** — `DesktopSurfaceController.AttachWindowToDesktop` 不保证在所有 Windows 版本上成功。
2. **AllowsTransparency 性能开销** — WPF 的 `AllowsTransparency="True"` 在低端 GPU 上可能引起渲染性能下降；未来可考虑使用 `SetWindowCompositionAttribute` (Windows 10/11 Acrylic API) 替代。
3. **半透明文本对比度** — 在极亮或极暗壁纸上，篮子标题和项目文字的对比度可能不足；未实现动态文字颜色适配。
4. **快照仅覆盖图标恢复** — 快照不包含篮子布局、停靠位置或用户自定义设置；这些仍需后续版本补充。
5. **设置不持久化** — `DesktopHostSettings` 在应用重启后重置为默认值；JSON 序列化路径已存在但尚未接入。
6. **原有的 Workspace/FenceGroup 窗口仍在代码中但不再是主要启动路径** — 未删除以避免破坏现有功能。
7. **文件组织选项始终禁用** — Settings 中的 "Physically organize" 复选框为纯占位，无后端逻辑。

---

## 测试结果

```
dotnet build: 0 warnings, 0 errors
dotnet test:  5 passed, 0 failed, 0 skipped
```

---

## 验收对照 (per v0.2.1 spec §9)

| # | 标准 | 状态 |
|---|------|------|
| 1 | 壁纸在应用运行时保持可见 | ✅ `AllowsTransparency="True"` + `Background="Transparent"` |
| 2 | 启动时隐藏桌面图标，退出时恢复 | ✅ `DesktopSessionSnapshot` + `RecoverFromPreviousSessionAsync` |
| 3 | 篮子以浮动叠加层渲染 | ✅ 半透明背景 + `DropShadowEffect` |
| 4 | 桌面项目扫描映射，不移动真实文件 | ✅ `DesktopScanner` + `DesktopClassifier`（只读扫描） |
| 5 | 篮子项目仍可打开原始目标 | ✅ `ShellLauncher.LaunchAsync` |
| 6 | 篮子支持折叠和边停靠 | ✅ `ToggleCollapseCommand` + `BasketDockEdge` 布局 |
| 7 | 设置暴露桌面接管开关 | ✅ SettingsWindow 四个开关 |
| 8 | 编译、测试、启动冒烟检查均通过 | ✅ |
