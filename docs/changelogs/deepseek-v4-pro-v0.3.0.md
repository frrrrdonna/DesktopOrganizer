# deepseek-v4-pro v0.3.0 — 2026-07-03

## 版本概述

篮子交互升级：支持自由拖拽移动、靠近屏幕边缘自动吸附、项目列表独立滚轮滚动、各篮子位置完全独立。布局从固定四边停靠切换为 Canvas 自由定位。

---

## 新增 (Added)

| 文件 | 说明 |
|------|------|
| - | 无新增文件 |

### Basket 模型新增字段
| 字段 | 类型 | 说明 |
|------|------|------|
| `Basket.X` | `double` | 篮子当前 X 坐标（Canvas.Left） |
| `Basket.Y` | `double` | 篮子当前 Y 坐标（Canvas.Top） |
| `Basket.SnapEdge` | `BasketDockEdge?` | 当前吸附边缘，null 表示自由位置 |

### BasketViewModel 新增
| 成员 | 说明 |
|------|------|
| `X`, `Y`, `SnapEdge` 可观察属性 | 与 Model 双向同步 |
| `ApplyDragDelta(dx, dy)` | 拖拽增量，自动解除吸附 |
| `SnapToNearestEdge()` | 40px 阈值边缘吸附：左/右/上/下 |
| `ClampX/ClampY` | 防止篮子拖出屏幕边界 |

### BasketView 新增
| 功能 | 说明 |
|------|------|
| **Thumb 拖拽手柄** | 标题栏整行可拖，`Cursor="SizeAll"` 视觉提示 |
| **ScrollViewer** | 包裹 ItemsControl，鼠标滚轮滚动文件列表 |
| **DragDelta + DragCompleted** | Thumb 事件 → ViewModel 方法 |

### DesktopHostWindow 布局变更
| 变更前 | 变更后 |
|--------|--------|
| 4 个独立 ItemsControl（Left/Right/Top/Bottom）| 1 个 ItemsControl + Canvas ItemsPanel |
| 固定边缘停靠，无法移动 | Canvas.Left/Top 绑定，自由定位 |

### DesktopHostViewModel 变更
| 变更前 | 变更后 |
|--------|--------|
| `LeftBaskets`, `RightBaskets`, `TopBaskets`, `BottomBaskets` | `AllBaskets` 单一集合 |
| 无位置计算 | `PlaceBasket()` 根据 DockEdge 计算初始位置，同类篮子垂直/水平错开 340px |

---

## 修改 (Changed)

| 文件 | 变更内容 |
|------|----------|
| `Core/Models/Basket.cs` | 新增 `X`, `Y`, `SnapEdge` 属性 |
| `App/ViewModels/BasketViewModel.cs` | 新增位置属性、`ApplyDragDelta`、`SnapToNearestEdge`、`ClampX/ClampY` |
| `App/Views/BasketView.xaml` | 标题栏改为 `Thumb` 拖拽手柄；ItemsControl 外包 `ScrollViewer`；移除 `Margin="8"` |
| `App/Views/BasketView.xaml.cs` | 新增 `OnDragDelta`、`OnDragCompleted` 事件处理 |
| `App/DesktopHostWindow.xaml` | 用 Canvas ItemsControl 替换 4 边 Grid 布局 |
| `App/ViewModels/DesktopHostViewModel.cs` | `AllBaskets` 替换 4 个边集合；新增 `PlaceBasket` 初始定位逻辑 |

---

## 删除 (Removed)

无文件删除。

从 `DesktopHostViewModel` 移除：
- `LeftBaskets`, `RightBaskets`, `TopBaskets`, `BottomBaskets` 四个集合
- `SelectCollection` 方法

---

## 已知问题 (Known Issues)

1. **刷新后拖拽位置丢失** — `RefreshDesktopAsync` 重新分类并重建所有篮子，用户手动调整的位置会被重置为初始位置。后续版本应保留用户位置。
2. **无位置持久化** — 篮子位置不会保存到磁盘，应用重启后恢复初始布局。需要将 `X`/`Y`/`SnapEdge` 纳入 `JsonWorkspaceStore` 或独立设置文件。
3. **吸附无动画** — 拖拽结束时篮子瞬间跳到吸附位置，无过渡动画。
4. **多显示器仅覆盖主屏** — `DesktopHostWindow` 使用 `VirtualScreenWidth/Height`，在 `OnLoaded` 中设置窗口覆盖所有虚拟屏幕，但初始位置计算使用主屏尺寸。
5. **Thumb 与 ToggleCollapse 按钮冲突** — 折叠按钮在 Thumb 内部且设置了 `Cursor="Hand"` 以区分，但快速点击时可能触发微小的拖拽事件。

---

## 测试结果

```
dotnet build: 0 warnings, 0 errors
dotnet test:  5 passed, 0 failed, 0 skipped
```
