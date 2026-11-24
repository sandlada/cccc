using System;
using System.IO;
using System.Security;

namespace Cccc.Application
{
    public sealed class UninstallHandler
    {
        private readonly StartupRegistration startup;
        private readonly IPreferencesStore currentStore;
        private readonly IPreferencesStore legacyStore;
        private readonly ISettingsDirectory settingsDirectory;

        public UninstallHandler(StartupRegistration startup, IPreferencesStore currentStore,
            IPreferencesStore legacyStore, ISettingsDirectory settingsDirectory)
        {
            ArgumentNullException.ThrowIfNull(startup);
            ArgumentNullException.ThrowIfNull(currentStore);
            ArgumentNullException.ThrowIfNull(legacyStore);
            ArgumentNullException.ThrowIfNull(settingsDirectory);
            this.startup = startup;
            this.currentStore = currentStore;
            this.legacyStore = legacyStore;
            this.settingsDirectory = settingsDirectory;
        }

        // Removes the traces Cccc writes outside its own folder: the startup
        // registration, both settings files, and their folder. A failure in one
        // area does not stop the others, so a rerun can finish the remaining work.
        public UninstallResult Handle()
        {
            UninstallResult result = new();
            string error = null;

            try
            {
                startup.SetEnabled(false);
                result.StartupCleared = true;
            }
            catch (Exception failure) when (failure is IOException or UnauthorizedAccessException or SecurityException or ArgumentException)
            {
                error = Describe(error, "startup", failure.GetType().Name);
            }

            DeleteResult current = currentStore.Delete();
            result.SettingsDeleted = current.Success;
            if (!current.Success) error = Describe(error, "settings", current.Error);

            DeleteResult legacy = legacyStore.Delete();
            result.LegacySettingsDeleted = legacy.Success;
            if (!legacy.Success) error = Describe(error, "legacy", legacy.Error);

            DeleteResult directory = settingsDirectory.Delete();
            result.DirectoryDeleted = directory.Success;
            if (!directory.Success) error = Describe(error, "directory", directory.Error);

            result.Success = result.StartupCleared && result.SettingsDeleted &&
                result.LegacySettingsDeleted && result.DirectoryDeleted;
            result.Error = error ?? "";
            return result;
        }

        private static string Describe(string existing, string area, string detail)
        {
            string entry = area + ":" + (string.IsNullOrEmpty(detail) ? "unknown" : detail);
            return string.IsNullOrEmpty(existing) ? entry : existing + "; " + entry;
        }
    }
}
