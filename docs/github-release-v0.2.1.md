# Desktop Organizer v0.2.1-alpha

## Alpha 版本说明

这是 `Desktop Organizer` 的桌面接管原型 Alpha 版本。

## 本次重点

- 透明桌面覆盖层
- 桌面图标隐藏与退出恢复
- 从桌面内容自动生成浮动篮子
- 托盘菜单与安全退出路径
- 异常退出后的桌面图标恢复快照

## 包含内容

- Windows x64 自包含单文件测试包
- 桌面扫描与分类
- 浮动篮子界面
- 设置窗口
- 桌面宿主窗口

## 已验证

- `dotnet test DesktopOrganizer.slnx`
- `dotnet build DesktopOrganizer.slnx -c Release`
- 当前桌面宿主程序启动 smoke test 通过

## 已知限制

- 当前仍为 Alpha 版本
- 还不是安装器形态
- WorkerW 附着仍是尽力而为，不保证所有 Windows 环境行为一致
- 桌面文件仍是映射显示，不会物理移动
