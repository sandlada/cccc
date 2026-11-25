using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using Cccc.Adapters.Outbound.Toast.Wpf;
using Cccc.Adapters.Outbound.Windows;
using Cccc.Application;
using Cccc.Domain.Feedback;
using Cccc.Platform.Windows;

namespace Cccc.Presentation.Verification
{
    internal sealed class PreviewWindow : Window
    {
        private enum PreviewThemeMode
        {
            System,
            Light,
            Dark
        }

        private static readonly Brush WindowBrush = CreateBrush("#101216");
        private static readonly Brush PanelBrush = CreateBrush("#191D24");
        private static readonly Brush InputBrush = CreateBrush("#101318");
        private static readonly Brush StrokeBrush = CreateBrush("#343B47");
        private static readonly Brush TextBrush = CreateBrush("#F1F4F8");
        private static readonly Brush MutedBrush = CreateBrush("#9AA5B4");
        private static readonly Brush AccentBrush = CreateBrush("#4DA3FF");
        private static readonly Brush AccentTextBrush = CreateBrush("#07121F");

        private readonly DispatcherTimer contentTimer;
        private readonly DispatcherTimer repeatTimer;
        private readonly DispatcherTimer diagnosticsTimer;

        private ComboBox kindBox;
        private ComboBox layoutBox;
        private ComboBox displayModeBox;
        private ComboBox themeBox;
        private ComboBox intervalBox;
        private TextBox titleBox;
        private TextBox previewBox;
        private TextBox typeBox;
        private TextBox diagnosticsBox;
        private CheckBox autoTitleCheck;
        private CheckBox repeatCheck;
        private CheckBox keepVisibleCheck;
        private CheckBox showPreviewCheck;
        private CheckBox materialCheck;
        private CheckBox fullCornerRadiusCheck;
        private CheckBox showBorderCheck;
        private CheckBox showShadowCheck;
        private CheckBox autoSaveCheck;
        private Slider countSlider;
        private Slider cornerRadiusSlider;
        private Slider paddingSlider;
        private TextBlock countLabel;
        private TextBlock cornerRadiusLabel;
        private TextBlock paddingLabel;
        private TextBlock stateLabel;
        private TextBlock statusLabel;
        private TextBlock outlineLabel;
        private Border previewOutline;

        private readonly FilePreferencesStore preferencesStore;
        private PreferencesModel currentPreferences;
        private ToastWindow toast;
        private int copyCount = 1;
        private bool updatingControls;
        private bool isClosed;

        internal PreviewWindow()
        {
            Title = "Cccc · WPF 實時預覽";
            MinWidth = 520;
            MinHeight = 480;
            Width = 720;
            Height = 640;
            WindowStartupLocation = WindowStartupLocation.Manual;
            Topmost = true;
            Background = WindowBrush;
            Foreground = TextBrush;
            FontFamily = new FontFamily("Segoe UI");
            FontSize = 13;

            contentTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
            contentTimer.Tick += delegate { contentTimer.Stop(); PresentCurrent(false); };
            repeatTimer = new DispatcherTimer();
            repeatTimer.Tick += delegate { SimulateCopy(); };
            diagnosticsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            diagnosticsTimer.Tick += delegate { UpdateDiagnostics(); };

            string settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Sandlada.Cccc", "settings-taskbar-experiment.txt");
            preferencesStore = new FilePreferencesStore(settingsPath, true);
            var loaded = preferencesStore.Load();
            currentPreferences = loaded.State == PreferencesLoadState.Loaded && loaded.Model != null
                ? loaded.Model
                : new PreferencesModel();

            PositionWindow();
            Content = CreateLayout();
            SyncControlsFromPreferences(currentPreferences);
            WireControls();

            SourceInitialized += delegate { PositionWindow(); };
            LocationChanged += delegate { RepositionToast(); };
            SizeChanged += delegate { UpdateOutline(); RepositionToast(); };
            Loaded += delegate
            {
                ApplyThemeSelection();
                CreateToast();
                PresentCurrent(false);
                statusLabel.Text = "預覽已啟動。";
                diagnosticsTimer.Start();
                UpdateDiagnostics();
            };
            Closed += delegate
            {
                isClosed = true;
                contentTimer.Stop();
                repeatTimer.Stop();
                diagnosticsTimer.Stop();
                CloseToast();
                ToastProfile.SetCompact(false);
            };
        }

        private void PositionWindow()
        {
            Rect work = SystemParameters.WorkArea;
            Width = Math.Min(720, Math.Max(520, work.Width - ToastWindow.OuterWidth - 72));
            Height = Math.Min(640, Math.Max(480, work.Height - 64));
            Left = work.Left + 24;
            Top = work.Top + Math.Max(16, (work.Height - Height) / 2);
        }

        private UIElement CreateLayout()
        {
            Grid root = new Grid { Background = WindowBrush };
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Border header = CreateHeader();
            Grid.SetRow(header, 0);
            root.Children.Add(header);

            Grid body = new Grid();
            body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(352) });
            body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(body, 1);
            root.Children.Add(body);

            Border controls = new Border
            {
                Background = PanelBrush,
                BorderBrush = StrokeBrush,
                BorderThickness = new Thickness(0, 0, 1, 0)
            };
            controls.Child = CreateControlsPanel();
            Grid.SetColumn(controls, 0);
            body.Children.Add(controls);

            Border stage = CreateStage();
            Grid.SetColumn(stage, 1);
            body.Children.Add(stage);

            return root;
        }

        private void WireControls()
        {
            kindBox.SelectionChanged += delegate
            {
                if (updatingControls) return;
                FeedbackViewKind kind = SelectedTag(kindBox, FeedbackViewKind.Confirmed);
                repeatCheck.IsEnabled = kind != FeedbackViewKind.Status;
                if (!repeatCheck.IsEnabled) repeatCheck.IsChecked = false;
                UpdateAutoTitle();
                UpdateRepeatTimer();
                QueueContentPreview();
            };
            layoutBox.SelectionChanged += delegate
            {
                if (updatingControls) return;
                repeatTimer.Stop();
                repeatCheck.IsChecked = false;
                CreateToast();
                PresentCurrent(false);
                statusLabel.Text = "已切換版面。";
            };
            themeBox.SelectionChanged += delegate
            {
                if (updatingControls) return;
                ApplyThemeSelection();
                statusLabel.Text = "已切換主題。";
            };
            intervalBox.SelectionChanged += delegate
            {
                if (updatingControls) return;
                UpdateRepeatTimer();
            };
            countSlider.ValueChanged += delegate
            {
                if (updatingControls) return;
                copyCount = (int)countSlider.Value;
                countLabel.Text = "×" + copyCount;
                if (autoTitleCheck.IsChecked == true)
                {
                    updatingControls = true;
                    titleBox.Text = DefaultTitle(SelectedTag(kindBox, FeedbackViewKind.Confirmed), copyCount);
                    updatingControls = false;
                }
                QueueContentPreview();
            };
            titleBox.TextChanged += delegate
            {
                if (autoTitleCheck.IsChecked != true) QueueContentPreview();
            };
            previewBox.TextChanged += delegate { QueueContentPreview(); };
            typeBox.TextChanged += delegate { QueueContentPreview(); };
            autoTitleCheck.Checked += delegate { UpdateAutoTitle(); QueueContentPreview(); };
            autoTitleCheck.Unchecked += delegate { UpdateAutoTitle(); QueueContentPreview(); };
            repeatCheck.Checked += delegate { UpdateRepeatTimer(); };
            repeatCheck.Unchecked += delegate { UpdateRepeatTimer(); };
            keepVisibleCheck.Checked += delegate
            {
                if (toast != null) toast.PreviewKeepVisible = true;
                statusLabel.Text = "已開啟保持顯示。";
            };
            keepVisibleCheck.Unchecked += delegate
            {
                if (toast != null)
                {
                    toast.PreviewKeepVisible = false;
                    PresentCurrent(false);
                }
                statusLabel.Text = "已恢復 2 秒自動淡出。";
            };
            showPreviewCheck.Checked += delegate { ApplyPersonalizationLive(); };
            showPreviewCheck.Unchecked += delegate { ApplyPersonalizationLive(); };
            materialCheck.Checked += delegate
            {
                if (toast == null) return;
                toast.SetMaterialEnabled(true);
                ApplyPersonalizationLive();
                statusLabel.Text = "Acrylic 已開啟。";
            };
            materialCheck.Unchecked += delegate
            {
                if (toast == null) return;
                toast.SetMaterialEnabled(false);
                ApplyPersonalizationLive();
                statusLabel.Text = "Acrylic 已關閉。";
            };
            displayModeBox.SelectionChanged += delegate { ApplyPersonalizationLive(); };
            cornerRadiusSlider.ValueChanged += delegate { ApplyPersonalizationLive(); };
            fullCornerRadiusCheck.Checked += delegate
            {
                cornerRadiusSlider.IsEnabled = false;
                ApplyPersonalizationLive();
            };
            fullCornerRadiusCheck.Unchecked += delegate
            {
                cornerRadiusSlider.IsEnabled = true;
                ApplyPersonalizationLive();
            };
            paddingSlider.ValueChanged += delegate { ApplyPersonalizationLive(); };
            showBorderCheck.Checked += delegate { ApplyPersonalizationLive(); };
            showBorderCheck.Unchecked += delegate { ApplyPersonalizationLive(); };
            showShadowCheck.Checked += delegate { ApplyPersonalizationLive(); };
            showShadowCheck.Unchecked += delegate { ApplyPersonalizationLive(); };
            layoutBox.SelectionChanged += delegate
            {
                if (updatingControls) return;
                if (autoSaveCheck.IsChecked == true) SavePreferencesToStore(false);
            };
        }

        private Border CreateHeader()
        {
            StackPanel title = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            title.Children.Add(new TextBlock
            {
                Text = "WPF 實時預覽",
                FontSize = 21,
                FontWeight = FontWeights.SemiBold,
                Foreground = TextBrush
            });
            TextBlock subtitle = new TextBlock
            {
                Text = "控制面板調整狀態，右側顯示真正的 ToastWindow。",
                Foreground = MutedBrush,
                Margin = new Thickness(0, 3, 0, 0)
            };
            title.Children.Add(subtitle);
            statusLabel = new TextBlock
            {
                Text = "準備中…",
                Foreground = AccentBrush,
                Margin = new Thickness(0, 3, 0, 0)
            };
            title.Children.Add(statusLabel);

            Button export = CreateButton("匯出 PNG…", delegate { ExportPreview(); }, false);
            export.MinWidth = 112;
            export.Margin = new Thickness(8, 0, 0, 0);
            DockPanel.SetDock(export, Dock.Right);

            DockPanel header = new DockPanel { LastChildFill = true };
            header.Children.Add(export);
            header.Children.Add(title);

            return new Border
            {
                Background = PanelBrush,
                BorderBrush = StrokeBrush,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(18, 14, 18, 14),
                Child = header
            };
        }

        private UIElement CreateControlsPanel()
        {
            kindBox = new ComboBox { MinHeight = 28 };
            AddChoice(kindBox, "已複製（確認成功）", FeedbackViewKind.Confirmed);
            AddChoice(kindBox, "保留原內容", FeedbackViewKind.Retained);
            AddChoice(kindBox, "未確認", FeedbackViewKind.Unconfirmed);
            AddChoice(kindBox, "剪貼板已更新", FeedbackViewKind.Updated);
            AddChoice(kindBox, "狀態提示", FeedbackViewKind.Status);
            kindBox.SelectedIndex = 0;

            layoutBox = new ComboBox { MinHeight = 28 };
            AddChoice(layoutBox, "卡片", LayoutMode.Card);
            AddChoice(layoutBox, "任務欄單行", LayoutMode.Taskbar);
            layoutBox.SelectedIndex = 0;

            displayModeBox = new ComboBox { MinHeight = 28 };
            AddChoice(displayModeBox, "圖示 + 標籤 + 複製內容（預設）", DisplayMode.Full);
            AddChoice(displayModeBox, "僅顯示圖示", DisplayMode.IconOnly);
            AddChoice(displayModeBox, "僅顯示圖示 + 標籤", DisplayMode.IconAndLabel);
            AddChoice(displayModeBox, "僅顯示圖示 + 複製內容", DisplayMode.IconAndContent);
            AddChoice(displayModeBox, "僅顯示標籤", DisplayMode.LabelOnly);
            AddChoice(displayModeBox, "僅顯示複製內容", DisplayMode.ContentOnly);
            AddChoice(displayModeBox, "僅顯示標籤 + 複製內容", DisplayMode.LabelAndContent);
            displayModeBox.SelectedIndex = 0;

            themeBox = new ComboBox { MinHeight = 28 };
            AddChoice(themeBox, "跟隨系統", PreviewThemeMode.System);
            AddChoice(themeBox, "淺色", PreviewThemeMode.Light);
            AddChoice(themeBox, "深色", PreviewThemeMode.Dark);
            themeBox.SelectedIndex = 0;

            intervalBox = new ComboBox { MinHeight = 28 };
            AddChoice(intervalBox, "100 ms", 100);
            AddChoice(intervalBox, "250 ms", 250);
            AddChoice(intervalBox, "500 ms", 500);
            AddChoice(intervalBox, "1000 ms", 1000);
            intervalBox.SelectedIndex = 2;

            titleBox = CreateTextBox(false);
            titleBox.Text = DefaultTitle(FeedbackViewKind.Confirmed, copyCount);
            titleBox.IsEnabled = false;
            previewBox = CreateTextBox(true);
            previewBox.Text = "這是一段即時預覽文字，可以直接在這裡修改。";
            typeBox = CreateTextBox(false);
            typeBox.Text = "文字";

            autoTitleCheck = CreateCheckBox("自動使用對應標題", true);
            repeatCheck = CreateCheckBox("自動連按", false);
            keepVisibleCheck = CreateCheckBox("保持顯示（不自動淡出）", true);
            showPreviewCheck = CreateCheckBox("顯示內容預覽", true);
            materialCheck = CreateCheckBox("啟用 Acrylic 材質", true);
            fullCornerRadiusCheck = CreateCheckBox("滿圓（高度 / 2）", false);
            showBorderCheck = CreateCheckBox("顯示邊框 (Border)", true);
            showShadowCheck = CreateCheckBox("顯示陰影 (Shadow)", true);
            autoSaveCheck = CreateCheckBox("即時同步至 Cccc 設定檔", true);

            cornerRadiusSlider = new Slider
            {
                Minimum = AppearanceBounds.MinCornerRadius,
                Maximum = AppearanceBounds.MaxCornerRadius,
                Value = AppearanceBounds.DefaultCornerRadius,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 4, 0, 0)
            };
            cornerRadiusLabel = new TextBlock
            {
                Text = AppearanceBounds.DefaultCornerRadius + " px",
                Foreground = TextBrush,
                FontWeight = FontWeights.SemiBold,
                FontFamily = new FontFamily("Consolas"),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            paddingSlider = new Slider
            {
                Minimum = AppearanceBounds.MinPadding,
                Maximum = AppearanceBounds.MaxPadding,
                Value = AppearanceBounds.DefaultPadding,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 4, 0, 0)
            };
            paddingLabel = new TextBlock
            {
                Text = AppearanceBounds.DefaultPadding + " px",
                Foreground = TextBrush,
                FontWeight = FontWeights.SemiBold,
                FontFamily = new FontFamily("Consolas"),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            countSlider = new Slider
            {
                Minimum = 1,
                Maximum = 120,
                Value = copyCount,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 4, 0, 0)
            };
            countLabel = new TextBlock
            {
                Text = "×" + copyCount,
                Foreground = TextBrush,
                FontWeight = FontWeights.SemiBold,
                FontFamily = new FontFamily("Consolas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 0, 2)
            };

            StackPanel content = new StackPanel();
            content.Children.Add(Label("結果類型"));
            content.Children.Add(kindBox);
            content.Children.Add(autoTitleCheck);
            content.Children.Add(Label("標題"));
            content.Children.Add(titleBox);
            content.Children.Add(Label("預覽文字"));
            content.Children.Add(previewBox);
            content.Children.Add(Label("內容類型文字"));
            content.Children.Add(typeBox);
            Grid countHeader = new Grid { Margin = new Thickness(0, 8, 0, 0) };
            countHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            countHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            TextBlock countTitle = Label("複製次數");
            countTitle.Margin = new Thickness(0, 0, 0, 0);
            countTitle.VerticalAlignment = VerticalAlignment.Center;
            countHeader.Children.Add(countTitle);
            countLabel.Margin = new Thickness(0, 0, 0, 0);
            countLabel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(countLabel, 1);
            countHeader.Children.Add(countLabel);
            content.Children.Add(countHeader);
            content.Children.Add(countSlider);

            StackPanel behaviour = new StackPanel();
            WrapPanel actions = new WrapPanel { Margin = new Thickness(0, 0, 0, 4) };
            actions.Children.Add(CreateButton("模擬複製", delegate { SimulateCopy(); }, true));
            actions.Children.Add(CreateButton("隱藏", delegate
            {
                if (toast == null) return;
                toast.HideAndClear();
                statusLabel.Text = "卡片已隱藏。";
                UpdateState();
            }, false));
            actions.Children.Add(CreateButton("重設", delegate
            {
                copyCount = 1;
                UpdateCountUi();
                if (toast != null) toast.HideAndClear();
                PresentCurrent(false);
                statusLabel.Text = "已重設為第 1 次複製。";
            }, false));
            behaviour.Children.Add(actions);
            behaviour.Children.Add(repeatCheck);
            behaviour.Children.Add(Label("自動連按間隔"));
            behaviour.Children.Add(intervalBox);
            behaviour.Children.Add(keepVisibleCheck);
            behaviour.Children.Add(showPreviewCheck);

            StackPanel appearance = new StackPanel();
            appearance.Children.Add(Label("版面"));
            appearance.Children.Add(layoutBox);
            appearance.Children.Add(Label("顯示模式"));
            appearance.Children.Add(displayModeBox);
            appearance.Children.Add(Label("主題"));
            appearance.Children.Add(themeBox);

            Grid radiusHeader = new Grid { Margin = new Thickness(0, 8, 0, 0) };
            radiusHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            radiusHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            TextBlock radiusTitle = Label("圓角半徑");
            radiusTitle.Margin = new Thickness(0);
            radiusTitle.VerticalAlignment = VerticalAlignment.Center;
            radiusHeader.Children.Add(radiusTitle);
            Grid.SetColumn(cornerRadiusLabel, 1);
            radiusHeader.Children.Add(cornerRadiusLabel);
            appearance.Children.Add(radiusHeader);
            appearance.Children.Add(cornerRadiusSlider);
            appearance.Children.Add(fullCornerRadiusCheck);

            Grid padHeader = new Grid { Margin = new Thickness(0, 8, 0, 0) };
            padHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            padHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            TextBlock padTitle = Label("內邊距 (Padding)");
            padTitle.Margin = new Thickness(0);
            padTitle.VerticalAlignment = VerticalAlignment.Center;
            padHeader.Children.Add(padTitle);
            Grid.SetColumn(paddingLabel, 1);
            padHeader.Children.Add(paddingLabel);
            appearance.Children.Add(padHeader);
            appearance.Children.Add(paddingSlider);

            appearance.Children.Add(showBorderCheck);
            appearance.Children.Add(showShadowCheck);
            appearance.Children.Add(materialCheck);
            appearance.Children.Add(autoSaveCheck);

            WrapPanel settingsActions = new WrapPanel { Margin = new Thickness(0, 8, 0, 0) };
            settingsActions.Children.Add(CreateButton("儲存為設定", delegate { SavePreferencesToStore(true); }, true));
            settingsActions.Children.Add(CreateButton("恢復預設外觀", delegate
            {
                SyncControlsFromPreferences(new PreferencesModel());
                ApplyPersonalizationLive();
                statusLabel.Text = "已恢復預設外觀設定。";
            }, false));
            appearance.Children.Add(settingsActions);

            TextBlock materialNote = new TextBlock
            {
                Text = "任務欄樣式不使用 Acrylic；系統高對比或關閉透明效果時也會停用。",
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 6, 0, 0)
            };
            appearance.Children.Add(materialNote);

            StackPanel root = new StackPanel();
            root.Children.Add(CreateSection("內容", content));
            root.Children.Add(CreateSection("行為", behaviour));
            root.Children.Add(CreateSection("外觀", appearance));

            return new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Padding = new Thickness(14),
                Content = root
            };
        }

        private Border CreateStage()
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            StackPanel heading = new StackPanel();
            heading.Children.Add(new TextBlock
            {
                Text = "實際卡片",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = TextBrush
            });
            heading.Children.Add(new TextBlock
            {
                Text = "卡片是獨立的懸浮視窗，會跟隨這個面板移動；它不是控制面板內的截圖。",
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0)
            });
            Grid.SetRow(heading, 0);
            grid.Children.Add(heading);

            previewOutline = new Border
            {
                BorderBrush = StrokeBrush,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(14),
                Background = CreateBrush("#0D0F13"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = CreateOutlineContent()
            };
            outlineLabel = (TextBlock)((StackPanel)previewOutline.Child).Children[1];
            Viewbox outlineView = new Viewbox
            {
                Stretch = Stretch.Uniform,
                StretchDirection = StretchDirection.DownOnly,
                Margin = new Thickness(12, 20, 12, 20),
                Child = previewOutline
            };
            Grid.SetRow(outlineView, 1);
            grid.Children.Add(outlineView);

            StackPanel footer = new StackPanel();
            stateLabel = new TextBlock
            {
                Text = "尚未建立卡片",
                Foreground = AccentBrush,
                Margin = new Thickness(0, 0, 0, 8)
            };
            footer.Children.Add(stateLabel);

            diagnosticsBox = new TextBox
            {
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.NoWrap,
                Height = 162,
                MinHeight = 120,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Background = InputBrush,
                Foreground = TextBrush,
                BorderBrush = StrokeBrush,
                Padding = new Thickness(8)
            };
            footer.Children.Add(CreateSection("診斷", diagnosticsBox));
            Grid.SetRow(footer, 2);
            grid.Children.Add(footer);

            return new Border
            {
                Background = PanelBrush,
                BorderBrush = StrokeBrush,
                BorderThickness = new Thickness(0),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(12),
                Padding = new Thickness(16),
                Child = grid
            };
        }

        private StackPanel CreateOutlineContent()
        {
            StackPanel content = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            content.Children.Add(new TextBlock
            {
                Text = "↗",
                FontSize = 30,
                Foreground = AccentBrush,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            content.Children.Add(new TextBlock
            {
                Text = "實際卡片在面板旁",
                Foreground = TextBrush,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                TextAlignment = TextAlignment.Center
            });
            content.Children.Add(new TextBlock
            {
                Text = "這裡顯示設計尺寸，真正內容由 WPF 即時繪製。",
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                MaxWidth = 240,
                Margin = new Thickness(0, 5, 0, 0)
            });
            return content;
        }

        private void CreateToast()
        {
            CloseToast();
            LayoutMode layout = SelectedTag(layoutBox, LayoutMode.Card);
            ToastProfile.SetCompact(layout == LayoutMode.Taskbar);
            Themes.MaterialAvailable = false;
            toast = new ToastWindow(materialCheck.IsChecked == true);
            toast.ApplyPersonalization(BuildPreferencesFromControls());
            toast.PreviewKeepVisible = keepVisibleCheck.IsChecked == true;
            toast.PreviewPositionProvider = CalculateToastPosition;
            UpdateMaterialAvailability();
            UpdateOutline();
        }

        private void SyncControlsFromPreferences(PreferencesModel model)
        {
            updatingControls = true;
            try
            {
                SelectByTag(layoutBox, model.Layout);
                SelectByTag(displayModeBox, model.DisplayMode);
                showPreviewCheck.IsChecked = model.ShowPreview;
                materialCheck.IsChecked = model.UseMaterial;
                double maxRadius = Math.Max(1, Math.Floor(AppearanceBounds.ComputeCardHeight(model.DisplayMode, model.Padding) / 2.0));
                cornerRadiusSlider.Maximum = maxRadius;
                cornerRadiusSlider.Value = model.FullCornerRadius ? maxRadius : Math.Min(model.CornerRadius, maxRadius);
                fullCornerRadiusCheck.IsChecked = model.FullCornerRadius;
                cornerRadiusSlider.IsEnabled = !model.FullCornerRadius;
                cornerRadiusLabel.Text = model.FullCornerRadius || cornerRadiusSlider.Value >= maxRadius
                    ? $"滿圓 ({maxRadius:0} px)"
                    : model.CornerRadius + " px";
                paddingSlider.Value = model.Padding;
                paddingLabel.Text = model.Padding + " px";
                showBorderCheck.IsChecked = model.ShowBorder;
                showShadowCheck.IsChecked = model.ShowShadow;
            }
            finally { updatingControls = false; }
        }

        private static void SelectByTag<T>(ComboBox box, T value)
        {
            for (int i = 0; i < box.Items.Count; i++)
            {
                if (box.Items[i] is ComboBoxItem item && item.Tag is T tag && EqualityComparer<T>.Default.Equals(tag, value))
                {
                    box.SelectedIndex = i;
                    return;
                }
            }
        }

        private PreferencesModel BuildPreferencesFromControls()
        {
            return new PreferencesModel
            {
                Layout = SelectedTag(layoutBox, LayoutMode.Card),
                DisplayMode = SelectedTag(displayModeBox, DisplayMode.Full),
                ShowPreview = showPreviewCheck.IsChecked == true,
                UseMaterial = materialCheck.IsChecked == true,
                CornerRadius = (int)Math.Round(cornerRadiusSlider.Value),
                FullCornerRadius = fullCornerRadiusCheck.IsChecked == true,
                Padding = (int)Math.Round(paddingSlider.Value),
                ShowBorder = showBorderCheck.IsChecked == true,
                ShowShadow = showShadowCheck.IsChecked == true
            };
        }

        private void ApplyPersonalizationLive()
        {
            if (updatingControls || isClosed) return;
            currentPreferences = BuildPreferencesFromControls();
            double maxRadius = Math.Max(1, Math.Floor(AppearanceBounds.ComputeCardHeight(currentPreferences.DisplayMode, currentPreferences.Padding) / 2.0));
            updatingControls = true;
            try
            {
                cornerRadiusSlider.Maximum = maxRadius;
                if (currentPreferences.FullCornerRadius)
                    cornerRadiusSlider.Value = maxRadius;
                else if (cornerRadiusSlider.Value > maxRadius)
                {
                    cornerRadiusSlider.Value = maxRadius;
                    currentPreferences.CornerRadius = (int)maxRadius;
                }
            }
            finally { updatingControls = false; }
            cornerRadiusLabel.Text = currentPreferences.FullCornerRadius || currentPreferences.CornerRadius >= maxRadius
                ? $"滿圓 ({maxRadius:0} px)"
                : currentPreferences.CornerRadius + " px";
            paddingLabel.Text = currentPreferences.Padding + " px";
            if (toast != null)
            {
                toast.ApplyPersonalization(currentPreferences);
                toast.RefreshAppearance();
                PresentCurrent(false);
                UpdateOutline();
            }
            if (autoSaveCheck.IsChecked == true) SavePreferencesToStore(false);
        }

        private void SavePreferencesToStore(bool announce)
        {
            currentPreferences = BuildPreferencesFromControls();
            PreferencesSaveResult result = preferencesStore.Save(currentPreferences);
            if (announce)
            {
                statusLabel.Text = result.Success
                    ? "已儲存至 Cccc 設定檔。"
                    : "儲存失敗：" + result.Error;
            }
        }

        private void CloseToast()
        {
            if (toast == null) return;
            toast.PreviewPositionProvider = null;
            toast.PreviewKeepVisible = false;
            toast.HideAndClear();
            toast.Close();
            toast = null;
        }

        private void PresentCurrent(bool simulateCopy)
        {
            if (toast == null) return;
            FeedbackViewKind kind = SelectedTag(kindBox, FeedbackViewKind.Confirmed);
            bool preview = showPreviewCheck.IsChecked == true;
            if (simulateCopy && kind != FeedbackViewKind.Status)
            {
                copyCount = copyCount >= 120 ? 1 : copyCount + 1;
                UpdateCountUi();
                toast.CopyPressed(copyCount, HostHandle(), preview);
            }
            toast.Present(BuildSample(), preview);
            UpdateOutline();
            UpdateState();
        }

        private void SimulateCopy()
        {
            PresentCurrent(true);
        }

        private FeedbackView BuildSample()
        {
            FeedbackViewKind kind = SelectedTag(kindBox, FeedbackViewKind.Confirmed);
            int count = kind == FeedbackViewKind.Status ? 0 : copyCount;
            string title = autoTitleCheck.IsChecked == true
                ? DefaultTitle(kind, count)
                : titleBox.Text;
            return new FeedbackView
            {
                Kind = kind,
                Title = title,
                Preview = previewBox.Text,
                TypeLabel = typeBox.Text,
                Count = count,
                Foreground = HostTarget()
            };
        }

        private static string DefaultTitle(FeedbackViewKind kind, int count)
        {
            return kind switch
            {
                FeedbackViewKind.Confirmed => FeedbackEngine.SuccessTitle(count),
                FeedbackViewKind.Retained => "刚才的内容仍在剪贴板",
                FeedbackViewKind.Unconfirmed => "未检测到新的复制结果",
                FeedbackViewKind.Updated => "剪贴板已更新",
                FeedbackViewKind.Status => "Cccc 預覽",
                _ => "已复制"
            };
        }

        private void UpdateCountUi()
        {
            updatingControls = true;
            copyCount = Math.Min(120, Math.Max(1, copyCount));
            countSlider.Value = copyCount;
            countLabel.Text = "×" + copyCount;
            if (autoTitleCheck.IsChecked == true)
            {
                FeedbackViewKind kind = SelectedTag(kindBox, FeedbackViewKind.Confirmed);
                string title = DefaultTitle(kind, copyCount);
                if (titleBox.Text != title) titleBox.Text = title;
            }
            updatingControls = false;
        }

        private void UpdateAutoTitle()
        {
            bool auto = autoTitleCheck.IsChecked == true;
            titleBox.IsEnabled = !auto;
            if (auto) UpdateCountUi();
        }

        private void QueueContentPreview()
        {
            if (updatingControls || isClosed) return;
            contentTimer.Stop();
            contentTimer.Start();
        }

        private void UpdateRepeatTimer()
        {
            repeatTimer.Stop();
            if (repeatCheck.IsChecked != true || isClosed) return;
            if (SelectedTag(kindBox, FeedbackViewKind.Confirmed) == FeedbackViewKind.Status) return;
            int milliseconds = SelectedTag(intervalBox, 500);
            repeatTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(50, milliseconds));
            repeatTimer.Start();
        }

        private void ApplyThemeSelection()
        {
            if (isClosed) return;
            PreviewThemeMode mode = SelectedTag(themeBox, PreviewThemeMode.System);
            if (mode == PreviewThemeMode.System)
            {
                Themes.Refresh();
            }
            else
            {
                Themes.Current = new Appearance
                {
                    Dark = mode == PreviewThemeMode.Dark,
                    Transparent = !SystemParameters.HighContrast
                };
            }
            UpdateMaterialAvailability();
            if (toast != null)
            {
                toast.RefreshAppearance();
                UpdateState();
            }
        }

        private void UpdateMaterialAvailability()
        {
            LayoutMode layout = SelectedTag(layoutBox, LayoutMode.Card);
            bool enabled = layout == LayoutMode.Card && Themes.Current.Transparent;
            materialCheck.IsEnabled = enabled;
            materialCheck.ToolTip = enabled
                ? "使用 Windows 原生 Acrylic 背景。"
                : "任務欄樣式、高對比或關閉透明效果時不會使用 Acrylic。";
        }

        private void UpdateOutline()
        {
            if (previewOutline == null) return;
            double w = toast != null ? toast.ActiveOuterWidth : ToastWindow.OuterWidth;
            double h = toast != null ? toast.ActiveOuterHeight : ToastWindow.OuterHeight;
            previewOutline.Width = w;
            previewOutline.Height = h;
            if (outlineLabel != null)
            {
                outlineLabel.Text = "實際卡片在面板旁 · " + Math.Round(w) + " × " +
                    Math.Round(h) + " DIP";
            }
        }

        private void RepositionToast()
        {
            if (toast == null || isClosed) return;
            IntPtr handle = new WindowInteropHelper(this).Handle;
            if (handle != IntPtr.Zero) toast.Position(handle);
        }

        private Point? CalculateToastPosition(Size toastSize)
        {
            IntPtr host = new WindowInteropHelper(this).Handle;
            if (host == IntPtr.Zero) return null;
            WindowsNativeMethods.WindowRectangle bounds;
            if (!WindowsNativeMethods.GetWindowRect(host, out bounds)) return null;
            int dpi = WindowsNativeMethods.GetWindowDpi(host);
            if (dpi <= 0) dpi = 96;
            double scale = dpi / 96.0;
            double left = bounds.Left / scale;
            double top = bounds.Top / scale;
            double width = (bounds.Right - bounds.Left) / scale;
            double height = (bounds.Bottom - bounds.Top) / scale;

            IntPtr monitor = WindowsNativeMethods.MonitorFromWindow(host, 2);
            WindowsNativeMethods.MonitorInformation information = new WindowsNativeMethods.MonitorInformation
            {
                Size = (uint)Marshal.SizeOf(typeof(WindowsNativeMethods.MonitorInformation))
            };
            double workLeft = 0, workTop = 0, workRight = left + width, workBottom = top + height;
            if (WindowsNativeMethods.GetMonitorInfo(monitor, ref information))
            {
                workLeft = information.Work.Left / scale;
                workTop = information.Work.Top / scale;
                workRight = information.Work.Right / scale;
                workBottom = information.Work.Bottom / scale;
            }

            const double gap = 20;
            double x = left + width + gap;
            if (x + toastSize.Width > workRight) x = left - toastSize.Width - gap;
            if (x < workLeft) x = workLeft;
            if (x + toastSize.Width > workRight) x = Math.Max(workLeft, workRight - toastSize.Width);
            double y = top + (height - toastSize.Height) / 2;
            if (y < workTop) y = workTop;
            if (y + toastSize.Height > workBottom) y = Math.Max(workTop, workBottom - toastSize.Height);
            return new Point(x, y);
        }

        private IntPtr HostHandle()
        {
            return new WindowInteropHelper(this).Handle;
        }

        private WindowTarget HostTarget()
        {
            return new WindowTarget(HostHandle().ToInt64(), (uint)Environment.ProcessId);
        }

        private void UpdateState()
        {
            if (stateLabel == null) return;
            if (toast == null)
            {
                stateLabel.Text = "尚未建立卡片";
                return;
            }
            LayoutMode layout = SelectedTag(layoutBox, LayoutMode.Card);
            FeedbackViewKind kind = SelectedTag(kindBox, FeedbackViewKind.Confirmed);
            string visibility = toast.IsVisible ? "顯示中" : "已隱藏";
            stateLabel.Text = (layout == LayoutMode.Card ? "卡片" : "任務欄") + " · " + KindName(kind) +
                " · ×" + copyCount + " · " + visibility;
        }

        private void UpdateDiagnostics()
        {
            if (isClosed || diagnosticsBox == null) return;
            if (toast == null)
            {
                diagnosticsBox.Text = "尚未建立卡片";
                return;
            }
            int dpi = 96;
            try
            {
                IntPtr handle = toast.Handle;
                if (handle != IntPtr.Zero) dpi = WindowsNativeMethods.GetWindowDpi(handle);
            }
            catch (Exception error)
            {
                diagnosticsBox.Text = "診斷失敗：" + error.Message;
                return;
            }
            string material = toast.HasNativeMaterial
                ? "Acrylic"
                : Themes.MaterialEnabled
                    ? (toast.MaterialFailure == null ? "Opaque" : "Fallback: " + toast.MaterialFailure)
                    : "關閉";
            List<string> lines = new List<string>
            {
                "DPI              " + dpi,
                "尺寸             " + ToastWindow.OuterWidth + " × " + ToastWindow.OuterHeight + " DIP",
                "版面             " + (ToastProfile.Compact ? "任務欄" : "卡片"),
                "材質             " + material,
                "材質可用         " + (Themes.MaterialAvailable ? "是" : "否"),
                "可見             " + (toast.IsVisible ? "是" : "否") + " / opacity " + toast.Opacity.ToString("F2"),
                "動畫執行         " + (toast.TestFramesRunning ? "是" : "否"),
                "徽章形狀         " + toast.TestBadgeShape,
                "徽章旋轉         " + toast.TestBadgeRotation.ToString("F1") + "°",
                "徽章自轉         " + toast.TestBadgeSpinAngle.ToString("F1") + "°",
                "徽章動畫         " + (toast.TestBadgeMotionRunning ? "執行中" : "靜止"),
                "文字準備         " + toast.LastPreparationMilliseconds.ToString("F2") + " ms",
                "背景建置         " + toast.BackgroundBuildCount + " 次 / 快取 " + toast.BackgroundCacheCount
            };
            diagnosticsBox.Text = string.Join(Environment.NewLine, lines);
            UpdateState();
        }

        private void ExportPreview()
        {
            if (toast == null) return;
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "匯出 Cccc 預覽",
                Filter = "PNG 影像 (*.png)|*.png",
                DefaultExt = ".png",
                AddExtension = true,
                OverwritePrompt = true,
                FileName = "Cccc-preview.png"
            };
            if (dialog.ShowDialog(this) != true) return;
            try
            {
                toast.RenderSample(dialog.FileName, BuildSample(), showPreviewCheck.IsChecked == true);
                statusLabel.Text = "已匯出：" + dialog.FileName;
            }
            catch (Exception error)
            {
                MessageBox.Show(this, error.Message, "匯出失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string KindName(FeedbackViewKind kind)
        {
            return kind switch
            {
                FeedbackViewKind.Confirmed => "確認成功",
                FeedbackViewKind.Retained => "保留原內容",
                FeedbackViewKind.Unconfirmed => "未確認",
                FeedbackViewKind.Updated => "已更新",
                FeedbackViewKind.Status => "狀態提示",
                _ => kind.ToString()
            };
        }

        private static void AddChoice(ComboBox box, string label, object value)
        {
            box.Items.Add(new ComboBoxItem { Content = label, Tag = value });
        }

        private static T SelectedTag<T>(ComboBox box, T fallback)
        {
            if (box != null && box.SelectedItem is ComboBoxItem item && item.Tag is T value) return value;
            return fallback;
        }

        private static TextBlock Label(string text)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = MutedBrush,
                Margin = new Thickness(0, 8, 0, 4)
            };
        }

        private static CheckBox CreateCheckBox(string text, bool value)
        {
            return new CheckBox
            {
                Content = text,
                IsChecked = value,
                Foreground = TextBrush,
                Margin = new Thickness(0, 7, 0, 0)
            };
        }

        private static TextBox CreateTextBox(bool multiline)
        {
            return new TextBox
            {
                AcceptsReturn = multiline,
                TextWrapping = multiline ? TextWrapping.Wrap : TextWrapping.NoWrap,
                Height = multiline ? 72 : Double.NaN,
                MinHeight = 28,
                VerticalContentAlignment = multiline ? VerticalAlignment.Top : VerticalAlignment.Center,
                VerticalScrollBarVisibility = multiline ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden,
                Background = InputBrush,
                Foreground = TextBrush,
                BorderBrush = StrokeBrush,
                CaretBrush = TextBrush,
                SelectionBrush = AccentBrush,
                Padding = new Thickness(6, 4, 6, 4)
            };
        }

        private static Button CreateButton(string text, RoutedEventHandler click, bool primary)
        {
            Button button = new Button
            {
                Content = text,
                Padding = new Thickness(11, 5, 11, 5),
                Margin = new Thickness(0, 0, 8, 8),
                MinWidth = 84
            };
            button.Click += click;
            if (primary)
            {
                button.Background = AccentBrush;
                button.Foreground = AccentTextBrush;
                button.BorderBrush = AccentBrush;
                button.FontWeight = FontWeights.SemiBold;
            }
            return button;
        }

        private static Border CreateSection(string title, UIElement content)
        {
            StackPanel stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = TextBrush,
                Margin = new Thickness(0, 0, 0, 4)
            });
            stack.Children.Add(content);
            return new Border
            {
                Background = WindowBrush,
                BorderBrush = StrokeBrush,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 12),
                Child = stack
            };
        }

        private static Brush CreateBrush(string value)
        {
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
            brush.Freeze();
            return brush;
        }
    }
}
