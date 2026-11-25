using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Cccc.Adapters.Outbound.Toast.Wpf;
using Cccc.Adapters.Outbound.Windows;
using Cccc.Application;
using WpfApplication = System.Windows.Application;
using WindowsForms = System.Windows.Forms;

namespace Cccc.Desktop
{
    internal static class Program
    {
        private const string MutexName = ProductIdentity.Mutex;

        [STAThread]
        internal static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--uninstall") return Uninstall();

            LocalizationService localizer = CreateEarlyLocalizer();

            if (args.Length == 2 && args[0] == "--render-preview")
            {
                WpfApplication previewApp = new WpfApplication { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                using (var sample = new ToastPresenter(new PreferencesModel(), previewApp.Dispatcher, localizer))
                {
                    sample.RenderSample(args[1]);
                }
                previewApp.Shutdown();
                return 0;
            }

            bool first;
            using (Mutex single = new Mutex(true, MutexName, out first))
            {
                if (!first) return 0;
                try
                {
                    WindowsForms.Application.EnableVisualStyles();
                    WpfApplication app = new WpfApplication { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                    using (DesktopHost host = new DesktopHost(app, localizer))
                    {
                        app.DispatcherUnhandledException += delegate(object sender,
                            DispatcherUnhandledExceptionEventArgs e)
                        {
                            e.Handled = true;
                            System.Diagnostics.Trace.TraceError("Cccc dispatcher: {0} (0x{1:X8})",
                                e.Exception.GetType().Name, e.Exception.HResult);
                            host.ReportFault();
                        };
                        host.Start();
                        app.Run();
                    }
                    return 0;
                }
                catch (Exception error)
                {
                    System.Diagnostics.Trace.TraceError("Cccc startup: {0} (0x{1:X8})",
                        error.GetType().Name, error.HResult);
                    MessageBox.Show(localizer.Get("error.startup_desktop_session"),
                        "Cccc", MessageBoxButton.OK, MessageBoxImage.Information);
                    return 1;
                }
                finally { single.ReleaseMutex(); }
            }
        }

        private static LocalizationService CreateEarlyLocalizer()
        {
            try
            {
                return new LocalizationService(new WindowsSystemLanguageProvider());
            }
            catch (Exception)
            {
                return new LocalizationService();
            }
        }

        // Silent cleanup used by scripts and real-device testing. Exit codes:
        // 0 = removed, 1 = partial failure (see trace output), 2 = Cccc is running.
        private static int Uninstall()
        {
            bool first;
            using (Mutex single = new Mutex(true, MutexName, out first))
            {
                if (!first) return 2;
                try
                {
                    var startup = new StartupRegistration(new RegistryStartupStore(),
                        DesktopPaths.ExecutablePath());
                    var handler = new UninstallHandler(startup,
                        new FilePreferencesStore(DesktopPaths.CurrentSettingsFile, true),
                        new FilePreferencesStore(DesktopPaths.LegacySettingsFile, true),
                        new FileSettingsDirectory(DesktopPaths.SettingsDirectory));
                    UninstallResult result = handler.Handle();
                    if (result.Success) return 0;
                    System.Diagnostics.Trace.TraceError("Cccc uninstall: {0}", result.Error);
                    return 1;
                }
                catch (Exception error)
                {
                    System.Diagnostics.Trace.TraceError("Cccc uninstall: {0} (0x{1:X8})",
                        error.GetType().Name, error.HResult);
                    return 1;
                }
                finally { single.ReleaseMutex(); }
            }
        }
    }
}
