using System;
using System.Linq;
using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace Cccc.Architecture.Tests
{
    public sealed class ArchitectureLayerTests
    {
        private static readonly Assembly Domain = typeof(Cccc.Domain.Feedback.FeedbackEngine).Assembly;
        private static readonly Assembly Application = typeof(Cccc.Application.FeedbackSession).Assembly;
        private static readonly Assembly Platform = typeof(Cccc.Platform.Windows.WindowsNativeMethods).Assembly;
        private static readonly Assembly Inbound = typeof(Cccc.Adapters.Inbound.Desktop.DesktopInputAdapter).Assembly;
        private static readonly Assembly OutboundWindows = typeof(Cccc.Adapters.Outbound.Windows.ClipboardWorker).Assembly;
        private static readonly Assembly Toast = typeof(Cccc.Adapters.Outbound.Toast.Wpf.ToastPresenter).Assembly;
        private static readonly Assembly Shape = typeof(Cccc.MaterialShape.MaterialShapes).Assembly;
        private static readonly Assembly Host = typeof(Cccc.Desktop.DesktopHost).Assembly;

        [Fact]
        public void Domain_depends_on_nothing_outside_itself()
        {
            var result = Types.InAssembly(Domain)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "Cccc.Application", "Cccc.Adapters", "Cccc.Desktop", "Cccc.Platform",
                    "System.Windows", "System.Windows.Forms", "Microsoft.Win32",
                    "Windows.UI", "Windows.Foundation", "Windows.Graphics")
                .GetResult();

            Assert.True(result.IsSuccessful, Describe(result));
        }

        [Fact]
        public void Application_depends_only_on_domain()
        {
            var result = Types.InAssembly(Application)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "Cccc.Adapters", "Cccc.Desktop", "Cccc.Platform",
                    "System.Windows", "System.Windows.Forms", "Microsoft.Win32",
                    "Windows.UI", "Windows.Foundation", "Windows.Graphics")
                .GetResult();

            Assert.True(result.IsSuccessful, Describe(result));
        }

        [Fact]
        public void Inbound_adapter_does_not_reference_outbound_adapters_or_host()
        {
            var result = Types.InAssembly(Inbound)
                .ShouldNot()
                .HaveDependencyOnAny("Cccc.Adapters.Outbound", "Cccc.Desktop")
                .GetResult();

            Assert.True(result.IsSuccessful, Describe(result));
        }

        [Fact]
        public void Outbound_windows_adapter_does_not_reference_inbound_or_host()
        {
            var result = Types.InAssembly(OutboundWindows)
                .ShouldNot()
                .HaveDependencyOnAny("Cccc.Adapters.Inbound", "Cccc.Adapters.Outbound.Toast", "Cccc.Desktop")
                .GetResult();

            Assert.True(result.IsSuccessful, Describe(result));
        }

        [Fact]
        public void Toast_adapter_does_not_reference_inbound_or_other_outbound_adapters_or_host()
        {
            var result = Types.InAssembly(Toast)
                .ShouldNot()
                .HaveDependencyOnAny("Cccc.Adapters.Inbound", "Cccc.Adapters.Outbound.Windows", "Cccc.Desktop")
                .GetResult();

            Assert.True(result.IsSuccessful, Describe(result));
        }

        [Fact]
        public void Production_projects_do_not_reference_the_host()
        {
            Assembly[] assemblies = [Domain, Application, Platform, Inbound, OutboundWindows, Toast, Shape];
            foreach (Assembly assembly in assemblies)
            {
                var result = Types.InAssembly(assembly)
                    .ShouldNot()
                    .HaveDependencyOn("Cccc.Desktop")
                    .GetResult();
                Assert.True(result.IsSuccessful, assembly.GetName().Name + ": " + Describe(result));
                Assert.DoesNotContain(assembly.GetReferencedAssemblies(),
                    reference => reference.Name == "Cccc");
            }
        }

        [Fact]
        public void Material_shape_library_references_no_Cccc_assembly()
        {
            Assert.DoesNotContain(Shape.GetReferencedAssemblies(),
                reference => reference.Name.StartsWith("Cccc.", StringComparison.Ordinal));
        }

        [Fact]
        public void Shape_library_depends_on_no_ui_framework()
        {
            var result = Types.InAssembly(Shape)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "System.Windows",
                    "System.Windows.Forms",
                    "Microsoft.Win32",
                    "Windows.UI",
                    "Windows.Foundation",
                    "Windows.Graphics")
                .GetResult();

            Assert.True(result.IsSuccessful, Describe(result));
        }

        [Fact]
        public void Domain_references_no_Cccc_assembly()
        {
            Assert.DoesNotContain(Domain.GetReferencedAssemblies(),
                reference => reference.Name.StartsWith("Cccc.", StringComparison.Ordinal));
        }

        [Fact]
        public void Application_references_only_domain_inside_Cccc()
        {
            string[] references = Application.GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .Where(name => name.StartsWith("Cccc.", StringComparison.Ordinal))
                .OrderBy(name => name)
                .ToArray();

            Assert.Equal(["Cccc.Domain"], references);
        }

        [Fact]
        public void Inbound_references_application_and_platform_but_not_domain()
        {
            string[] references = CcccReferences(Inbound);
            Assert.Equal(["Cccc.Application", "Cccc.Platform.Windows"], references);
        }

        [Fact]
        public void Windows_outbound_references_application_and_platform_but_not_domain()
        {
            string[] references = CcccReferences(OutboundWindows);
            Assert.Equal(["Cccc.Application", "Cccc.Platform.Windows"], references);
        }

        [Fact]
        public void Toast_references_application_domain_material_shape_and_platform()
        {
            string[] references = CcccReferences(Toast);
            Assert.Equal([
                "Cccc.Application",
                "Cccc.Domain",
                "Cccc.MaterialShape",
                "Cccc.Platform.Windows"
            ], references);
        }

        [Fact]
        public void Host_references_application_platform_and_adapters()
        {
            string[] references = CcccReferences(Host);
            Assert.Contains("Cccc.Application", references);
            Assert.Contains("Cccc.Platform.Windows", references);
            Assert.Contains("Cccc.Adapters.Inbound.Desktop", references);
            Assert.Contains("Cccc.Adapters.Outbound.Windows", references);
            Assert.Contains("Cccc.Adapters.Outbound.Toast.Wpf", references);
        }

        private static string[] CcccReferences(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .Where(name => name.StartsWith("Cccc.", StringComparison.Ordinal))
                .OrderBy(name => name)
                .ToArray();
        }

        private static string Describe(TestResult result)
        {
            return result.FailingTypeNames is null
                ? "rule failed without reporting types"
                : string.Join(", ", result.FailingTypeNames);
        }
    }
}
