# Cccc 命名規範

本專案採用統一的 C#／.NET 命名規則。新程式碼與重命名都應遵守以下條款。

## 品牌前綴

- 一律使用 `Cccc`。
- 三個字母以上的縮寫只保留首字母大寫，例如 `Wpf`、`Dpi`、`Png`、`Abi`。
- 不再使用四字母全大寫形式；專案、命名空間、組件、資料夾、檔案與使用者可見文字都使用 `Cccc`。

## 專案與命名空間

- 格式：`Cccc.<架構角色>.<平台或技術>`。
- 架構角色使用完整名稱：`Domain`、`Application`、`Adapters`、`Platform`、`Desktop`、`Presentation`。
- 測試專案：`Cccc.<被測專案>.Tests`。
- 離屏驗證程式：`Cccc.Presentation.Verification`。

目前專案：

```text
Cccc.Domain
Cccc.Application
Cccc.Platform.Windows
Cccc.Adapters.Inbound.Desktop
Cccc.Adapters.Outbound.Windows
Cccc.Adapters.Outbound.Toast.Wpf
Cccc.MaterialShape
Cccc.Desktop
Cccc.Domain.Tests
Cccc.Application.Tests
Cccc.Adapters.Outbound.Windows.Tests
Cccc.Architecture.Tests
Cccc.MaterialShape.Tests
Cccc.Presentation.Verification
```

## 型別與成員

- 使用完整英文單詞，不使用任意縮寫。
- 方法採「動詞 + 對象 + 上下文」：
  - `GetProcessIdForWindow`
  - `SetWindowOwner`
  - `GetExtendedWindowStyle`
  - `EstimateAnimationDurationMilliseconds`
- 介面使用 `I` + 名詞或能力：`IMeasurer`、`IClipboardReader`。
- 非同步方法以 `Async` 結尾。
- 布林值以 `Is`、`Has`、`Can` 或 `Should` 開頭，例如 `IsKeyDown`、`IsConvex`。

以下縮寫與模糊名稱不再使用：

| 避免 | 改用 |
|---|---|
| `App` | `Desktop`、`Application` |
| `Info` | `Information` |
| `Num` | `Count` |
| `Verts` | `Vertices` |
| `Millis` | `Milliseconds` |
| `Ex` | `Extended` |
| `Ish` | `Approximately` |
| `Native` | `WindowsNativeMethods` 或具體平台名稱 |
| `Rect` | `WindowRectangle` |
| `Down` | `IsKeyDown` |
| `Helper` | 具體的 Builder、Factory 或 Mapper |
| `Util` / `Common` / `Core` | 具體領域名稱 |

## 檔案

- 檔名等於主要型別名稱，例如 `DesktopHost.cs`、`ISystemClock.cs`。
- 一個公開型別一個檔案。
- 不再使用 `Models.cs`、`Fakes.cs` 這類泛稱檔名；相關型別已拆成獨立檔案。
- `Program.cs` 與組件屬性檔案是合理的例外。

## 例外

- P/Invoke 的 Win32 函式名稱保留平台原名，例如 `GetWindowRect`、`SetWindowPos`；我們自己的包裝方法仍使用完整名稱。
- 官方技術名稱保留官方拼寫，例如 `Wpf`、`Toast`、`Acrylic`、`Abi`。
- 外部 URL、上游專案名稱與 GitHub 倉庫路徑不隨本專案改名。

## 測試命名

- xUnit 測試方法使用可讀的完整句子，例如 `Verified_copy_is_confirmed`。
- 測試替身使用具體名稱：`FakeClipboardReader`、`FakeSystemClock`、`RecordingFeedbackPresenter`。
- 架構測試檔名為 `ArchitectureLayerTests.cs`，類別為 `ArchitectureLayerTests`。
