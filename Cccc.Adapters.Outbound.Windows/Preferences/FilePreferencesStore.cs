using System;
using System.IO;
using System.Security;
using Cccc.Application;

namespace Cccc.Adapters.Outbound.Windows
{
    public sealed class FilePreferencesStore : IPreferencesStore
    {
        private readonly string path;
        private readonly bool includeLayout;

        public FilePreferencesStore(string path, bool includeLayout)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            this.path = path;
            this.includeLayout = includeLayout;
        }

        public PreferencesLoadResult Load()
        {
            PreferencesLoadResult result = new() { State = PreferencesLoadState.Missing };
            try
            {
                string[] lines = File.ReadAllLines(path);
                result.State = PreferencesLoadState.Loaded;
                foreach (string raw in lines)
                {
                    string line = raw.Trim();
                    if (line == "preview=off") result.Model.ShowPreview = false;
                    else if (line == "preview=on") result.Model.ShowPreview = true;
                    else if (line == "material=off") result.Model.UseMaterial = false;
                    else if (line == "material=on") result.Model.UseMaterial = true;
                    else if (includeLayout && line == "layout=card") result.Model.Layout = LayoutMode.Card;
                    else if (includeLayout && line == "layout=taskbar") result.Model.Layout = LayoutMode.Taskbar;
                }
            }
            catch (IOException error) when (error is FileNotFoundException or DirectoryNotFoundException)
            {
                result.State = PreferencesLoadState.Missing;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or SecurityException)
            {
                result.State = PreferencesLoadState.ReadError;
                result.Error = error.GetType().Name;
            }
            return result;
        }

        public DeleteResult Delete()
        {
            DeleteResult result = new();
            try
            {
                string target = Path.GetFullPath(path);
                if (File.Exists(target)) File.Delete(target);
                string folder = Path.GetDirectoryName(target);
                if (folder is not null && Directory.Exists(folder))
                {
                    foreach (string temporary in Directory.GetFiles(folder, Path.GetFileName(target) + ".*.tmp"))
                    {
                        try { File.Delete(temporary); }
                        catch (Exception error) when (error is IOException or UnauthorizedAccessException or SecurityException) { }
                    }
                }
                result.Success = true;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or SecurityException)
            {
                result.Error = error.GetType().Name;
            }
            return result;
        }

        public PreferencesSaveResult Save(PreferencesModel model)
        {
            string temporary = null;
            PreferencesSaveResult result = new();
            try
            {
                string target = Path.GetFullPath(path);
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                temporary = $"{target}.{Guid.NewGuid():N}.tmp";
                string text = (model.ShowPreview ? "preview=on" : "preview=off") + Environment.NewLine +
                    (model.UseMaterial ? "material=on" : "material=off") + Environment.NewLine;
                if (includeLayout)
                    text += (model.Layout == LayoutMode.Taskbar ? "layout=taskbar" : "layout=card") +
                        Environment.NewLine;
                File.WriteAllText(temporary, text);
                if (File.Exists(target)) File.Replace(temporary, target, null);
                else File.Move(temporary, target);
                result.Success = true;
                return result;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or SecurityException)
            {
                result.Error = error.GetType().Name;
                return result;
            }
            finally
            {
                if (temporary is not null)
                {
                    try { File.Delete(temporary); }
                    catch (Exception error) when (error is IOException or UnauthorizedAccessException or SecurityException) { }
                }
            }
        }
    }
}
