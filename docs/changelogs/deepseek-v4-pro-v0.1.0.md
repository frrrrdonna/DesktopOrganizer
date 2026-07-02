# deepseek-v4-pro v0.1.0 — 2026-07-02

## 版本概述

桌面整理程序（Desktop Organizer）初始骨架 + 首个里程碑功能实现。
基于 WPF + .NET 10 + CommunityToolkit.Mvvm 构建，采用分层架构（Core / App / Infrastructure）。

---

## 新增 (Added)

### 项目结构
| 文件 | 说明 |
|------|------|
| `DesktopOrganizer.slnx` | .NET 解决方案文件 |
| `src/DesktopOrganizer.Core/` | 领域层：Models + Abstractions |
| `src/DesktopOrganizer.App/` | WPF 宿主应用：Views + ViewModels + Services |
| `src/DesktopOrganizer.Infrastructure/` | 基础设施层：Persistence + Shell |
| `tests/DesktopOrganizer.Core.Tests/` | xUnit 单元测试项目 |

### Core 层
- `Workspace` — 根聚合，包含分组列表
- `FenceGroup` — 组容器模型（Id, Name, X, Y, Width, Height, IsCollapsed, Portal, Items）
- `FenceItem` — 项目模型（Id, DisplayName, Path, Type）
- `FenceItemType` — 枚举（Unknown, Shortcut, File, Folder）
- `FolderPortal` — 文件夹入口模型（FolderPath）
- `IWorkspaceStore` — 持久化接口（LoadAsync / SaveAsync）
- `ILauncher` — Shell 启动接口（LaunchAsync）

### Infrastructure 层
- `JsonWorkspaceStore` — JSON 文件持久化实现，缺失文件时自动创建默认工作区
- `ShellLauncher` — 使用 `Process.Start` + `UseShellExecute` 打开文件/文件夹

### App 层 — ViewModels
- `BaseViewModel` — 基于 `ObservableObject` 的 MVVM 基类
- `MainWindowViewModel` — 顶层协调器：
  - `LoadCommand` — 从持久化加载工作区
  - `SaveCommand` — 保存工作区到磁盘
  - `CreateGroupCommand` — 创建新组并自动保存
  - 通过回调模式编排组操作（删除、打开项目、变更自动存档）
- `FenceGroupViewModel` — 组容器 ViewModel：
  - `ToggleCollapseCommand` — 折叠/展开
  - `BeginRename / CommitRename / CancelRename` — 内联重命名
  - `RemoveCommand` — 删除组
  - `AddItem(string path)` — 拖放添加项目，自动推断 FenceItemType
  - 变更回调 → 自动触发父级存档
- `FenceItemViewModel` — 项目 ViewModel：
  - `OpenCommand` — 通过 ShellLauncher 打开
  - `RemoveCommand` — 从组中移除

### App 层 — Views
- `MainWindow.xaml` — 工具栏（New Group + Save 按钮）+ WrapPanel 组网格
- `FenceGroupView.xaml` — 组卡片 UI：
  - 标题栏（显示/编辑双模式）
  - 项目计数显示
  - 项目列表（每项带打开/删除按钮）
  - 拖放文件支持（AllowDrop + 蓝色高亮反馈）
- `FenceGroupView.xaml.cs` — 拖放事件处理（DragEnter / DragLeave / Drop → 提取文件路径）
- `App.xaml.cs` — 启动组合根，手动注入 JsonWorkspaceStore + ShellLauncher

### App 层 — Services
- `WorkspacePathProvider` — 返回 `%LocalAppData%/DesktopOrganizer/workspace.json`

### 测试
- `WorkspaceTests` — 5 个测试：
  - Workspace 默认值
  - FenceGroup 默认值
  - FenceItem 默认值
  - Workspace 添加/删除组
  - FenceGroup 添加/删除项目

### 依赖项
| 包 | 用途 |
|----|------|
| `CommunityToolkit.Mvvm 8.4.0` | MVVM 源生成器 |
| `xunit 2.9.3` | 单元测试框架 |
| `Microsoft.NET.Test.Sdk 17.14.1` | 测试运行器 |
| `coverlet.collector 6.0.4` | 代码覆盖率 |

---

## 修改 (Changed)

无 — 此为首个版本。

---

## 删除 (Removed)

无 — 此为首个版本。

---

## 已知问题 (Known Issues)

1. **无依赖注入容器** — 当前在 `App.xaml.cs` 中手动 new 依赖项。项目规模增长后应引入 `Microsoft.Extensions.DependencyInjection`。
2. **拖放不验证路径有效性** — `AddItem` 接受任意字符串路径，不检查文件是否存在。
3. **删除项目无用确认** — 点击 ✕ 立即删除，无确认对话框。
4. **重命名不支持 Escape 取消焦点** — 编辑模式下点击组外区域不会自动取消重命名。
5. **无自动存档防抖** — 每次变更立即存档，频繁操作时可能产生 IO 压力。
6. **无工作区版本迁移机制** — JSON 格式变更后旧文件将加载失败。
7. **ShellLauncher 为伪异步** — `Process.Start` 是同步调用，包裹在 `Task.CompletedTask` 中。
8. **组位置未持久化到布局** — X/Y 字段存在但 UI 未使用绝对定位，组以 WrapPanel 自动排列。
9. **边界情况：空名称组** — 重命名为空白字符串被 CommitRename 忽略，但创建组时允许空名称。

---

## 测试结果

```
通过: 5  |  失败: 0  |  跳过: 0  |  警告: 0  |  错误: 0
```
