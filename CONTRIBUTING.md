# Contributing

## 环境

- Windows x64
- .NET 10 SDK 或更新版本
- PowerShell 7
- 内嵌字体位于 `assets/fonts/`
- `preview/shape` 另需 Node.js、Python

## 构建

```powershell
pwsh ./build.ps1
```

发布构建：

```powershell
pwsh ./build.ps1 -Configuration Release
```

输出目录：`artifacts/build/`，程序为 `Cccc.exe`。

## 测试

```powershell
dotnet test Cccc.sln -c Debug --nologo
```

或使用构建脚本：

```powershell
pwsh ./build.ps1 -CheckQuality
```

## 呈现检查

```powershell
pwsh ./build.ps1 -CheckAnimation
pwsh ./build.ps1 -CheckTaskbar
pwsh ./build.ps1 -CheckMaterial
pwsh ./build.ps1 -CheckShape
```

全部运行：

```powershell
pwsh ./build.ps1 -CheckQuality -CheckAnimation -CheckTaskbar -CheckMaterial -CheckShape
```

## WPF 实时预览

```powershell
dotnet watch run `
  --project Cccc.Presentation.Verification/Cccc.Presentation.Verification.csproj `
  -- preview
```

说明见 `Cccc.Presentation.Verification/README.md`。

## 形状预览

```powershell
cd preview/shape
npm install --no-audit --no-fund
npm run build
npm run serve
```

打开 <http://127.0.0.1:8792/>。

## 清理

移除开机启动项、设置和构建输出：

```powershell
pwsh ./build.ps1 -Uninstall
```

同时清理 `bin`、`obj`：

```powershell
pwsh ./build.ps1 -Uninstall -Deep
```

## 提交前

- 构建通过。
- `dotnet test` 通过。
- 相关呈现检查通过。
- 不提交 `bin/`、`obj/`、`artifacts/`、`node_modules/`。
