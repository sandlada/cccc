using System;
using System.Globalization;
using Cccc.Application;

namespace Cccc.Adapters.Outbound.Windows
{
    public sealed class WindowsSystemLanguageProvider : ISystemLanguageProvider
    {
        public string GetSystemLanguageTag()
        {
            try
            {
                CultureInfo.CurrentUICulture.ClearCachedData();
                string name = CultureInfo.CurrentUICulture.Name;
                if (!string.IsNullOrWhiteSpace(name))
                    return name;

                string installed = CultureInfo.InstalledUICulture.Name;
                return string.IsNullOrWhiteSpace(installed) ? null : installed;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
