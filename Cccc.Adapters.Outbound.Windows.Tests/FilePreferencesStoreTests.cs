using System;
using System.IO;
using Cccc.Adapters.Outbound.Windows;
using Cccc.Application;
using Xunit;

namespace Cccc.Adapters.Outbound.Windows.Tests
{
    public sealed class FilePreferencesStoreTests
    {
        [Fact]
        public void Round_trip_preserves_preview_material_and_layout()
        {
            string directory = Path.Combine(Path.GetTempPath(), "Cccc-store-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "settings.txt");
                var store = new FilePreferencesStore(path, true);
                var saved = store.Save(new PreferencesModel
                {
                    ShowPreview = false,
                    UseMaterial = false,
                    Layout = LayoutMode.Taskbar
                });

                Assert.True(saved.Success);
                var loaded = store.Load();
                Assert.Equal(PreferencesLoadState.Loaded, loaded.State);
                Assert.False(loaded.Model.ShowPreview);
                Assert.False(loaded.Model.UseMaterial);
                Assert.Equal(LayoutMode.Taskbar, loaded.Model.Layout);
            }
            finally { Directory.Delete(directory, true); }
        }

        [Fact]
        public void Layout_is_ignored_when_the_profile_does_not_include_it()
        {
            string directory = Path.Combine(Path.GetTempPath(), "Cccc-store-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "settings.txt");
                File.WriteAllText(path, "layout=taskbar\npreview=on\n");
                var loaded = new FilePreferencesStore(path, false).Load();
                Assert.Equal(LayoutMode.Card, loaded.Model.Layout);
                Assert.True(loaded.Model.ShowPreview);
            }
            finally { Directory.Delete(directory, true); }
        }

        [Fact]
        public void Unknown_keys_are_ignored_and_missing_file_is_missing()
        {
            string directory = Path.Combine(Path.GetTempPath(), "Cccc-store-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "settings.txt");
                Assert.Equal(PreferencesLoadState.Missing,
                    new FilePreferencesStore(path, true).Load().State);
                File.WriteAllText(path, "garbage\nmaterial=off\npreview=on\n");
                var loaded = new FilePreferencesStore(path, true).Load();
                Assert.Equal(PreferencesLoadState.Loaded, loaded.State);
                Assert.False(loaded.Model.UseMaterial);
                Assert.True(loaded.Model.ShowPreview);
            }
            finally { Directory.Delete(directory, true); }
        }

        [Fact]
        public void Delete_removes_settings_file_and_temporary_files()
        {
            string directory = Path.Combine(Path.GetTempPath(), "Cccc-store-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "settings.txt");
                var store = new FilePreferencesStore(path, true);
                Assert.True(store.Save(new PreferencesModel { ShowPreview = true }).Success);
                File.WriteAllText(path + ".ab12.tmp", "stale");

                DeleteResult result = store.Delete();

                Assert.True(result.Success);
                Assert.False(File.Exists(path));
                Assert.Empty(Directory.GetFiles(directory));
            }
            finally { Directory.Delete(directory, true); }
        }

        [Fact]
        public void Delete_succeeds_when_the_file_was_never_written()
        {
            string directory = Path.Combine(Path.GetTempPath(), "Cccc-store-" + Guid.NewGuid().ToString("N"));
            try
            {
                var store = new FilePreferencesStore(Path.Combine(directory, "settings.txt"), true);

                DeleteResult result = store.Delete();

                Assert.True(result.Success);
                Assert.Equal("", result.Error);
                Assert.False(Directory.Exists(directory));
            }
            finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        }

        [Fact]
        public void Locked_target_reports_failure_and_cleans_temporary_file()
        {
            string directory = Path.Combine(Path.GetTempPath(), "Cccc-store-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "settings.txt");
                var store = new FilePreferencesStore(path, true);
                Assert.True(store.Save(new PreferencesModel { ShowPreview = true }).Success);
                using (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var result = store.Save(new PreferencesModel { ShowPreview = false });
                    Assert.False(result.Success);
                    Assert.False(string.IsNullOrEmpty(result.Error));
                }
                Assert.Empty(Directory.GetFiles(directory, "*.tmp"));
            }
            finally { Directory.Delete(directory, true); }
        }
    }
}
