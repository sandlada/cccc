using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Cccc.Domain.Localization;

namespace Cccc.Application
{
    public static class LocalizationCatalogLoader
    {
        private const string ResourceMarker = ".Localization.Catalogs.";

        public static IReadOnlyList<LocalizationCatalog> LoadEmbeddedCatalogs(Assembly assembly = null)
        {
            Assembly source = assembly ?? typeof(LocalizationCatalogLoader).Assembly;
            string[] names = source.GetManifestResourceNames();
            List<LocalizationCatalog> catalogs = new();
            HashSet<string> seenTags = new(StringComparer.OrdinalIgnoreCase);

            foreach (string resourceName in names)
            {
                if (!resourceName.Contains(ResourceMarker, StringComparison.Ordinal) ||
                    !resourceName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    using Stream stream = source.GetManifestResourceStream(resourceName);
                    if (stream is null) continue;
                    LocalizationCatalog catalog = ParseCatalog(stream, resourceName);
                    if (catalog is null) continue;
                    if (seenTags.Add(catalog.Descriptor.Tag.Value))
                        catalogs.Add(catalog);
                }
                catch (Exception error)
                {
                    Trace.TraceWarning("Cccc localization catalog '{0}' skipped: {1}", resourceName, error.Message);
                }
            }

            catalogs.Sort(CompareCatalogs);
            return catalogs;
        }

        public static LocalizationCatalog ParseCatalog(Stream stream, string sourceName = null)
        {
            using JsonDocument document = JsonDocument.Parse(stream, new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            });

            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return null;
            if (!root.TryGetProperty("meta", out JsonElement meta) || meta.ValueKind != JsonValueKind.Object) return null;
            if (!root.TryGetProperty("strings", out JsonElement stringsElement) || stringsElement.ValueKind != JsonValueKind.Object) return null;

            string rawTag = ReadOptionalString(meta, "tag");
            string normalizedTag = LanguageTag.Normalize(rawTag);
            if (normalizedTag is null || string.Equals(normalizedTag, LanguageTag.SystemValue, StringComparison.OrdinalIgnoreCase))
                return null;

            string nativeName = ReadOptionalString(meta, "nativeName");
            if (string.IsNullOrWhiteSpace(nativeName)) return null;

            string cultureName = ReadOptionalString(meta, "culture");
            List<string> aliases = ReadOptionalStringList(meta, "aliases");
            string fontFamily = ReadOptionalFontFallback(meta);

            Dictionary<string, string> entries = new(StringComparer.Ordinal);
            foreach (JsonProperty property in stringsElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.String)
                    entries[property.Name] = property.Value.GetString() ?? "";
            }

            LanguageDescriptor descriptor = new(
                new LanguageTag(normalizedTag),
                nativeName,
                aliases,
                cultureName,
                fontFamily);

            return new LocalizationCatalog(descriptor, entries);
        }

        private static string ReadOptionalString(JsonElement parent, string name)
        {
            if (parent.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String)
                return value.GetString();
            return null;
        }

        private static List<string> ReadOptionalStringList(JsonElement parent, string name)
        {
            if (!parent.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.Array)
                return null;
            List<string> items = new();
            foreach (JsonElement item in value.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    string text = item.GetString();
                    if (!string.IsNullOrWhiteSpace(text)) items.Add(text);
                }
            }
            return items;
        }

        private static string ReadOptionalFontFallback(JsonElement meta)
        {
            if (!meta.TryGetProperty("fontFallback", out JsonElement value)) return null;
            if (value.ValueKind == JsonValueKind.String) return value.GetString();
            if (value.ValueKind == JsonValueKind.Array)
            {
                List<string> fonts = new();
                foreach (JsonElement item in value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        string font = item.GetString()?.Trim();
                        if (!string.IsNullOrEmpty(font)) fonts.Add(font);
                    }
                }
                return fonts.Count > 0 ? string.Join(", ", fonts) : null;
            }
            return null;
        }

        private static int CompareCatalogs(LocalizationCatalog left, LocalizationCatalog right)
        {
            bool leftEn = left.Descriptor.Tag == LanguageTag.English;
            bool rightEn = right.Descriptor.Tag == LanguageTag.English;
            if (leftEn && !rightEn) return -1;
            if (!leftEn && rightEn) return 1;
            return StringComparer.OrdinalIgnoreCase.Compare(left.Descriptor.Tag.Value, right.Descriptor.Tag.Value);
        }
    }
}
