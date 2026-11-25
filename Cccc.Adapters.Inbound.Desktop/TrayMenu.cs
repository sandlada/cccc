using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Cccc.Application;

namespace Cccc.Adapters.Inbound.Desktop
{
    public sealed class TrayMenu : IDisposable
    {
        private readonly FeedbackSession session;
        private readonly PreferencesService preferences;
        private readonly StartupRegistration startup;
        private readonly ChangePreferencesHandler changePreferences;
        private readonly ChangeStartupHandler changeStartup;
        private readonly SetPausedHandler setPaused;
        private readonly ShowStatusHandler status;
        private readonly ILocalizer localizer;
        private readonly Func<bool> isDark;
        private readonly Func<TaskbarPlacementDecision> taskbarStatus;
        private readonly Action<PreferencesModel> applyPreferences;
        private readonly Action<bool> switchLayout;
        private readonly Func<UninstallResult> uninstall;
        private readonly Action exit;
        private readonly string productLabel;
        private readonly NotifyIcon tray;
        private readonly Icon trayIcon;
        private readonly bool ownsIcon;
        private readonly ContextMenuStrip menu;
        private readonly ToolStripMenuItem headingItem, pauseItem, previewItem, materialItem, startupItem;
        private readonly ToolStripMenuItem personalizationItem, styleMenu, cardStyleItem, taskbarStyleItem, positionStatusItem;
        private readonly ToolStripMenuItem languageMenu, systemLanguageItem, uninstallItem, exitItem;
        private readonly List<(string Tag, ToolStripMenuItem Item)> languageItems = new();

        public TrayMenu(FeedbackSession session, PreferencesService preferences,
            StartupRegistration startup, ChangePreferencesHandler changePreferences,
            ChangeStartupHandler changeStartup, SetPausedHandler setPaused,
            ShowStatusHandler status, ILocalizer localizer, Func<bool> isDark,
            Func<TaskbarPlacementDecision> taskbarStatus,
            Action<PreferencesModel> applyPreferences, Action<bool> switchLayout,
            Func<UninstallResult> uninstall, Action exit, string productLabel)
        {
            this.session = session;
            this.preferences = preferences;
            this.startup = startup;
            this.changePreferences = changePreferences;
            this.changeStartup = changeStartup;
            this.setPaused = setPaused;
            this.status = status;
            this.localizer = localizer ?? new LocalizationService();
            this.isDark = isDark;
            this.taskbarStatus = taskbarStatus;
            this.applyPreferences = applyPreferences;
            this.switchLayout = switchLayout;
            this.uninstall = uninstall;
            this.exit = exit;
            this.productLabel = productLabel;

            string entry = Assembly.GetEntryAssembly() == null ? "" : Assembly.GetEntryAssembly().Location;
            trayIcon = string.IsNullOrEmpty(entry) ? null : Icon.ExtractAssociatedIcon(entry);
            ownsIcon = trayIcon != null;
            if (trayIcon == null) trayIcon = SystemIcons.Application;
            tray = new NotifyIcon { Icon = trayIcon, Text = SafeTooltip(this.localizer.Get("tray.tooltip.active")), Visible = true };

            menu = new ContextMenuStrip();
            headingItem = new ToolStripMenuItem { Enabled = false };
            pauseItem = new ToolStripMenuItem { Checked = session.Paused };
            pauseItem.Click += delegate
            {
                SetPausedResult result = setPaused.Handle(!session.Paused);
                SetPausedState(result.Paused);
            };
            previewItem = new ToolStripMenuItem { Checked = preferences.Current.ShowPreview };
            previewItem.Click += delegate
            {
                var result = changePreferences.Handle(new ChangePreferencesCommand
                {
                    ShowPreview = !preferences.Current.ShowPreview
                });
                applyPreferences(result.Preferences);
                RefreshPreferences(result.Preferences);
                if (!result.Success)
                    ShowWarning(this.localizer.Get("warning.save_failed.title"), this.localizer.Get("warning.save_failed.detail"));
                status.Handle(
                    this.localizer.Get(preferences.Current.ShowPreview ? "status.preview_on.title" : "status.preview_off.title"),
                    this.localizer.Get(preferences.Current.ShowPreview ? "status.preview_on.detail" : "status.preview_off.detail"));
            };
            materialItem = new ToolStripMenuItem { Checked = preferences.Current.UseMaterial };
            materialItem.Click += delegate
            {
                var result = changePreferences.Handle(new ChangePreferencesCommand
                {
                    UseMaterial = !preferences.Current.UseMaterial
                });
                applyPreferences(result.Preferences);
                RefreshPreferences(result.Preferences);
                if (!result.Success)
                    ShowWarning(this.localizer.Get("warning.save_failed.title"), this.localizer.Get("warning.save_failed.detail"));
                status.Handle(
                    this.localizer.Get(preferences.Current.UseMaterial ? "status.material_on.title" : "status.material_off.title"),
                    this.localizer.Get(preferences.Current.UseMaterial
                        ? (preferences.Current.Layout == LayoutMode.Taskbar
                            ? "status.material_on.detail_taskbar"
                            : "status.material_on.detail_card")
                        : "status.material_off.detail"));
            };
            startupItem = new ToolStripMenuItem();
            startupItem.Click += delegate
            {
                StartupState current = ReadStartupState();
                ChangeStartupResult result = changeStartup.Handle(current == null || !current.Enabled);
                if (!result.Success)
                {
                    RefreshStartup();
                    ShowWarning(this.localizer.Get("warning.startup_failed.title"), this.localizer.Get("warning.startup_failed.detail"));
                    return;
                }
                SetStartupState(result.State);
                status.Handle(
                    this.localizer.Get(result.State.Enabled ? "status.startup_on.title" : "status.startup_off.title"),
                    this.localizer.Get(result.State.Enabled ? "status.startup_on.detail" : "status.startup_off.detail"));
            };
            personalizationItem = new ToolStripMenuItem();
            personalizationItem.Click += delegate
            {
                PersonalizationDialog.ShowDialogOrActivate(
                    preferences, changePreferences, status, this.localizer, isDark, applyPreferences, RefreshPreferences);
            };
            styleMenu = new ToolStripMenuItem();
            cardStyleItem = new ToolStripMenuItem();
            taskbarStyleItem = new ToolStripMenuItem();
            cardStyleItem.Click += delegate { SwitchLayout(false); };
            taskbarStyleItem.Click += delegate { SwitchLayout(true); };
            styleMenu.DropDownItems.Add(cardStyleItem);
            styleMenu.DropDownItems.Add(taskbarStyleItem);

            // Language submenu dynamically built from auto-discovered catalogs
            languageMenu = new ToolStripMenuItem();
            systemLanguageItem = new ToolStripMenuItem();
            systemLanguageItem.Click += delegate { SwitchLanguage(LanguageSettings.SystemTag); };
            languageMenu.DropDownItems.Add(systemLanguageItem);
            languageMenu.DropDownItems.Add(new ToolStripSeparator());
            foreach (LanguageOption option in this.localizer.AvailableLanguages)
            {
                string tag = option.Tag;
                ToolStripMenuItem item = new ToolStripMenuItem(option.NativeName);
                item.Click += delegate { SwitchLanguage(tag); };
                languageItems.Add((tag, item));
                languageMenu.DropDownItems.Add(item);
            }

            positionStatusItem = new ToolStripMenuItem { Enabled = false };
            uninstallItem = new ToolStripMenuItem();
            uninstallItem.Click += delegate { Uninstall(); };
            exitItem = new ToolStripMenuItem();
            exitItem.Click += delegate { exit(); };

            menu.Items.Add(headingItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(pauseItem);
            menu.Items.Add(previewItem);
            menu.Items.Add(startupItem);
            menu.Items.Add(materialItem);
            menu.Items.Add(personalizationItem);
            menu.Items.Add(styleMenu);
            menu.Items.Add(languageMenu);
            menu.Items.Add(positionStatusItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(uninstallItem);
            menu.Items.Add(exitItem);
            menu.Opening += delegate
            {
                WindowsMenuTheme.ApplyDarkMode(isDark());
                RefreshAll();
            };
            tray.ContextMenuStrip = menu;
            tray.DoubleClick += delegate
            {
                status.Handle(
                    this.localizer.Get(session.Paused ? "status.paused.title" : "status.running.title"),
                    this.localizer.Get(session.Paused ? "status.paused.tray_detail" : "status.running.detail"));
            };

            this.localizer.LanguageChanged += OnLanguageChanged;
            RefreshAll();
            UpdateTheme();
        }

        public void RefreshAll()
        {
            headingItem.Text = localizer.Format("tray.heading", productLabel);
            pauseItem.Text = localizer.Get("tray.pause");
            pauseItem.Checked = session.Paused;
            personalizationItem.Text = localizer.Get("tray.personalization");
            styleMenu.Text = localizer.Get("tray.style");
            cardStyleItem.Text = localizer.Get("tray.style.card");
            taskbarStyleItem.Text = localizer.Get("tray.style.taskbar");
            languageMenu.Text = localizer.Get("tray.language");
            systemLanguageItem.Text = localizer.Get("tray.language.system");
            uninstallItem.Text = localizer.Get("tray.uninstall");
            exitItem.Text = localizer.Get("tray.exit");
            tray.Text = SafeTooltip(localizer.Get(session.Paused ? "tray.tooltip.paused" : "tray.tooltip.active"));
            RefreshPreferences(preferences.Current);
            RefreshStartup();
        }

        public void RefreshPreferences(PreferencesModel model)
        {
            bool compact = model.Layout == LayoutMode.Taskbar;
            previewItem.Enabled = !compact;
            previewItem.Checked = !compact && model.ShowPreview;
            previewItem.Text = localizer.Get(compact ? "tray.preview_compact" : "tray.preview");
            materialItem.Checked = model.UseMaterial;
            materialItem.Text = localizer.Get("tray.material");
            cardStyleItem.Checked = !compact;
            taskbarStyleItem.Checked = compact;
            positionStatusItem.Visible = compact;
            string placementText = FeedbackMapper.ResolveTaskbarPlacement(taskbarStatus(), localizer);
            positionStatusItem.Text = localizer.Format("tray.position_status", placementText);

            string preferredLanguage = LanguageSettings.NormalizePreference(model.Language);
            systemLanguageItem.Checked = LanguageSettings.IsSystem(preferredLanguage);
            foreach (var entry in languageItems)
                entry.Item.Checked = !systemLanguageItem.Checked &&
                    string.Equals(entry.Tag, preferredLanguage, StringComparison.OrdinalIgnoreCase);
        }

        public void SetPausedState(bool paused)
        {
            pauseItem.Checked = paused;
            tray.Text = SafeTooltip(localizer.Get(paused ? "tray.tooltip.paused" : "tray.tooltip.active"));
        }

        public void SetStartupState(StartupState state)
        {
            startupItem.Enabled = true;
            startupItem.Checked = state != null && state.Enabled;
            startupItem.Text = localizer.Get(state != null && state.Enabled && !state.CurrentLocation
                ? "tray.startup_other"
                : "tray.startup");
        }

        public void UpdateTheme()
        {
            WindowsMenuTheme.ApplyDarkMode(isDark());
        }

        public void ShowWarning(string title, string detail)
        {
            tray.ShowBalloonTip(5000, title, detail, ToolTipIcon.Warning);
        }

        private void OnLanguageChanged(LanguageOption _)
        {
            if (menu.InvokeRequired)
                menu.BeginInvoke(new Action(RefreshAll));
            else
                RefreshAll();
        }

        private void SwitchLanguage(string languageTag)
        {
            string normalized = LanguageSettings.NormalizePreference(languageTag);
            var result = changePreferences.Handle(new ChangePreferencesCommand { Language = normalized });
            localizer.ApplyPreference(result.Preferences.Language);
            applyPreferences(result.Preferences);
            RefreshAll();
            if (!result.Success)
                ShowWarning(localizer.Get("warning.save_failed.title"), localizer.Get("warning.save_failed.detail"));

            string detail = LanguageSettings.IsSystem(normalized)
                ? localizer.Format("status.language_changed.detail_system", localizer.CurrentLanguage.NativeName)
                : localizer.Format("status.language_changed.detail_manual", localizer.CurrentLanguage.NativeName);
            status.Handle(localizer.Get("status.language_changed.title"), detail);
        }

        private void RefreshStartup()
        {
            StartupState state = ReadStartupState();
            if (state == null)
            {
                startupItem.Checked = false;
                startupItem.Enabled = false;
                startupItem.Text = localizer.Get("tray.startup_failed");
                return;
            }
            SetStartupState(state);
        }

        private StartupState ReadStartupState()
        {
            try { return startup.Read(); }
            catch (Exception) { return null; }
        }

        private void SwitchLayout(bool compact)
        {
            if ((preferences.Current.Layout == LayoutMode.Taskbar) == compact) return;
            if (compact && !TaskbarLayoutDialog.Confirm(isDark(), localizer)) return;
            var result = changePreferences.Handle(new ChangePreferencesCommand
            {
                Layout = compact ? LayoutMode.Taskbar : LayoutMode.Card
            });
            if (!result.Success)
                ShowWarning(localizer.Get("warning.save_failed.title"), localizer.Get("warning.save_failed.detail"));
            applyPreferences(result.Preferences);
            switchLayout(compact);
            RefreshPreferences(result.Preferences);
            status.Handle(
                localizer.Get(compact ? "status.layout_taskbar.title" : "status.layout_card.title"),
                localizer.Get("status.layout.detail"));
        }

        private void Uninstall()
        {
            DialogResult confirm = MessageBox.Show(
                localizer.Get("dialog.uninstall.confirm"),
                localizer.Get("dialog.uninstall.title"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (confirm != DialogResult.Yes) return;

            UninstallResult result = uninstall();
            if (!result.Success)
            {
                var remaining = new List<string>();
                if (!result.StartupCleared) remaining.Add(localizer.Get("dialog.uninstall.item.startup"));
                if (!result.SettingsDeleted) remaining.Add(localizer.Get("dialog.uninstall.item.settings"));
                if (!result.LegacySettingsDeleted) remaining.Add(localizer.Get("dialog.uninstall.item.legacy_settings"));
                if (!result.DirectoryDeleted) remaining.Add(localizer.Get("dialog.uninstall.item.directory"));
                MessageBox.Show(
                    localizer.Format("dialog.uninstall.partial_header", string.Join("\n", remaining)),
                    localizer.Get("dialog.uninstall.title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            exit();
        }

        private static string SafeTooltip(string text)
        {
            if (string.IsNullOrEmpty(text)) return "Cccc";
            return text.Length <= 63 ? text : text.Substring(0, 63);
        }

        public void Dispose()
        {
            localizer.LanguageChanged -= OnLanguageChanged;
            tray.Visible = false;
            if (menu != null) menu.Dispose();
            tray.Dispose();
            if (ownsIcon && trayIcon != null) trayIcon.Dispose();
        }
    }
}
