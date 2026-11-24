using System;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal static class Themes
    {
        internal static Appearance Current = Read();
        internal static bool MaterialAvailable;
        internal static bool MaterialEnabled = true;
        internal static bool UseAcrylic { get { return MaterialEnabled && MaterialAvailable && Current.Transparent; } }
        internal static Appearance Read()
        {
            Appearance value = new Appearance { Transparent = true };
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                {
                    if (key != null) { value.Dark = Convert.ToInt32(key.GetValue(ToastProfile.Compact ? "SystemUsesLightTheme" : "AppsUseLightTheme", 1)) == 0;
                        value.Transparent = Convert.ToInt32(key.GetValue("EnableTransparency", 1)) != 0; }
                }
            }
            catch (Exception) { }
            if (SystemParameters.HighContrast) value.Transparent = false;
            try { if (Windows.System.Power.PowerManager.EnergySaverStatus == Windows.System.Power.EnergySaverStatus.On) value.Transparent = false; }
            catch (Exception) { }
            return value;
        }
        internal static bool Refresh()
        {
            Appearance next = Read();
            bool changed = next.Dark != Current.Dark || next.Transparent != Current.Transparent;
            Current = next; return changed;
        }
    }
}
