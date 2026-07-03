# Desktop Organizer v0.3.0-alpha

## Alpha 版本说明

这是 `Desktop Organizer` 的第三代桌面层 Alpha 版本，重点在于篮子交互升级。

## 本次重点

- 篮子支持自由拖拽
- 接近屏幕边缘时自动吸附
- 各篮子位置完全独立
- 篮子项目列表支持独立滚动
- 桌面宿主从固定边缘布局切换为自由画布布局

## 已验证

- `dotnet test DesktopOrganizer.slnx`
- `dotnet build DesktopOrganizer.slnx -c Release`
- 当前程序启动 smoke test 通过

## 已知限制

- 篮子位置暂未持久化
- 刷新桌面后手动位置会被重置
- 当前仍为 Alpha 版本
- 当前仍不是安装器形态
