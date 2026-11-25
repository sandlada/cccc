using System;
using System.Collections.Generic;
using Cccc.Domain.Localization;

namespace Cccc.Application
{
    public sealed class LocalizationCatalog
    {
        public LocalizationCatalog(LanguageDescriptor descriptor, IReadOnlyDictionary<string, string> strings)
        {
            ArgumentNullException.ThrowIfNull(descriptor);
            ArgumentNullException.ThrowIfNull(strings);
            Descriptor = descriptor;
            Strings = strings;
        }

        public LanguageDescriptor Descriptor { get; }
        public IReadOnlyDictionary<string, string> Strings { get; }
    }
}
