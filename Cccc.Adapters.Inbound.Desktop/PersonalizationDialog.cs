using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Inbound.Desktop
{
    internal sealed class PersonalizationDialog
    {
        private static Window activeDialog;

        private static readonly (DisplayMode Mode, string LabelKey)[] DisplayModes =
        {
            (DisplayMode.Full, "dialog.personalization.mode.full"),
            (DisplayMode.IconOnly, "dialog.personalization.mode.icon_only"),
            (DisplayMode.IconAndLabel, "dialog.personalization.mode.icon_label"),
            (DisplayMode.IconAndContent, "dialog.personalization.mode.icon_content"),
            (DisplayMode.LabelOnly, "dialog.personalization.mode.label_only"),
            (DisplayMode.ContentOnly, "dialog.personalization.mode.content_only"),
            (DisplayMode.LabelAndContent, "dialog.personalization.mode.label_content")
        };

        private static readonly (WindowPosition? Pos, string LabelKey)[] PositionGridCells =
        {
            (WindowPosition.TopLeft, "dialog.personalization.pos.top_left"),
            (WindowPosition.TopCenter, "dialog.personalization.pos.top_center"),
            (WindowPosition.TopRight, "dialog.personalization.pos.top_right"),
            (WindowPosition.MiddleLeft, "dialog.personalization.pos.middle_left"),
            (null, ""),
            (WindowPosition.MiddleRight, "dialog.personalization.pos.middle_right"),
            (WindowPosition.BottomLeft, "dialog.personalization.pos.bottom_left"),
            (WindowPosition.BottomCenter, "dialog.personalization.pos.bottom_center"),
            (WindowPosition.BottomRight, "dialog.personalization.pos.bottom_right")
        };

        internal static void ShowDialogOrActivate(
            PreferencesService preferences,
            ChangePreferencesHandler changePreferences,
            ShowStatusHandler status,
            ILocalizer localizer,
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

            ILocalizer l10n = localizer ?? new LocalizationService();
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
                Title = l10n.Get("dialog.personalization.title"),
                Width = 460,
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

            TextBlock headingText = new TextBlock
            {
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 14)
            };
            content.Children.Add(headingText);

            // Language (dynamically populated from discovered catalogs)
            TextBlock languageLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 6)
            };
            content.Children.Add(languageLabel);
            ComboBox languageBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 14),
                Padding = new Thickness(8, 6, 8, 6)
            };
            List<string> languageTags = new();
            content.Children.Add(languageBox);

            // Display Mode
            TextBlock displayModeLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 6)
            };
            content.Children.Add(displayModeLabel);
            ComboBox modeBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 14),
                Padding = new Thickness(8, 6, 8, 6)
            };
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
            TextBlock radiusLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            };
            radiusHeader.Children.Add(radiusLabel);
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
            TextBlock counterRadiusLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            };
            counterRadiusHeader.Children.Add(counterRadiusLabel);
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
            TextBlock paddingLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            };
            paddingHeader.Children.Add(paddingLabel);
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
            CheckBox borderBox = new CheckBox { Foreground = foreground, Margin = new Thickness(0, 0, 0, 8) };
            CheckBox shadowBox = new CheckBox { Foreground = foreground, Margin = new Thickness(0, 0, 0, 8) };
            CheckBox previewBox = new CheckBox { Foreground = foreground, Margin = new Thickness(0, 0, 0, 8) };
            CheckBox materialBox = new CheckBox { Foreground = foreground, Margin = new Thickness(0, 0, 0, 4) };
            content.Children.Add(borderBox);
            content.Children.Add(shadowBox);
            content.Children.Add(previewBox);
            content.Children.Add(materialBox);

            // Window Position (8-way grid)
            TextBlock positionLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground,
                Margin = new Thickness(0, 12, 0, 6)
            };
            content.Children.Add(positionLabel);

            UniformGrid positionGrid = new UniformGrid
            {
                Rows = 3,
                Columns = 3,
                Margin = new Thickness(0, 0, 0, 12)
            };
            Button[] positionButtons = new Button[PositionGridCells.Length];
            for (int i = 0; i < PositionGridCells.Length; i++)
            {
                var cell = PositionGridCells[i];
                if (cell.Pos is null)
                {
                    positionGrid.Children.Add(new Border { Margin = new Thickness(2), Height = 28 });
                    continue;
                }
                Button btn = new Button
                {
                    Height = 28,
                    Margin = new Thickness(2),
                    Tag = cell.Pos.Value
                };
                positionButtons[i] = btn;
                positionGrid.Children.Add(btn);
            }
            content.Children.Add(positionGrid);

            // Offset X
            DockPanel offsetXHeader = new DockPanel { Margin = new Thickness(0, 0, 0, 4) };
            TextBlock offsetXValueText = new TextBlock
            {
                Foreground = subtle,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DockPanel.SetDock(offsetXValueText, Dock.Right);
            offsetXHeader.Children.Add(offsetXValueText);
            TextBlock offsetXLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            };
            offsetXHeader.Children.Add(offsetXLabel);
            content.Children.Add(offsetXHeader);

            Slider offsetXSlider = new Slider
            {
                Minimum = PositionBounds.MinOffset,
                Maximum = PositionBounds.MaxOffset,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 0, 0, 10)
            };
            content.Children.Add(offsetXSlider);

            // Offset Y
            DockPanel offsetYHeader = new DockPanel { Margin = new Thickness(0, 0, 0, 4) };
            TextBlock offsetYValueText = new TextBlock
            {
                Foreground = subtle,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DockPanel.SetDock(offsetYValueText, Dock.Right);
            offsetYHeader.Children.Add(offsetYValueText);
            TextBlock offsetYLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground
            };
            offsetYHeader.Children.Add(offsetYLabel);
            content.Children.Add(offsetYHeader);

            Slider offsetYSlider = new Slider
            {
                Minimum = PositionBounds.MinOffset,
                Maximum = PositionBounds.MaxOffset,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 0, 0, 10)
            };
            content.Children.Add(offsetYSlider);

            TextBlock taskbarAdaptText = new TextBlock { TextWrapping = TextWrapping.Wrap, Foreground = foreground };
            CheckBox taskbarAdaptBox = new CheckBox
            {
                Content = taskbarAdaptText,
                Foreground = foreground,
                Margin = new Thickness(0, 0, 0, 10)
            };
            content.Children.Add(taskbarAdaptBox);

            // Font Family
            TextBlock fontFamilyLabel = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                Foreground = foreground,
                Margin = new Thickness(0, 10, 0, 6)
            };
            content.Children.Add(fontFamilyLabel);
            Grid fontRow = new Grid { Margin = new Thickness(0, 0, 0, 4) };
            fontRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            fontRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            TextBox fontFamilyBox = new TextBox
            {
                Padding = new Thickness(8, 5, 8, 5),
                VerticalContentAlignment = VerticalAlignment.Center
            };
            if (!highContrast && dark)
            {
                fontFamilyBox.Background = new SolidColorBrush(Color.FromRgb(32, 32, 32));
                fontFamilyBox.Foreground = foreground;
                fontFamilyBox.BorderBrush = new SolidColorBrush(Color.FromRgb(72, 72, 72));
            }
            Button applyFontButton = new Button
            {
                MinWidth = 64,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(8, 0, 0, 0)
            };
            Grid.SetColumn(fontFamilyBox, 0);
            Grid.SetColumn(applyFontButton, 1);
            fontRow.Children.Add(fontFamilyBox);
            fontRow.Children.Add(applyFontButton);
            content.Children.Add(fontRow);

            // Footer buttons
            Grid buttons = new Grid { Margin = new Thickness(24, 12, 24, 16) };
            buttons.ColumnDefinitions.Add(new ColumnDefinition());
            buttons.ColumnDefinitions.Add(new ColumnDefinition());
            buttons.ColumnDefinitions.Add(new ColumnDefinition());

            Button previewButton = new Button { MinHeight = 32, Margin = new Thickness(0, 0, 6, 0) };
            Button resetButton = new Button { MinHeight = 32, Margin = new Thickness(3, 0, 3, 0) };
            Button closeButton = new Button { MinHeight = 32, Margin = new Thickness(6, 0, 0, 0), IsCancel = true };

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

            ScrollViewer scroll = new ScrollViewer
            {
                Content = content,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MaxHeight = 560
            };

            DockPanel.SetDock(footer, Dock.Bottom);
            layout.Children.Add(footer);
            layout.Children.Add(scroll);
            dialog.Content = layout;

            bool suppress = false;

            void PopulateLocalizedStrings()
            {
                suppress = true;
                try
                {
                    dialog.Title = l10n.Get("dialog.personalization.title");
                    headingText.Text = l10n.Get("dialog.personalization.heading");
                    languageLabel.Text = l10n.Get("dialog.personalization.language");
                    displayModeLabel.Text = l10n.Get("dialog.personalization.display_mode");
                    radiusLabel.Text = l10n.Get("dialog.personalization.corner_radius");
                    fullRoundBox.Content = l10n.Get("dialog.personalization.corner_full");
                    counterRadiusLabel.Text = l10n.Get("dialog.personalization.counter_corner_radius");
                    fullCounterRoundBox.Content = l10n.Get("dialog.personalization.counter_corner_full");
                    paddingLabel.Text = l10n.Get("dialog.personalization.padding");
                    borderBox.Content = l10n.Get("dialog.personalization.show_border");
                    shadowBox.Content = l10n.Get("dialog.personalization.show_shadow");
                    previewBox.Content = l10n.Get("dialog.personalization.show_preview");
                    materialBox.Content = l10n.Get("dialog.personalization.use_material");
                    positionLabel.Text = l10n.Get("dialog.personalization.position");
                    offsetXLabel.Text = l10n.Get("dialog.personalization.offset_x");
                    offsetYLabel.Text = l10n.Get("dialog.personalization.offset_y");
                    taskbarAdaptText.Text = l10n.Get("dialog.personalization.adapt_taskbar");
                    fontFamilyLabel.Text = l10n.Get("dialog.personalization.font_family");
                    fontFamilyBox.ToolTip = l10n.Get("dialog.personalization.font_tooltip");
                    applyFontButton.Content = l10n.Get("dialog.personalization.apply");
                    previewButton.Content = l10n.Get("dialog.personalization.preview");
                    resetButton.Content = l10n.Get("dialog.personalization.reset");
                    closeButton.Content = l10n.Get("dialog.personalization.close");

                    languageBox.Items.Clear();
                    languageTags.Clear();
                    languageTags.Add(LanguageSettings.SystemTag);
                    languageBox.Items.Add(l10n.Get("tray.language.system"));
                    foreach (LanguageOption option in l10n.AvailableLanguages)
                    {
                        languageTags.Add(option.Tag);
                        languageBox.Items.Add(option.NativeName);
                    }

                    modeBox.Items.Clear();
                    foreach (var item in DisplayModes)
                        modeBox.Items.Add(l10n.Get(item.LabelKey));

                    for (int i = 0; i < PositionGridCells.Length; i++)
                    {
                        if (positionButtons[i] != null)
                            positionButtons[i].Content = l10n.Get(PositionGridCells[i].LabelKey);
                    }
                }
                finally { suppress = false; }
            }

            void SyncFromModel(PreferencesModel m)
            {
                suppress = true;
                try
                {
                    string prefLang = LanguageSettings.NormalizePreference(m.Language);
                    int langIdx = languageTags.FindIndex(t => string.Equals(t, prefLang, StringComparison.OrdinalIgnoreCase));
                    languageBox.SelectedIndex = langIdx >= 0 ? langIdx : 0;

                    int modeIdx = Array.FindIndex(DisplayModes, x => x.Mode == m.DisplayMode);
                    modeBox.SelectedIndex = modeIdx >= 0 ? modeIdx : 0;
                    double maxRadius = Math.Max(1, Math.Floor(AppearanceBounds.ComputeCardHeight(m.DisplayMode, m.Padding) / 2.0));
                    radiusSlider.Maximum = maxRadius;
                    radiusSlider.Value = m.FullCornerRadius ? maxRadius : Math.Min(m.CornerRadius, maxRadius);
                    fullRoundBox.IsChecked = m.FullCornerRadius;
                    radiusSlider.IsEnabled = !m.FullCornerRadius;
                    radiusValueText.Text = m.FullCornerRadius || radiusSlider.Value >= maxRadius
                        ? l10n.Format("dialog.personalization.corner_full_value", Math.Round(maxRadius))
                        : $"{m.CornerRadius} px";
                    counterRadiusSlider.Value = m.FullCounterCornerRadius ? AppearanceBounds.MaxCounterCornerRadius : m.CounterCornerRadius;
                    fullCounterRoundBox.IsChecked = m.FullCounterCornerRadius;
                    counterRadiusSlider.IsEnabled = !m.FullCounterCornerRadius;
                    counterRadiusValueText.Text = m.FullCounterCornerRadius || counterRadiusSlider.Value >= AppearanceBounds.MaxCounterCornerRadius
                        ? l10n.Format("dialog.personalization.corner_full_value", AppearanceBounds.MaxCounterCornerRadius)
                        : $"{m.CounterCornerRadius} px";
                    paddingSlider.Value = m.Padding;
                    paddingValueText.Text = $"{m.Padding} px";
                    borderBox.IsChecked = m.ShowBorder;
                    shadowBox.IsChecked = m.ShowShadow;
                    previewBox.IsChecked = m.ShowPreview;
                    materialBox.IsChecked = m.UseMaterial;
                    fontFamilyBox.Text = m.FontFamily ?? AppearanceBounds.DefaultFontFamily;
                    foreach (Button btn in positionButtons)
                    {
                        if (btn == null) continue;
                        bool selected = (WindowPosition)btn.Tag == m.Position;
                        btn.FontWeight = selected ? FontWeights.Bold : FontWeights.Normal;
                        btn.BorderThickness = new Thickness(selected ? 2 : 1);
                        if (!highContrast)
                        {
                            btn.BorderBrush = selected
                                ? new SolidColorBrush(Color.FromRgb(21, 151, 107))
                                : new SolidColorBrush(dark ? Color.FromRgb(72, 72, 72) : Color.FromRgb(200, 200, 204));
                        }
                    }
                    offsetXSlider.Value = m.OffsetX;
                    offsetXValueText.Text = $"{m.OffsetX:+#;-#;0} px";
                    offsetYSlider.Value = m.OffsetY;
                    offsetYValueText.Text = $"{m.OffsetY:+#;-#;0} px";
                    taskbarAdaptBox.IsChecked = m.AdaptToTaskbar;
                }
                finally { suppress = false; }
            }

            void ApplyChange(ChangePreferencesCommand command)
            {
                if (suppress) return;
                var result = changePreferences.Handle(command);
                if (command.Language != null)
                    l10n.ApplyPreference(result.Preferences.Language);
                applyPreferences(result.Preferences);
                refreshTray(result.Preferences);
                SyncFromModel(result.Preferences);
            }

            void OnLanguageChanged(LanguageOption _)
            {
                if (dialog.Dispatcher.CheckAccess())
                {
                    PopulateLocalizedStrings();
                    SyncFromModel(preferences.Current);
                }
                else
                {
                    dialog.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        PopulateLocalizedStrings();
                        SyncFromModel(preferences.Current);
                    }));
                }
            }

            l10n.LanguageChanged += OnLanguageChanged;
            dialog.Closed += delegate
            {
                l10n.LanguageChanged -= OnLanguageChanged;
                activeDialog = null;
            };

            PopulateLocalizedStrings();
            SyncFromModel(preferences.Current);

            languageBox.SelectionChanged += delegate
            {
                if (languageBox.SelectedIndex >= 0 && languageBox.SelectedIndex < languageTags.Count)
                    ApplyChange(new ChangePreferencesCommand { Language = languageTags[languageBox.SelectedIndex] });
            };
            modeBox.SelectionChanged += delegate
            {
                if (modeBox.SelectedIndex >= 0 && modeBox.SelectedIndex < DisplayModes.Length)
                    ApplyChange(new ChangePreferencesCommand { DisplayMode = DisplayModes[modeBox.SelectedIndex].Mode });
            };
            radiusSlider.ValueChanged += delegate
            {
                ApplyChange(new ChangePreferencesCommand { CornerRadius = (int)Math.Round(radiusSlider.Value) });
            };
            fullRoundBox.Checked += delegate { ApplyChange(new ChangePreferencesCommand { FullCornerRadius = true }); };
            fullRoundBox.Unchecked += delegate { ApplyChange(new ChangePreferencesCommand { FullCornerRadius = false }); };
            counterRadiusSlider.ValueChanged += delegate
            {
                ApplyChange(new ChangePreferencesCommand { CounterCornerRadius = (int)Math.Round(counterRadiusSlider.Value) });
            };
            fullCounterRoundBox.Checked += delegate { ApplyChange(new ChangePreferencesCommand { FullCounterCornerRadius = true }); };
            fullCounterRoundBox.Unchecked += delegate { ApplyChange(new ChangePreferencesCommand { FullCounterCornerRadius = false }); };
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
            foreach (Button btn in positionButtons)
            {
                if (btn == null) continue;
                WindowPosition targetPos = (WindowPosition)btn.Tag;
                btn.Click += delegate { ApplyChange(new ChangePreferencesCommand { Position = targetPos }); };
            }
            offsetXSlider.ValueChanged += delegate
            {
                ApplyChange(new ChangePreferencesCommand { OffsetX = (int)Math.Round(offsetXSlider.Value) });
            };
            offsetYSlider.ValueChanged += delegate
            {
                ApplyChange(new ChangePreferencesCommand { OffsetY = (int)Math.Round(offsetYSlider.Value) });
            };
            taskbarAdaptBox.Checked += delegate { ApplyChange(new ChangePreferencesCommand { AdaptToTaskbar = true }); };
            taskbarAdaptBox.Unchecked += delegate { ApplyChange(new ChangePreferencesCommand { AdaptToTaskbar = false }); };
            void CommitFontFamily()
            {
                ApplyChange(new ChangePreferencesCommand { FontFamily = fontFamilyBox.Text });
            }
            applyFontButton.Click += delegate { CommitFontFamily(); };
            fontFamilyBox.KeyDown += (_, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    CommitFontFamily();
                    e.Handled = true;
                }
            };
            fontFamilyBox.LostKeyboardFocus += delegate { CommitFontFamily(); };

            previewButton.Click += delegate
            {
                status.Handle(
                    l10n.Get("status.personalization_preview.title"),
                    l10n.Get("status.personalization_preview.detail"));
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
                    UseMaterial = true,
                    FontFamily = AppearanceBounds.DefaultFontFamily,
                    Position = WindowPosition.BottomLeft,
                    OffsetX = PositionBounds.DefaultOffset,
                    OffsetY = PositionBounds.DefaultOffset,
                    AdaptToTaskbar = true
                });
            };
            closeButton.Click += delegate { dialog.Close(); };

            dialog.Show();
        }
    }
}
