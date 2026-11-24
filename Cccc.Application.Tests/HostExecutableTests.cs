using Cccc.Application;
using Xunit;

namespace Cccc.Application.Tests
{
    public sealed class HostExecutableTests
    {
        [Fact]
        public void Prefers_the_apphost_beside_the_assembly()
        {
            string resolved = HostExecutable.Resolve(@"C:\App\Cccc.dll", @"C:\App\Cccc.exe",
                path => true);

            Assert.Equal(@"C:\App\Cccc.exe", resolved);
        }

        [Fact]
        public void Falls_back_to_the_process_image_when_the_apphost_is_missing()
        {
            string resolved = HostExecutable.Resolve(@"C:\App\Cccc.dll", @"C:\Tools\dotnet.exe",
                path => false);

            Assert.Equal(@"C:\Tools\dotnet.exe", resolved);
        }

        [Fact]
        public void Falls_back_to_the_assembly_location_when_nothing_else_is_available()
        {
            string resolved = HostExecutable.Resolve(@"C:\App\Cccc.dll", null, path => false);

            Assert.Equal(@"C:\App\Cccc.dll", resolved);
        }
    }
}
