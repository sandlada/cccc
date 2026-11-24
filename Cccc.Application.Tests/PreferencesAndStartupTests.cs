using System;
using System.IO;
using Cccc.Application;
using Xunit;

namespace Cccc.Application.Tests
{
    public sealed class PreferencesAndStartupTests
    {
        [Fact]
        public void Missing_current_settings_imports_loaded_legacy_once()
        {
            var current = new MemoryPreferencesStore
            {
                LoadResult = new() { State = PreferencesLoadState.Missing }
            };
            var legacy = new MemoryPreferencesStore
            {
                LoadResult = new()
                {
                    State = PreferencesLoadState.Loaded,
                    Model = new() { ShowPreview = false, UseMaterial = false, Layout = LayoutMode.Taskbar }
                }
            };
            var service = new PreferencesService(current, legacy);

            service.Load();

            Assert.False(service.Current.ShowPreview);
            Assert.False(service.Current.UseMaterial);
            Assert.Equal(LayoutMode.Card, service.Current.Layout);
            Assert.Equal(PreferencesLoadState.Missing, service.State);
        }

        [Fact]
        public void Loaded_current_settings_win_over_legacy()
        {
            var current = new MemoryPreferencesStore
            {
                LoadResult = new()
                {
                    State = PreferencesLoadState.Loaded,
                    Model = new() { ShowPreview = true, UseMaterial = true }
                }
            };
            var legacy = new MemoryPreferencesStore
            {
                LoadResult = new()
                {
                    State = PreferencesLoadState.Loaded,
                    Model = new() { ShowPreview = false, UseMaterial = false }
                }
            };
            var service = new PreferencesService(current, legacy);

            service.Load();

            Assert.True(service.Current.ShowPreview);
            Assert.True(service.Current.UseMaterial);
        }

        [Fact]
        public void First_run_persists_default_settings_without_touching_startup()
        {
            var current = new MemoryPreferencesStore
            {
                LoadResult = new() { State = PreferencesLoadState.Missing }
            };
            var service = new PreferencesService(current, new MemoryPreferencesStore());
            service.Load();

            string error;
            Assert.True(service.InitializeFirstRun(out error));
            Assert.Null(error);
            Assert.Equal(1, current.SaveCount);
            Assert.Equal(PreferencesLoadState.Loaded, service.State);
        }

        [Fact]
        public void Unreadable_settings_stop_first_run()
        {
            var current = new MemoryPreferencesStore
            {
                LoadResult = new() { State = PreferencesLoadState.ReadError }
            };
            var service = new PreferencesService(current, new MemoryPreferencesStore());
            service.Load();

            string error;
            Assert.False(service.InitializeFirstRun(out error));
        }

        [Fact]
        public void Startup_command_quotes_absolute_path_and_rejects_unsafe_values()
        {
            Assert.Equal("\"C:\\Program Files\\Cccc\\Cccc.exe\"",
                StartupRegistration.CommandFor(@"C:\Program Files\Cccc\Cccc.exe"));
            Assert.Throws<ArgumentException>(() => StartupRegistration.CommandFor("relative.exe"));
            Assert.Throws<ArgumentException>(() => StartupRegistration.CommandFor("C:\\a\"b.exe"));
            Assert.Throws<ArgumentException>(() => StartupRegistration.CommandFor("C:\\a\nb.exe"));
        }

        [Fact]
        public void Startup_enable_and_disable_round_trip()
        {
            var store = new MemoryStartupStore();
            var startup = new StartupRegistration(store, @"C:\Cccc\Cccc.exe");

            StartupState enabled = startup.SetEnabled(true);
            Assert.True(enabled.Enabled);
            Assert.True(enabled.CurrentLocation);
            Assert.Equal("\"C:\\Cccc\\Cccc.exe\"", store.Value);

            StartupState disabled = startup.SetEnabled(false);
            Assert.False(disabled.Enabled);
            Assert.Null(store.Value);
        }

        [Fact]
        public void Change_startup_reports_io_failure()
        {
            var store = new MemoryStartupStore { FailWrite = true };
            var handler = new ChangeStartupHandler(new StartupRegistration(store, @"C:\Cccc\Cccc.exe"));

            ChangeStartupResult result = handler.Handle(true);

            Assert.False(result.Success);
            Assert.Equal("IOException", result.Error);
        }

        [Fact]
        public void Change_startup_reports_read_failure()
        {
            var store = new MemoryStartupStore { FailRead = true };
            var handler = new ChangeStartupHandler(new StartupRegistration(store, @"C:\Cccc\Cccc.exe"));

            ChangeStartupResult result = handler.Handle(true);

            Assert.False(result.Success);
            Assert.Equal("IOException", result.Error);
        }
    }
}
