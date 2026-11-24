using System;
using System.IO;
using Cccc.Adapters.Outbound.Windows;
using Cccc.Desktop;
using Xunit;

namespace Cccc.Architecture.Tests
{
    // Guards the identity strings that keep this fork's settings, startup entry
    // and single-instance mutex separate from the upstream CCCC application.
    // Windows treats paths and registry value names case-insensitively, so
    // "Cccc" is not distinct from "CCCC"; these assertions must never regress
    // to a case-only difference.
    public sealed class ProductIdentityTests
    {
        [Fact]
        public void Runtime_identity_matches_the_product_name()
        {
            Assert.Equal("Sandlada.Cccc", ProductIdentity.Name);
            Assert.Equal(ProductIdentity.Name, ProductIdentity.StartupEntry);
            Assert.Equal(ProductIdentity.Name, Path.GetFileName(DesktopPaths.SettingsDirectory));
        }

        [Fact]
        public void Runtime_identity_is_not_a_case_only_variant_of_upstream()
        {
            Assert.NotEqual("CCCC", ProductIdentity.Name, StringComparer.OrdinalIgnoreCase);
            Assert.NotEqual("CCCC", RegistryStartupStore.EntryName, StringComparer.OrdinalIgnoreCase);
            Assert.NotEqual("Local\\CCCC.CopyFeedback.v1", ProductIdentity.Mutex,
                StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Startup_store_entry_matches_the_product_identity()
        {
            Assert.Equal(ProductIdentity.StartupEntry, RegistryStartupStore.EntryName);
        }

        [Fact]
        public void Build_script_uses_the_same_identity()
        {
            string script = File.ReadAllText(FindBuildScript());
            Assert.Contains(ProductIdentity.Name, script, StringComparison.Ordinal);
            Assert.Contains(ProductIdentity.StartupEntry, script, StringComparison.Ordinal);
        }

        private static string FindBuildScript()
        {
            DirectoryInfo directory = new(AppContext.BaseDirectory);
            while (directory is not null)
            {
                string candidate = Path.Combine(directory.FullName, "build.ps1");
                if (File.Exists(candidate)) return candidate;
                directory = directory.Parent;
            }
            throw new FileNotFoundException("build.ps1 was not found above the test output directory.");
        }
    }
}
