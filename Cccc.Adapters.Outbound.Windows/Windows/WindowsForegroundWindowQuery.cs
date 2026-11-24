using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Windows
{
    public sealed class WindowsForegroundWindowQuery : IForegroundWindowQuery
    {
        public WindowTarget Current
        {
            get
            {
                var window = WindowsNativeMethods.GetForegroundWindow();
                return new(window.ToInt64(), WindowsNativeMethods.GetProcessIdForWindow(window));
            }
        }
    }
}
