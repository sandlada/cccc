using System;
using System.IO;
using Cccc.Adapters.Outbound.Windows;
using Cccc.Application;
using Xunit;

namespace Cccc.Adapters.Outbound.Windows.Tests
{
    public sealed class FileSettingsDirectoryTests
    {
        [Fact]
        public void Delete_removes_the_folder_and_everything_inside_it()
        {
            string root = Path.Combine(Path.GetTempPath(), "Cccc-dir-" + Guid.NewGuid().ToString("N"));
            string directory = Path.Combine(root, "Cccc");
            Directory.CreateDirectory(directory);
            try
            {
                File.WriteAllText(Path.Combine(directory, "settings.txt"), "preview=on\n");
                File.WriteAllText(Path.Combine(directory, "leftover.tmp"), "stale");
                var store = new FileSettingsDirectory(directory);

                DeleteResult result = store.Delete();

                Assert.True(result.Success);
                Assert.False(Directory.Exists(directory));
            }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }

        [Fact]
        public void Delete_succeeds_when_the_folder_was_never_created()
        {
            string directory = Path.Combine(Path.GetTempPath(), "Cccc-dir-" + Guid.NewGuid().ToString("N"));
            var store = new FileSettingsDirectory(Path.Combine(directory, "Cccc"));

            DeleteResult result = store.Delete();

            Assert.True(result.Success);
            Assert.Equal("", result.Error);
        }
    }
}
