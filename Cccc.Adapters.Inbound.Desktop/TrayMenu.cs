using System;
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
        private readonly Func<bool> isDark;
        private readonly Func<string> taskbarStatus;
        private readonly Action<PreferencesModel> applyPreferences;
        private readonly Action<bool> switchLayout;
        private readonly Func<UninstallResult> uninstall;
        private readonly Action exit;
        private readonly string productLabel;
        private readonly NotifyIcon tray;
        private readonly Icon trayIcon;
        private readonly bool ownsIcon;
        private readonly ContextMenuStrip menu;
        private readonly ToolStripMenuItem pauseItem, previewItem, materialItem, startupItem;
        private readonly ToolStripMenuItem cardStyleItem, taskbarStyleItem, positionStatusItem;

        public TrayMenu(FeedbackSession session, PreferencesService preferences,
            StartupRegistration startup, ChangePreferencesHandler changePreferences,
            ChangeStartupHandler changeStartup, SetPausedHandler setPaused,
            ShowStatusHandler status, Func<bool> isDark, Func<string> taskbarStatus,
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
            tray = new NotifyIcon { Icon = trayIcon, Text = "Cccc · 复制确认", Visible = true };

            menu = new ContextMenuStrip();
            ToolStripMenuItem heading = new ToolStripMenuItem("Cccc 复制确认 · " + productLabel)
            {
                Enabled = false
            };
            pauseItem = new ToolStripMenuItem("暂停提示") { Checked = session.Paused };
            pauseItem.Click += delegate
            {
                SetPausedResult result = setPaused.Handle(!session.Paused);
                SetPausedState(result.Paused);
            };
            previewItem = new ToolStripMenuItem("显示内容预览") { Checked = preferences.Current.ShowPreview };
            previewItem.Click += delegate
            {
                var result = changePreferences.Handle(new ChangePreferencesCommand
                {
                    ShowPreview = !preferences.Current.ShowPreview
                });
                applyPreferences(result.Preferences);
                RefreshPreferences(result.Preferences);
                if (!result.Success) ShowWarning("设置未能保存", "本次选择仍然生效，重启后可能恢复原设置。");
                status.Handle(preferences.Current.ShowPreview ? "内容预览已开启" : "内容预览已隐藏",
                    preferences.Current.ShowPreview ? "复制后显示一小段内容" : "复制后仅显示内容类型和次数");
            };
            materialItem = new ToolStripMenuItem("高级材质") { Checked = preferences.Current.UseMaterial };
            materialItem.Click += delegate
            {
                var result = changePreferences.Handle(new ChangePreferencesCommand
                {
                    UseMaterial = !preferences.Current.UseMaterial
                });
                applyPreferences(result.Preferences);
                RefreshPreferences(result.Preferences);
                if (!result.Success) ShowWarning("设置未能保存", "本次选择仍然生效，重启后可能恢复原设置。");
                status.Handle(preferences.Current.UseMaterial ? "高级材质已开启" : "高级材质已关闭",
                    preferences.Current.UseMaterial
                        ? (preferences.Current.Layout == LayoutMode.Taskbar
                            ? "借用任务栏背景，不叠加额外模糊"
                            : "系统允许时显示 Acrylic 材质")
                        : "使用纯色背景，保留文字与动画");
            };
            startupItem = new ToolStripMenuItem("开机自启动");
            startupItem.Click += delegate
            {
                StartupState current = ReadStartupState();
                ChangeStartupResult result = changeStartup.Handle(current == null || !current.Enabled);
                if (!result.Success)
                {
                    RefreshStartup();
                    ShowWarning("未能更改开机自启动", "设置未能保存，请稍后重试");
                    return;
                }
                SetStartupState(result.State);
                status.Handle(result.State.Enabled ? "开机自启动已开启" : "开机自启动已关闭",
                    result.State.Enabled ? "下次登录 Windows 时启动 Cccc" : "需要时双击程序启动");
            };
            ToolStripMenuItem uninstallItem = new ToolStripMenuItem("卸载 Cccc…");
            uninstallItem.Click += delegate { Uninstall(); };
            ToolStripMenuItem exitItem = new ToolStripMenuItem("退出 Cccc");
            exitItem.Click += delegate { exit(); };

            menu.Items.Add(heading);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(pauseItem);
            menu.Items.Add(previewItem);
            menu.Items.Add(startupItem);
            menu.Items.Add(materialItem);
            ToolStripMenuItem personalizationItem = new ToolStripMenuItem("个性化设置…");
            personalizationItem.Click += delegate
            {
                PersonalizationDialog.ShowDialogOrActivate(
                    preferences, changePreferences, status, isDark, applyPreferences, RefreshPreferences);
            };
            ToolStripMenuItem styleMenu = new ToolStripMenuItem("提示样式");
            cardStyleItem = new ToolStripMenuItem("普通卡片");
            taskbarStyleItem = new ToolStripMenuItem("任务栏单行（实验性）");
            cardStyleItem.Click += delegate { SwitchLayout(false); };
            taskbarStyleItem.Click += delegate { SwitchLayout(true); };
            styleMenu.DropDownItems.Add(cardStyleItem);
            styleMenu.DropDownItems.Add(taskbarStyleItem);
            positionStatusItem = new ToolStripMenuItem { Enabled = false };
            menu.Items.Add(personalizationItem);
            menu.Items.Add(styleMenu);
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
                status.Handle(session.Paused ? "Cccc 已暂停" : "Cccc 正在运行",
                    session.Paused ? "取消勾选托盘菜单的“暂停提示”即可恢复" : "复制时自动提示 · 右键托盘图标可设置");
            };
            RefreshAll();
            UpdateTheme();
        }

        public void RefreshAll()
        {
            pauseItem.Checked = session.Paused;
            tray.Text = session.Paused ? "Cccc · 已暂停" : "Cccc · 复制确认";
            RefreshPreferences(preferences.Current);
            RefreshStartup();
        }

        public void RefreshPreferences(PreferencesModel model)
        {
            bool compact = model.Layout == LayoutMode.Taskbar;
            previewItem.Enabled = !compact;
            previewItem.Checked = !compact && model.ShowPreview;
            previewItem.Text = compact ? "内容预览（单行样式不显示）" : "显示内容预览";
            materialItem.Checked = model.UseMaterial;
            materialItem.Text = "高级材质";
            cardStyleItem.Checked = !compact;
            taskbarStyleItem.Checked = compact;
            positionStatusItem.Visible = compact;
            positionStatusItem.Text = "定位：" + taskbarStatus();
        }

        public void SetPausedState(bool paused)
        {
            pauseItem.Checked = paused;
            tray.Text = paused ? "Cccc · 已暂停" : "Cccc · 复制确认";
        }

        public void SetStartupState(StartupState state)
        {
            startupItem.Enabled = true;
            startupItem.Checked = state != null && state.Enabled;
            startupItem.Text = state != null && state.Enabled && !state.CurrentLocation
                ? "开机自启动（其他位置）"
                : "开机自启动";
        }

        public void UpdateTheme()
        {
            WindowsMenuTheme.ApplyDarkMode(isDark());
        }

        public void ShowWarning(string title, string detail)
        {
            tray.ShowBalloonTip(5000, title, detail, ToolTipIcon.Warning);
        }

        private void RefreshStartup()
        {
            StartupState state = ReadStartupState();
            if (state == null)
            {
                startupItem.Checked = false;
                startupItem.Enabled = false;
                startupItem.Text = "开机自启动（读取失败）";
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
            if (compact && !TaskbarLayoutDialog.Confirm(isDark())) return;
            var result = changePreferences.Handle(new ChangePreferencesCommand
            {
                Layout = compact ? LayoutMode.Taskbar : LayoutMode.Card
            });
            if (!result.Success) ShowWarning("设置未能保存", "本次选择仍然生效，重启后可能恢复原设置。");
            applyPreferences(result.Preferences);
            switchLayout(compact);
            RefreshPreferences(result.Preferences);
            status.Handle(compact ? "已切换任务栏单行" : "已切换普通卡片",
                "复制反馈与动画保持不变");
        }

        private void Uninstall()
        {
            DialogResult confirm = MessageBox.Show(
                "将移除开机自启动，并删除 %LocalAppData%\\Sandlada.Cccc 设置文件夹。\n" +
                "不会删除程序文件夹，需要时请自行删除。\n\n确定要卸载 Cccc 吗？",
                "卸载 Cccc", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (confirm != DialogResult.Yes) return;

            UninstallResult result = uninstall();
            if (!result.Success)
            {
                var remaining = new System.Collections.Generic.List<string>();
                if (!result.StartupCleared) remaining.Add("开机自启动项");
                if (!result.SettingsDeleted) remaining.Add("当前设置文件");
                if (!result.LegacySettingsDeleted) remaining.Add("旧版设置文件");
                if (!result.DirectoryDeleted) remaining.Add("设置文件夹");
                MessageBox.Show(
                    "以下内容未能自动移除：\n" + string.Join("\n", remaining) +
                    "\n\n请手动检查 HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run " +
                    "或 %LocalAppData%\\Sandlada.Cccc。",
                    "卸载 Cccc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            exit();
        }

        public void Dispose()
        {
            tray.Visible = false;
            if (menu != null) menu.Dispose();
            tray.Dispose();
            if (ownsIcon && trayIcon != null) trayIcon.Dispose();
        }
    }
}
