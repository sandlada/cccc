using System;
using System.IO;
using Cccc.Application;

namespace Cccc.Desktop
{
    internal static class DesktopPaths
    {
        internal static string SettingsDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ProductIdentity.Name);

        internal static string CurrentSettingsFile => Path.Combine(
            SettingsDirectory, "settings-taskbar-experiment.txt");

        internal static string LegacySettingsFile => Path.Combine(
            SettingsDirectory, "settings.txt");

        internal static string ExecutablePath() => HostExecutable.Resolve(
            typeof(DesktopPaths).Assembly.Location,
            Environment.ProcessPath,
            File.Exists);
    }
}
