using System;
using System.Collections.Generic;

namespace Cccc.Domain.Localization
{
    public static class LanguageResolver
    {
        public static LanguageDescriptor Resolve(
            string requestedTag,
            IReadOnlyList<LanguageDescriptor> available,
            LanguageDescriptor fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);
            if (available is null || available.Count == 0) return fallback;

            string normalized = LanguageTag.Normalize(requestedTag);
            if (normalized is null || string.Equals(normalized, LanguageTag.SystemValue, StringComparison.OrdinalIgnoreCase))
                return fallback;

            // Pass 1: exact canonical tag match
            LanguageDescriptor exactTag = FindByTag(available, normalized);
            if (exactTag is not null) return exactTag;

            // Pass 2: exact alias match
            LanguageDescriptor exactAlias = FindByAlias(available, normalized);
            if (exactAlias is not null) return exactAlias;

            // Pass 3: BCP-47 parent subtag walk (e.g. zh-Hans-CN -> zh-Hans, fr-CA -> fr)
            string candidate = normalized;
            while (true)
            {
                int dash = candidate.LastIndexOf('-');
                if (dash <= 0) break;
                candidate = candidate.Substring(0, dash);

                LanguageDescriptor parentTag = FindByTag(available, candidate);
                if (parentTag is not null) return parentTag;

                LanguageDescriptor parentAlias = FindByAlias(available, candidate);
                if (parentAlias is not null) return parentAlias;
            }

            // Pass 4: safe fallback (English)
            return fallback;
        }

        private static LanguageDescriptor FindByTag(IReadOnlyList<LanguageDescriptor> available, string tag)
        {
            for (int i = 0; i < available.Count; i++)
            {
                if (string.Equals(available[i].Tag.Value, tag, StringComparison.OrdinalIgnoreCase))
                    return available[i];
            }
            return null;
        }

        private static LanguageDescriptor FindByAlias(IReadOnlyList<LanguageDescriptor> available, string tag)
        {
            for (int i = 0; i < available.Count; i++)
            {
                IReadOnlyList<string> aliases = available[i].Aliases;
                for (int j = 0; j < aliases.Count; j++)
                {
                    if (string.Equals(aliases[j], tag, StringComparison.OrdinalIgnoreCase))
                        return available[i];
                }
            }
            return null;
        }
    }
}
