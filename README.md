# Desktop Organizer

一个面向 `Windows` 的桌面整理工具原型，灵感来自 `Stardock Fences`。  
它的目标是提供类似“桌面分组收纳”的体验，让用户可以把快捷方式、文件和文件夹按组整理，并在下次启动时恢复布局。

## 项目定位

`Desktop Organizer` 目前处于 `Alpha` 阶段，重点是验证以下核心体验：

- 创建分组
- 分组重命名
- 折叠 / 展开分组
- 拖拽文件、文件夹、快捷方式到分组
- 点击打开条目
- 删除条目和分组
- 工作区自动保存与重启恢复

当前版本更适合作为内部测试原型，还不是最终稳定版。

## 当前特性

- 基于 `WPF + .NET 10` 构建
- 分层架构：应用层 / 核心层 / 基础设施层 / 测试层
- JSON 工作区持久化
- 删除确认弹窗
- 拖拽去重与无效路径过滤
- 自动保存防抖
- `Windows x64` 单文件测试包

## 技术栈

- `C#`
- `.NET 10`
- `WPF`
- `CommunityToolkit.Mvvm`
- `xUnit`

## 项目结构

```text
src/
  DesktopOrganizer.App/              # WPF 应用层
  DesktopOrganizer.Core/             # 领域模型与抽象接口
  DesktopOrganizer.Infrastructure/   # 持久化与系统集成
tests/
  DesktopOrganizer.Core.Tests/       # 单元测试
docs/
  development-guide.md               # 开发说明
  release-checklist.md               # 发布前检查清单
  changelogs/                        # 版本变更记录
```

## 本地运行

```powershell
dotnet build DesktopOrganizer.slnx
dotnet run --project src/DesktopOrganizer.App/DesktopOrganizer.App.csproj
```

## 测试

```powershell
dotnet test DesktopOrganizer.slnx
```

## 打包

当前仓库已经支持生成 `Windows x64` 自包含单文件测试包：

```powershell
dotnet publish src/DesktopOrganizer.App/DesktopOrganizer.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/packages/v0.1.3-alpha
```

## 路线图

接下来的重点方向：

- 更完整的桌面分组交互
- 更稳定的持久化与布局恢复
- 更正式的安装包交付形式
- 自定义图标与发布元数据完善

## 文档

- [开发说明](docs/development-guide.md)
- [发布检查清单](docs/release-checklist.md)
- [Alpha 打包说明](docs/alpha-package-notes-v0.1.3.md)

## 状态说明

当前仓库已经可以生成可启动的 Alpha 测试包，但仍有这些已知限制：

- 还不是安装器形态
- 仍使用默认程序图标
- 组布局仍是基于 `WrapPanel` 的原型方案，不是最终桌面布局系统
