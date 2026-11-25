<p align="center">
  <img src="assets/Cccc.svg" alt="Cccc 图标" width="128" height="128">
</p>

# Cccc

一款用于 Windows 的剪贴板复制反馈工具。执行复制操作时，在屏幕指定位置显示非抢占焦点的提示窗口，用于确认复制结果、预览内容并记录连续复制次数。

<video src="docs/explorer_NWBIepwEBq.mp4" controls width="640">
  <a href="docs/explorer_NWBIepwEBq.mp4">示例视频</a>
</video>

## 功能特性

- **复制反馈与连续计数**：首次复制时淡入提示；连续复制时展示递增计数与 Material 3 Expressive 徽章形变动画，操作停止后自动淡出。
- **内容预览**：支持显示已复制的文本或文件摘要；不保存剪贴板历史，亦不上传任何剪贴板数据。
- **外观与个性化**：
  - 跟随 Windows 浅色/深色模式，支持 Acrylic 材质与不透明背景切换。
  - 提供普通卡片与任务栏单行（实验性）两种提示样式。
  - 支持在“个性化设置”中调整显示位置、偏移量、圆角半径、内边距、边框、阴影及字体族（默认使用 Noto Sans 系列回退链）。
- **多语言支持**：内置简体中文、繁体中文、英语、日语、韩语，支持跟随系统语言或手动切换。
- **系统托盘控制**：提供暂停提示、内容预览开关、高级材质开关、开机自启动、提示样式、语言切换、个性化设置及卸载入口。

## 运行环境

- **操作系统**：Windows x64（`net10.0-windows10.0.19041.0`）
- **运行时**：.NET 10 Desktop Runtime（x64）

## 构建与测试

使用仓库根目录的 `build.ps1` 构建项目并输出至 `artifacts/build/`：

```powershell
pwsh ./build.ps1
```

运行单元测试与架构边界检查：

```powershell
dotnet test Cccc.sln -c Debug --nologo
```

或通过构建脚本执行完整质量与表现层验证：

```powershell
pwsh ./build.ps1 -CheckQuality
```

打包为发布版 `.zip` 压缩包（默认使用 `Release` 配置，输出至 `artifacts/release/`）：

```powershell
pwsh ./build.ps1 -Package
```

> 附加 `-SelfContained` 可生成包含 .NET 运行时的独立发布包；`build.ps1` 另支持 `-CheckAnimation`、`-CheckTaskbar`、`-CheckMaterial` 与 `-CheckShape` 验证参数。构建完成后，直接运行 `artifacts/build/Cccc.exe`（需保留同目录依赖文件）即可使用。

## 卸载与清理

程序在运行目录之外仅写入以下两处用户级数据：

- 注册表自启动项：`HKCU\Software\Microsoft\Windows\CurrentVersion\Run` 下的 `Sandlada.Cccc`
- 配置目录：`%LocalAppData%\Sandlada.Cccc`

可通过以下任一方式完成清理（程序目录需手动删除）：

1. **托盘卸载**：右键系统托盘图标，选择“卸载 Cccc…”，确认后程序将移除自启动项与配置目录并自动退出。
2. **命令行卸载**：退出运行中的实例后执行 `Cccc.exe --uninstall`（返回码：`0` 表示成功，`1` 表示部分项目清理失败，`2` 表示仍有实例正在运行）。
3. **脚本清理**：执行 `pwsh ./build.ps1 -Uninstall` 停止运行中的实例并清理上述系统痕迹与 `artifacts/build/`；附加 `-Deep` 参数可一并清理各项目的 `bin` 与 `obj` 目录。

## 开源许可

本项目采用 [MIT License](LICENSE) 许可协议。
