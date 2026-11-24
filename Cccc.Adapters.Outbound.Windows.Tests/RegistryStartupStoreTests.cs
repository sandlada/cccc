using System;
using Microsoft.Win32;
using Cccc.Adapters.Outbound.Windows;
using Xunit;

namespace Cccc.Adapters.Outbound.Windows.Tests
{
    public sealed class RegistryStartupStoreTests
    {
        [Fact]
        public void Isolated_key_round_trips_and_deletes()
        {
            string key = "Software\\Cccc\\StartupTests\\" + Guid.NewGuid().ToString("N");
            try
            {
                var store = new RegistryStartupStore(key);
                Assert.Null(store.Read());
                store.Write("\"C:\\Cccc\\Cccc.exe\"");
                Assert.Equal("\"C:\\Cccc\\Cccc.exe\"", store.Read());
                store.Delete();
                Assert.Null(store.Read());
            }
            finally
            {
                try { Registry.CurrentUser.DeleteSubKeyTree(key, false); }
                catch (ArgumentException) { }
            }
        }

        [Fact]
        public void Arbitrary_registry_keys_are_rejected()
        {
            Assert.Throws<ArgumentException>(() =>
                new RegistryStartupStore("Software\\Microsoft\\Windows\\CurrentVersion\\Run"));
        }
    }
}
