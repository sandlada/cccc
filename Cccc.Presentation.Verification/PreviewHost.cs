using System.Windows;
using WpfApplication = System.Windows.Application;

namespace Cccc.Presentation.Verification
{
    internal static class PreviewHost
    {
        internal static void Run(WpfApplication application)
        {
            PreviewWindow window = new PreviewWindow();
            application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            application.MainWindow = window;
            window.Show();
            application.Run();
        }
    }
}
