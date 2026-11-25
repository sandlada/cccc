using System;
using System.Globalization;
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
                    else if (line.StartsWith("display=", StringComparison.Ordinal))
                        result.Model.DisplayMode = ParseDisplayMode(line.Substring("display=".Length));
                    else if (line.StartsWith("corner_radius=", StringComparison.Ordinal) &&
                        int.TryParse(line.Substring("corner_radius=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out int cornerRadius))
                        result.Model.CornerRadius = AppearanceBounds.ClampCornerRadius(cornerRadius);
                    else if (line == "corner_full=on") result.Model.FullCornerRadius = true;
                    else if (line == "corner_full=off") result.Model.FullCornerRadius = false;
                    else if (line.StartsWith("counter_corner_radius=", StringComparison.Ordinal) &&
                        int.TryParse(line.Substring("counter_corner_radius=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out int counterCornerRadius))
                        result.Model.CounterCornerRadius = AppearanceBounds.ClampCounterCornerRadius(counterCornerRadius);
                    else if (line == "counter_corner_full=on") result.Model.FullCounterCornerRadius = true;
                    else if (line == "counter_corner_full=off") result.Model.FullCounterCornerRadius = false;
                    else if (line.StartsWith("padding=", StringComparison.Ordinal) &&
                        int.TryParse(line.Substring("padding=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out int padding))
                        result.Model.Padding = AppearanceBounds.ClampPadding(padding);
                    else if (line == "border=on") result.Model.ShowBorder = true;
                    else if (line == "border=off") result.Model.ShowBorder = false;
                    else if (line == "shadow=on") result.Model.ShowShadow = true;
                    else if (line == "shadow=off") result.Model.ShowShadow = false;
                    else if (line.StartsWith("font_family=", StringComparison.Ordinal))
                        result.Model.FontFamily = AppearanceBounds.NormalizeFontFamily(line.Substring("font_family=".Length));
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
                text += "display=" + FormatDisplayMode(model.DisplayMode) + Environment.NewLine +
                    "corner_radius=" + AppearanceBounds.ClampCornerRadius(model.CornerRadius).ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
                    (model.FullCornerRadius ? "corner_full=on" : "corner_full=off") + Environment.NewLine +
                    "counter_corner_radius=" + AppearanceBounds.ClampCounterCornerRadius(model.CounterCornerRadius).ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
                    (model.FullCounterCornerRadius ? "counter_corner_full=on" : "counter_corner_full=off") + Environment.NewLine +
                    "padding=" + AppearanceBounds.ClampPadding(model.Padding).ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
                    (model.ShowBorder ? "border=on" : "border=off") + Environment.NewLine +
                    (model.ShowShadow ? "shadow=on" : "shadow=off") + Environment.NewLine +
                    "font_family=" + AppearanceBounds.NormalizeFontFamily(model.FontFamily) + Environment.NewLine;
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

        private static DisplayMode ParseDisplayMode(string value)
        {
            return value switch
            {
                "icon" => DisplayMode.IconOnly,
                "icon_label" => DisplayMode.IconAndLabel,
                "icon_content" => DisplayMode.IconAndContent,
                "label" => DisplayMode.LabelOnly,
                "content" => DisplayMode.ContentOnly,
                "label_content" => DisplayMode.LabelAndContent,
                _ => DisplayMode.Full
            };
        }

        private static string FormatDisplayMode(DisplayMode mode)
        {
            return mode switch
            {
                DisplayMode.IconOnly => "icon",
                DisplayMode.IconAndLabel => "icon_label",
                DisplayMode.IconAndContent => "icon_content",
                DisplayMode.LabelOnly => "label",
                DisplayMode.ContentOnly => "content",
                DisplayMode.LabelAndContent => "label_content",
                _ => "full"
            };
        }
    }
}
