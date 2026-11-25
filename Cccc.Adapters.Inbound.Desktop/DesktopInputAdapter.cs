using System;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Threading;
using Cccc.Application;

namespace Cccc.Adapters.Inbound.Desktop
{
    public sealed class DesktopInputAdapter : IDisposable
    {
        private readonly KeyboardHookListener keyboard;
        private readonly ClipboardListener clipboard;
        private readonly TrayMenu tray;

        public DesktopInputAdapter(FeedbackSession session, PreferencesService preferences,
            StartupRegistration startup, HandleKeyHandler handleKey,
            ObserveClipboardHandler observe, ChangePreferencesHandler changePreferences,
            ChangeStartupHandler changeStartup, SetPausedHandler setPaused,
            ShowStatusHandler status, ILocalizer localizer, ISystemClock clock,
            Dispatcher dispatcher, CancellationToken cancellationToken, Func<bool> isDark,
            Func<TaskbarPlacementDecision> taskbarStatus, Action<PreferencesModel> applyPreferences,
            Action<bool> switchLayout, Func<UninstallResult> uninstall,
            Action exit, string productLabel)
        {
            keyboard = new KeyboardHookListener(handleKey, dispatcher);
            clipboard = new ClipboardListener(observe, clock, dispatcher, cancellationToken);
            tray = new TrayMenu(session, preferences, startup, changePreferences, changeStartup,
                setPaused, status, localizer, isDark, taskbarStatus, applyPreferences, switchLayout,
                uninstall, exit, productLabel);
        }

        public void Start(HwndSource source)
        {
            keyboard.Start();
            clipboard.Attach(source);
        }

        public void Refresh()
        {
            tray.RefreshAll();
        }

        public void ShowWarning(string title, string detail)
        {
            tray.ShowWarning(title, detail);
        }

        public void SetPausedState(bool paused)
        {
            tray.SetPausedState(paused);
        }

        public void UpdateTheme()
        {
            tray.UpdateTheme();
        }

        public void Dispose()
        {
            keyboard.Dispose();
            clipboard.Dispose();
            tray.Dispose();
        }
    }
}
