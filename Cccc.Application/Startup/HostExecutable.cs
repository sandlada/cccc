using System;
using System.IO;

namespace Cccc.Application
{
    public static class HostExecutable
    {
        // Assembly.Location points at Cccc.dll, which Windows cannot launch from the
        // Run key. Prefer the apphost executable beside the assembly, then the
        // process image, and keep the assembly path as a last resort.
        public static string Resolve(string assemblyLocation, string processPath,
            Func<string, bool> fileExists)
        {
            ArgumentNullException.ThrowIfNull(fileExists);
            if (!string.IsNullOrWhiteSpace(assemblyLocation))
            {
                string host = Path.ChangeExtension(assemblyLocation, ".exe");
                if (!string.IsNullOrWhiteSpace(host) && fileExists(host)) return host;
            }
            if (!string.IsNullOrWhiteSpace(processPath)) return processPath;
            return assemblyLocation ?? "";
        }
    }
}
