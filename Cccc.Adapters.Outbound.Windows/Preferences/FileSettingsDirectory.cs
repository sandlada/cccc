using System;
using System.IO;
using System.Security;
using Cccc.Application;

namespace Cccc.Adapters.Outbound.Windows
{
    public sealed class FileSettingsDirectory : ISettingsDirectory
    {
        private readonly string path;

        public FileSettingsDirectory(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            this.path = path;
        }

        public DeleteResult Delete()
        {
            DeleteResult result = new();
            try
            {
                string target = Path.GetFullPath(path);
                if (Directory.Exists(target)) Directory.Delete(target, true);
                result.Success = true;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or SecurityException)
            {
                result.Error = error.GetType().Name;
            }
            return result;
        }
    }
}
