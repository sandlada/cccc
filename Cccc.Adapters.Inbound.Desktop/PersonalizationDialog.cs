using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Inbound.Desktop
{
    internal sealed class PersonalizationDialog
    {
        private static Window activeDialog;

        private static readonly (DisplayMode Mode, string Label)[] DisplayModes =
        {
            (DisplayMode.Full, "图标 + 标签 + 复制内容（默认）"),
            (DisplayMode.IconOnly, "仅显示图标"),
            (DisplayMode.IconAndLabel, "仅显示图标 + 标签"),
            (DisplayMode.IconAndContent, "仅显示图标 + 复制内容"),
            (DisplayMode.LabelOnly, "仅显示标签"),
            (DisplayMode.ContentOnly, "仅显示复制内容"),
            (DisplayMode.LabelAndContent, "仅显示标签 + 复制内容")
        };

        internal static void ShowDialogOrActivate(
            PreferencesService preferences,
            ChangePreferencesHandler changePreferences,
            ShowStatusHandler status,
            Func<bool> isDark,
            Action<PreferencesModel> applyPreferences,
            Action<PreferencesModel> refreshTray)
        {
            if (activeDialog != null)
            {
                if (activeDialog.WindowState == WindowState.Minimized)
                    activeDialog.WindowState = WindowState.Normal;
                activeDialog.Activate();
                return;
            }

            bool dark = isDark();
            bool highContrast = SystemParameters.HighContrast;
            Brush background = highContrast ? SystemColors.WindowBrush :
                new SolidColorBrush(dark ? Color.FromRgb(43, 43, 43) : Colors.White);
            Brush foreground = highContrast ? SystemColors.WindowTextBrush :
                new SolidColorBrush(dark ? Colors.White : Color.FromRgb(26, 26, 26));
            Brush subtle = highContrast ? SystemColors.GrayTextBrush :
                new SolidColorBrush(dark ? Color.FromRgb(174, 174, 178) : Color.FromRgb(110, 110, 115));

            Window dialog = new Window
            {
                Title = "Cccc · 个性化设置",
                Width = 420,
                SizeToContent = SizeToContent.Height,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = background,
                Foreground = foreground,
                FontSize = 13,
                FontFamily = SystemFonts.MessageFontFamily,
                ShowInTaskbar = true
            };
            activeDialog = dialog;
            dialog.Closed += delegate { activeDialog = null; };

            if (!highContrast)
            {
                dialog.SourceInitialized += delegate
                {
                    int useDarkTitle = dark ? 1 : 0;
                    WindowsNativeMethods.DwmSetWindowAttribute(new WindowInteropHelper(dialog).Handle,
                        WindowsNativeMethods.DwmWindowAttributeUseImmersiveDarkMode, ref useDarkTitle, sizeof(int));
                };
            }

            DockPanel layout = new DockPanel();
            StackPanel content = new StackPanel { Margin = new Thickness(24, 20, 24, 16) };

            content.Children.Add(new TextBlock
            {
                Text = "个性化外观",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 14)
            });

            // Display Mode
            content.Children.Add(new TextBlock
            {
                Text = "显示内容模式",
                FontWeight = FontWeights.Medium,
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 6)
            });
            ComboBox modeBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 14),
                Padding = new Thickness(8, 6, 8, 6)
            };
            foreach (var item in DisplayModes)
                modeBox.Items.Add(item.Label);
            content.Children.Add(modeBox);

            // Corner Radius
            DockPanel radiusHeader = new DockPanel { Margin = new Thickness(0, 0, 0, 4) };
            TextBlock radiusValueText = new TextBlock
            {
                Foreground = subtle,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DockPanel.SetDock(radiusValueText, Dock.Right);
            radiusHeader.Children.Add(radiusValueText);
            radiusHeader.Children.Add(new TextBlock
            {
                Text = "圆角半径",
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            });
            content.Children.Add(radiusHeader);

            Slider radiusSlider = new Slider
            {
                Minimum = AppearanceBounds.MinCornerRadius,
                Maximum = AppearanceBounds.MaxCornerRadius,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 0, 0, 6)
            };
            content.Children.Add(radiusSlider);

            CheckBox fullRoundBox = new CheckBox
            {
                Content = "满圆（高度 / 2）",
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 14)
            };
            content.Children.Add(fullRoundBox);

            // Counter Corner Radius
            DockPanel counterRadiusHeader = new DockPanel { Margin = new Thickness(0, 0, 0, 4) };
            TextBlock counterRadiusValueText = new TextBlock
            {
                Foreground = subtle,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DockPanel.SetDock(counterRadiusValueText, Dock.Right);
            counterRadiusHeader.Children.Add(counterRadiusValueText);
            counterRadiusHeader.Children.Add(new TextBlock
            {
                Text = "计数器圆角半径",
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            });
            content.Children.Add(counterRadiusHeader);

            Slider counterRadiusSlider = new Slider
            {
                Minimum = AppearanceBounds.MinCounterCornerRadius,
                Maximum = AppearanceBounds.MaxCounterCornerRadius,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 0, 0, 6)
            };
            content.Children.Add(counterRadiusSlider);

            CheckBox fullCounterRoundBox = new CheckBox
            {
                Content = "计数器满圆（高度 / 2）",
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 14)
            };
            content.Children.Add(fullCounterRoundBox);

            // Padding
            DockPanel paddingHeader = new DockPanel { Margin = new Thickness(0, 0, 0, 4) };
            TextBlock paddingValueText = new TextBlock
            {
                Foreground = subtle,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DockPanel.SetDock(paddingValueText, Dock.Right);
            paddingHeader.Children.Add(paddingValueText);
            paddingHeader.Children.Add(new TextBlock
            {
                Text = "内边距 (Padding)",
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            });
            content.Children.Add(paddingHeader);

            Slider paddingSlider = new Slider
            {
                Minimum = AppearanceBounds.MinPadding,
                Maximum = AppearanceBounds.MaxPadding,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 0, 0, 14)
            };
            content.Children.Add(paddingSlider);

            // Toggles
            CheckBox borderBox = new CheckBox
            {
                Content = "显示边框 (Border)",
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 8)
            };
            CheckBox shadowBox = new CheckBox
            {
                Content = "显示阴影 (Shadow)",
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 8)
            };
            CheckBox previewBox = new CheckBox
            {
                Content = "显示复制内容预览",
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 8)
            };
            CheckBox materialBox = new CheckBox
            {
                Content = "高级材质 (Acrylic)",
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 4)
            };
            content.Children.Add(borderBox);
            content.Children.Add(shadowBox);
            content.Children.Add(previewBox);
            content.Children.Add(materialBox);

            // Footer buttons
            Grid buttons = new Grid { Margin = new Thickness(24, 12, 24, 16) };
            buttons.ColumnDefinitions.Add(new ColumnDefinition());
            buttons.ColumnDefinitions.Add(new ColumnDefinition());
            buttons.ColumnDefinitions.Add(new ColumnDefinition());

            Button previewButton = new Button { Content = "预览效果", MinHeight = 32, Margin = new Thickness(0, 0, 6, 0) };
            Button resetButton = new Button { Content = "恢复默认", MinHeight = 32, Margin = new Thickness(3, 0, 3, 0) };
            Button closeButton = new Button { Content = "关闭", MinHeight = 32, Margin = new Thickness(6, 0, 0, 0), IsCancel = true };

            Grid.SetColumn(previewButton, 0);
            Grid.SetColumn(resetButton, 1);
            Grid.SetColumn(closeButton, 2);
            buttons.Children.Add(previewButton);
            buttons.Children.Add(resetButton);
            buttons.Children.Add(closeButton);

            Border footer = new Border { Child = buttons };
            if (!highContrast)
            {
                footer.Background = new SolidColorBrush(dark ? Color.FromRgb(32, 32, 32) : Color.FromRgb(246, 246, 246));
                footer.BorderBrush = new SolidColorBrush(dark ? Color.FromRgb(29, 29, 29) : Color.FromRgb(226, 226, 226));
                footer.BorderThickness = new Thickness(0, 1, 0, 0);
            }

            DockPanel.SetDock(footer, Dock.Bottom);
            layout.Children.Add(footer);
            layout.Children.Add(content);
            dialog.Content = layout;

            bool suppress = false;
            void SyncFromModel(PreferencesModel m)
            {
                suppress = true;
                try
                {
                    int modeIdx = Array.FindIndex(DisplayModes, x => x.Mode == m.DisplayMode);
                    modeBox.SelectedIndex = modeIdx >= 0 ? modeIdx : 0;
                    double maxRadius = Math.Max(1, Math.Floor(AppearanceBounds.ComputeCardHeight(m.DisplayMode, m.Padding) / 2.0));
                    radiusSlider.Maximum = maxRadius;
                    radiusSlider.Value = m.FullCornerRadius ? maxRadius : Math.Min(m.CornerRadius, maxRadius);
                    fullRoundBox.IsChecked = m.FullCornerRadius;
                    radiusSlider.IsEnabled = !m.FullCornerRadius;
                    radiusValueText.Text = m.FullCornerRadius
                        ? $"满圆 ({maxRadius:0} px)"
                        : (radiusSlider.Value >= maxRadius ? $"满圆 ({maxRadius:0} px)" : $"{m.CornerRadius} px");
                    counterRadiusSlider.Value = m.FullCounterCornerRadius ? AppearanceBounds.MaxCounterCornerRadius : m.CounterCornerRadius;
                    fullCounterRoundBox.IsChecked = m.FullCounterCornerRadius;
                    counterRadiusSlider.IsEnabled = !m.FullCounterCornerRadius;
                    counterRadiusValueText.Text = m.FullCounterCornerRadius || counterRadiusSlider.Value >= AppearanceBounds.MaxCounterCornerRadius
                        ? $"满圆 ({AppearanceBounds.MaxCounterCornerRadius} px)"
                        : $"{m.CounterCornerRadius} px";
                    paddingSlider.Value = m.Padding;
                    paddingValueText.Text = $"{m.Padding} px";
                    borderBox.IsChecked = m.ShowBorder;
                    shadowBox.IsChecked = m.ShowShadow;
                    previewBox.IsChecked = m.ShowPreview;
                    materialBox.IsChecked = m.UseMaterial;
                }
                finally { suppress = false; }
            }

            void ApplyChange(ChangePreferencesCommand command)
            {
                if (suppress) return;
                var result = changePreferences.Handle(command);
                applyPreferences(result.Preferences);
                refreshTray(result.Preferences);
                SyncFromModel(result.Preferences);
            }

            SyncFromModel(preferences.Current);

            modeBox.SelectionChanged += delegate
            {
                if (modeBox.SelectedIndex >= 0 && modeBox.SelectedIndex < DisplayModes.Length)
                    ApplyChange(new ChangePreferencesCommand { DisplayMode = DisplayModes[modeBox.SelectedIndex].Mode });
            };
            radiusSlider.ValueChanged += delegate
            {
                ApplyChange(new ChangePreferencesCommand { CornerRadius = (int)Math.Round(radiusSlider.Value) });
            };
            fullRoundBox.Checked += delegate
            {
                ApplyChange(new ChangePreferencesCommand { FullCornerRadius = true });
            };
            fullRoundBox.Unchecked += delegate
            {
                ApplyChange(new ChangePreferencesCommand { FullCornerRadius = false });
            };
            counterRadiusSlider.ValueChanged += delegate
            {
                ApplyChange(new ChangePreferencesCommand { CounterCornerRadius = (int)Math.Round(counterRadiusSlider.Value) });
            };
            fullCounterRoundBox.Checked += delegate
            {
                ApplyChange(new ChangePreferencesCommand { FullCounterCornerRadius = true });
            };
            fullCounterRoundBox.Unchecked += delegate
            {
                ApplyChange(new ChangePreferencesCommand { FullCounterCornerRadius = false });
            };
            paddingSlider.ValueChanged += delegate
            {
                ApplyChange(new ChangePreferencesCommand { Padding = (int)Math.Round(paddingSlider.Value) });
            };
            borderBox.Checked += delegate { ApplyChange(new ChangePreferencesCommand { ShowBorder = true }); };
            borderBox.Unchecked += delegate { ApplyChange(new ChangePreferencesCommand { ShowBorder = false }); };
            shadowBox.Checked += delegate { ApplyChange(new ChangePreferencesCommand { ShowShadow = true }); };
            shadowBox.Unchecked += delegate { ApplyChange(new ChangePreferencesCommand { ShowShadow = false }); };
            previewBox.Checked += delegate { ApplyChange(new ChangePreferencesCommand { ShowPreview = true }); };
            previewBox.Unchecked += delegate { ApplyChange(new ChangePreferencesCommand { ShowPreview = false }); };
            materialBox.Checked += delegate { ApplyChange(new ChangePreferencesCommand { UseMaterial = true }); };
            materialBox.Unchecked += delegate { ApplyChange(new ChangePreferencesCommand { UseMaterial = false }); };

            previewButton.Click += delegate
            {
                status.Handle("个性化预览", "这是一段即时预览文字，可以在复制时查看当前样式");
            };
            resetButton.Click += delegate
            {
                ApplyChange(new ChangePreferencesCommand
                {
                    DisplayMode = DisplayMode.Full,
                    CornerRadius = AppearanceBounds.DefaultCornerRadius,
                    FullCornerRadius = false,
                    CounterCornerRadius = AppearanceBounds.DefaultCounterCornerRadius,
                    FullCounterCornerRadius = false,
                    Padding = AppearanceBounds.DefaultPadding,
                    ShowBorder = true,
                    ShowShadow = true,
                    ShowPreview = true,
                    UseMaterial = true
                });
            };
            closeButton.Click += delegate { dialog.Close(); };

            dialog.Show();
        }
    }
}
