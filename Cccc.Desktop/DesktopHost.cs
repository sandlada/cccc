using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Cccc.Adapters.Inbound.Desktop;
using Cccc.Adapters.Outbound.Toast.Wpf;
using Cccc.Adapters.Outbound.Windows;
using Cccc.Application;
using WpfApplication = System.Windows.Application;
using Cccc.Platform.Windows;

namespace Cccc.Desktop
{
    internal sealed class DesktopHost : IDisposable
    {
        private readonly WpfApplication app;
        private readonly Dispatcher dispatcher;
        private readonly CancellationTokenSource shutdown = new CancellationTokenSource();
        private readonly FeedbackSession session = new FeedbackSession();
        private readonly StopwatchSystemClock clock = new StopwatchSystemClock();
        private readonly WindowsForegroundWindowQuery foreground = new WindowsForegroundWindowQuery();
        private readonly PreferencesService preferences;
        private readonly StartupRegistration startup;
        private readonly ToastPresenter presenter;
        private readonly FilePreferencesStore currentPreferences;
        private readonly FilePreferencesStore legacyPreferences;
        private readonly UninstallHandler uninstall;

        private HwndSource source;
        private HwndSourceHook sourceHook;
        private ClipboardWorker clipboard;
        private DesktopInputAdapter inbound;
        private DispatcherTimer deadlineTimer;
        private RecordCopyAttemptHandler recordCopy;
        private ExpireAttemptHandler expire;
        private ResetSessionHandler reset;
        private SetPausedHandler setPaused;
        private bool disposed;
        private bool faulted;

        internal DesktopHost(WpfApplication app)
        {
            this.app = app;
            dispatcher = app.Dispatcher;

            string currentFile = DesktopPaths.CurrentSettingsFile;
            string legacyFile = DesktopPaths.LegacySettingsFile;
            const bool includeLayout = true;

            startup = new StartupRegistration(new RegistryStartupStore(),
                DesktopPaths.ExecutablePath());
            currentPreferences = new FilePreferencesStore(currentFile, includeLayout);
            legacyPreferences = new FilePreferencesStore(legacyFile, includeLayout);
            preferences = new PreferencesService(
                currentPreferences,
                legacyPreferences);
            preferences.Load();
            uninstall = new UninstallHandler(startup, currentPreferences, legacyPreferences,
                new FileSettingsDirectory(DesktopPaths.SettingsDirectory));
            presenter = new ToastPresenter(preferences.Current, dispatcher);
        }

        internal void Start()
        {
            HwndSourceParameters parameters = new HwndSourceParameters("Cccc Clipboard Listener")
            {
                WindowStyle = unchecked((int)0x80000000),
                Width = 0,
                Height = 0
            };
            source = new HwndSource(parameters);
            sourceHook = HandleWindowMessage;
            source.AddHook(sourceHook);
            if (!WindowsNativeMethods.AddClipboardFormatListener(source.Handle)) throw new Win32Exception();

            clipboard = new ClipboardWorker(source.Handle, clock);

            recordCopy = new RecordCopyAttemptHandler(session, clock, foreground, clipboard,
                presenter, preferences);
            var observe = new ObserveClipboardHandler(session, clock, clipboard, foreground, presenter);
            expire = new ExpireAttemptHandler(session, clock, clipboard, presenter);
            reset = new ResetSessionHandler(session, clipboard);
            var handleKey = new HandleKeyHandler(session, recordCopy);
            setPaused = new SetPausedHandler(session, clipboard, foreground, presenter);
            var status = new ShowStatusHandler(foreground, presenter);
            var changePreferences = new ChangePreferencesHandler(preferences);
            var changeStartup = new ChangeStartupHandler(startup);
            var initialize = new InitializeSettingsHandler(preferences, startup);

            Func<string> taskbarStatus = () => presenter.TaskbarStatus;

            inbound = new DesktopInputAdapter(session, preferences, startup, handleKey, observe,
                changePreferences, changeStartup, setPaused, status, clock, dispatcher,
                shutdown.Token, delegate { return presenter.IsDark; }, taskbarStatus,
                delegate (PreferencesModel model) { presenter.UpdatePreferences(model); },
                SwitchLayout, delegate { return uninstall.Handle(); },
                delegate { app.Shutdown(); }, ProductProfile.Label);
            inbound.Start(source);

            deadlineTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            deadlineTimer.Tick += delegate
            {
                if (!session.Paused)
                {
                    expire.Handle();
                    if (!session.HasPending && session.Count == 0) deadlineTimer.Stop();
                }
                else deadlineTimer.Stop();
            };
            deadlineTimer.Start();

            InitializationResult initialized = initialize.Handle();
            presenter.UpdatePreferences(initialized.Preferences);
            inbound.Refresh();
            if (initialized.NeedsWarning)
                inbound.ShowWarning("部分设置未能初始化", "请在托盘菜单检查开机自启动设置。");
            status.Handle("Cccc 已启动", "现在复制试试吧 · 右键托盘图标可设置");
        }

        private void SwitchLayout(bool compact)
        {
            if (disposed || app.Dispatcher.HasShutdownStarted) return;
            if (deadlineTimer != null) deadlineTimer.Stop();
            if (reset != null) reset.Handle();
            presenter.ReplaceLayout(compact ? LayoutMode.Taskbar : LayoutMode.Card,
                preferences.Current.UseMaterial);
            if (inbound != null) inbound.Refresh();
        }

        private IntPtr HandleWindowMessage(IntPtr windowHandle, int message, IntPtr wordParameter, IntPtr longParameter, ref bool handled)
        {
            if ((message == 0x001A || message == 0x031A || message == 0x0320 || message == 0x0218) &&
                !disposed)
            {
                presenter.RefreshAppearance();
                if (inbound != null) inbound.UpdateTheme();
            }
            return IntPtr.Zero;
        }

        internal void ReportFault()
        {
            if (faulted) { app.Shutdown(1); return; }
            faulted = true;
            try
            {
                if (setPaused != null) setPaused.Handle(true);
            }
            catch (Exception) { }
            if (inbound != null)
            {
                inbound.SetPausedState(true);
                inbound.ShowWarning("Cccc 已暂停", "检测遇到异常，请退出后重新打开。");
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            shutdown.Cancel();
            if (deadlineTimer != null) deadlineTimer.Stop();
            if (inbound != null) { inbound.Dispose(); inbound = null; }
            if (clipboard != null) { clipboard.Dispose(); clipboard = null; }
            if (source != null)
            {
                WindowsNativeMethods.RemoveClipboardFormatListener(source.Handle);
                if (sourceHook != null) source.RemoveHook(sourceHook);
                source.Dispose();
                source = null;
            }
            presenter.Dispose();
            shutdown.Dispose();
        }
    }
}
