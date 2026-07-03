# Desktop Organizer v0.3.4

`v0.3.4` 是一个聚焦收口的补丁版本，主要修复语言切换后宿主界面不能即时刷新的问题，并补齐 `v0.3.3` 宿主持久化链路的自动化测试。

## 本次重点

- 语言切换后，宿主窗口状态文本、工具栏按钮、托盘菜单项、托盘提示文本可在当前会话内即时更新，无需重启应用。
- 自动化测试从 `v0.3.3` 的 5 个提升到 19 个，新增覆盖坐标约束、分类器稳定 key、篮子路由和宿主状态 JSON 持久化。
- 将坐标约束逻辑提取为 `CoordinateClamper`，替代原先宿主 ViewModel 中不可单测的私有实现。

## 具体变更

| 区域 | 内容 |
|------|------|
| 宿主状态文本 | `DesktopHostViewModel` 订阅 `LocalizationService.Instance.PropertyChanged`，语言切换后会重新计算 `StatusText`。 |
| 托盘菜单 | `TrayIcon` 新增 `ClearAllItems()` 和 `UpdateTooltip()`；`App.xaml.cs` 在语言切换时重建托盘菜单。 |
| 托盘提示 | 不再硬编码 `"Desktop Organizer"`，改为读取 `LocalizationService.Get("Host.Title")`。 |
| 坐标约束 | `ClampX` / `ClampY` 从 `DesktopHostViewModel` 提取到 `DesktopOrganizer.Core` 的 `CoordinateClamper`。 |
| 测试覆盖 | 新增 `CoordinateClamperTests`（7）、`DesktopClassifierTests`（5）、`JsonDesktopHostStateStoreTests`（2）。 |

## 验证

```text
dotnet build DesktopOrganizer.slnx -c Release
dotnet test DesktopOrganizer.slnx
```

备份文件：`backups/v0.3.4.tar.gz`

## 当前限制

- 多显示器布局恢复仍只基于虚拟屏幕坐标。
- 篮子身份仍受限于默认分类 key，暂不支持自定义持久化篮子。
- 桌面扫描结果仍为运行时生成，暂未持久化。
