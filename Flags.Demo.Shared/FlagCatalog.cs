using System;
using System.Collections.Generic;
using System.Linq;
using Flags.Icons;

namespace Flags.Demo.Shared {
    public static class FlagCatalog {
        private static readonly Lazy<IReadOnlyDictionary<DemoVariant, IReadOnlyList<FlagEntry>>> ByVariantLazy =
            new Lazy<IReadOnlyDictionary<DemoVariant, IReadOnlyList<FlagEntry>>>(() =>
                FlagIcons.AvailableKinds
                    .Select(k => new FlagEntry(k))
                    .GroupBy(e => e.Variant)
                    .ToDictionary(g => g.Key, g => (IReadOnlyList<FlagEntry>)g.OrderBy(e => e.Code, StringComparer.Ordinal).ToArray()));

        public static IReadOnlyDictionary<DemoVariant, IReadOnlyList<FlagEntry>> ByVariant => ByVariantLazy.Value;

        public static IReadOnlyList<DemoVariant> AllVariants { get; } = new[] {
            DemoVariant.SVG, DemoVariant.PNG1x, DemoVariant.PNG2x, DemoVariant.PNG3x,
        };

        public static IReadOnlyList<FlagEntry> Filter(DemoVariant variant, string? search) {
            if (!ByVariant.TryGetValue(variant, out var source)) return Array.Empty<FlagEntry>();
            if (string.IsNullOrWhiteSpace(search)) return source;
            return source
                .Where(e => e.Code.IndexOf(search!, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
        }
    }
}
