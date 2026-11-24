using System;
using Cccc.Application;
using Xunit;

namespace Cccc.Application.Tests
{
    public sealed class UninstallHandlerTests
    {
        private static UninstallHandler Create(MemoryStartupStore startupStore,
            MemoryPreferencesStore current, MemoryPreferencesStore legacy,
            MemorySettingsDirectory directory = null)
        {
            return new UninstallHandler(
                new StartupRegistration(startupStore, @"C:\Cccc\Cccc.exe"),
                current, legacy, directory ?? new MemorySettingsDirectory());
        }

        [Fact]
        public void Uninstall_clears_startup_settings_and_the_settings_folder()
        {
            var startupStore = new MemoryStartupStore { Value = "\"C:\\Cccc\\Cccc.exe\"" };
            var current = new MemoryPreferencesStore();
            var legacy = new MemoryPreferencesStore();
            var directory = new MemorySettingsDirectory();

            UninstallResult result = Create(startupStore, current, legacy, directory).Handle();

            Assert.True(result.Success);
            Assert.True(result.StartupCleared);
            Assert.True(result.SettingsDeleted);
            Assert.True(result.LegacySettingsDeleted);
            Assert.True(result.DirectoryDeleted);
            Assert.Null(startupStore.Value);
            Assert.Equal(1, current.DeleteCount);
            Assert.Equal(1, legacy.DeleteCount);
            Assert.Equal(1, directory.DeleteCount);
            Assert.Equal("", result.Error);
        }

        [Fact]
        public void Uninstall_reports_settings_failure_but_still_clears_startup()
        {
            var startupStore = new MemoryStartupStore { Value = "\"C:\\Cccc\\Cccc.exe\"" };
            var current = new MemoryPreferencesStore
            {
                DeleteResult = new DeleteResult { Success = false, Error = "IOException" }
            };
            var legacy = new MemoryPreferencesStore();

            UninstallResult result = Create(startupStore, current, legacy).Handle();

            Assert.False(result.Success);
            Assert.True(result.StartupCleared);
            Assert.False(result.SettingsDeleted);
            Assert.True(result.LegacySettingsDeleted);
            Assert.True(result.DirectoryDeleted);
            Assert.Null(startupStore.Value);
            Assert.Equal(1, legacy.DeleteCount);
            Assert.Contains("settings:IOException", result.Error);
        }

        [Fact]
        public void Uninstall_reports_startup_failure_but_still_deletes_settings()
        {
            var startupStore = new MemoryStartupStore { FailDelete = true };
            var current = new MemoryPreferencesStore();
            var legacy = new MemoryPreferencesStore();

            UninstallResult result = Create(startupStore, current, legacy).Handle();

            Assert.False(result.Success);
            Assert.False(result.StartupCleared);
            Assert.True(result.SettingsDeleted);
            Assert.True(result.LegacySettingsDeleted);
            Assert.True(result.DirectoryDeleted);
            Assert.Equal(1, current.DeleteCount);
            Assert.Contains("startup:IOException", result.Error);
        }

        [Fact]
        public void Uninstall_reports_remaining_settings_folder()
        {
            var startupStore = new MemoryStartupStore();
            var current = new MemoryPreferencesStore();
            var legacy = new MemoryPreferencesStore();
            var directory = new MemorySettingsDirectory
            {
                DeleteResult = new DeleteResult { Success = false, Error = "UnauthorizedAccessException" }
            };

            UninstallResult result = Create(startupStore, current, legacy, directory).Handle();

            Assert.False(result.Success);
            Assert.False(result.DirectoryDeleted);
            Assert.Contains("directory:UnauthorizedAccessException", result.Error);
        }

        [Fact]
        public void Uninstall_is_idempotent_when_nothing_was_written()
        {
            var startupStore = new MemoryStartupStore();
            var current = new MemoryPreferencesStore();
            var legacy = new MemoryPreferencesStore();
            UninstallHandler handler = Create(startupStore, current, legacy);

            UninstallResult first = handler.Handle();
            UninstallResult second = handler.Handle();

            Assert.True(first.Success);
            Assert.True(second.Success);
            Assert.Null(startupStore.Value);
            Assert.Equal(2, current.DeleteCount);
            Assert.Equal("", second.Error);
        }
    }
}
