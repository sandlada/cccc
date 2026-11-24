# Cccc 呈現驗證

`Cccc.Presentation.Verification` 包含離屏檢查與開發期 WPF 預覽工具。預覽工具只存在於這個專案，不是正式 `Cccc.exe` 的功能。

## WPF 實時預覽

```powershell
dotnet watch run `
  --project Cccc.Presentation.Verification/Cccc.Presentation.Verification.csproj `
  -- preview
```

啟動後會出現兩個視窗：

- **控制面板**：調整內容、動畫、主題、材質與版面。
- **實際卡片**：真正的 `ToastWindow`，會固定在面板旁並跟隨面板移動。

## 控制面板

| 分類 | 功能 |
| --- | --- |
| 內容 | 結果類型、自動／自訂標題、預覽文字、內容類型、複製次數 |
| 行為 | 模擬複製、自動連按、連按間隔、保持顯示、2 秒後淡出、隱藏、重設 |
| 外觀 | 卡片／任務欄樣式、跟隨系統／淺色／深色、Acrylic |
| 診斷 | DPI、尺寸、材質狀態、動畫狀態、徽章形狀與旋轉、文字準備時間、背景快取 |

`匯出 PNG…` 會用目前內容輸出 detached WPF 卡片，適合保存視覺檢查結果。原生 Acrylic 需要在實際桌面上取樣，PNG 只包含卡片本身。

## Hot Reload 範圍

- 控制面板操作會立即反映，不需要重新編譯。
- `dotnet watch` 可套用支援的 C# Hot Reload 編輯。
- 建構函式、欄位、型別結構等不支援的修改會重新啟動程式。
- 專案沒有 `.xaml`，因此不使用 XAML Hot Reload。

## 定位與 DPI

預覽卡片使用 `ToastWindow.PreviewPositionProvider`，由面板依目前監視器工作區計算位置。正式程式仍使用原本的左下角或任務欄定位。預覽會使用實際監視器 DPI；任意 DPI 不能只靠程式模擬。
