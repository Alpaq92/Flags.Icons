using System;
using System.Collections.Generic;
using System.Linq;
using Flags.Icons;

namespace Flags.Demo.Shared {
    /// <summary>
    /// Demo-side catalog of every flag from every source. Always returns a list of
    /// <see cref="FlagSection"/>s so XAML and code-first demos can render section headers
    /// uniformly (single-source views are 1 section; <see cref="DemoSource.All"/> is 5 sections).
    /// </summary>
    public static class FlagCatalog {
        private static readonly Lazy<IReadOnlyList<FlagEntry>> TwemojiLazy =
            new(() => FlagIcons.TwemojiFlags.Select(f => new FlagEntry(f)).OrderBy(e => e.Code, StringComparer.Ordinal).ToArray());
        private static readonly Lazy<IReadOnlyList<FlagEntry>> CircleLazy =
            new(() => FlagIcons.CircleFlags.Select(f => new FlagEntry(f)).OrderBy(e => e.Code, StringComparer.Ordinal).ToArray());
        private static readonly Lazy<IReadOnlyList<FlagEntry>> SquareLazy =
            new(() => FlagIcons.SquareFlags.Select(f => new FlagEntry(f)).OrderBy(e => e.Code, StringComparer.Ordinal).ToArray());
        private static readonly Lazy<IReadOnlyList<FlagEntry>> LipisLazy =
            new(() => FlagIcons.LipisFlags.Select(f => new FlagEntry(f)).OrderBy(e => e.Code, StringComparer.Ordinal).ToArray());
        private static readonly Lazy<IReadOnlyList<FlagEntry>> FlagHubLazy =
            new(() => FlagIcons.FlagHubFlags.Select(f => new FlagEntry(f)).OrderBy(e => e.Code, StringComparer.Ordinal).ToArray());

        /// <summary>The 6 options surfaced by the demos' source-picker combobox.</summary>
        public static IReadOnlyList<DemoSource> AllSources { get; } = new[] {
            DemoSource.Twemoji, DemoSource.Circle, DemoSource.Square, DemoSource.Lipis, DemoSource.FlagHub, DemoSource.All,
        };

        public static IReadOnlyList<FlagSection> Sections(DemoSource source, string? search) {
            if (source == DemoSource.All) {
                return new[] {
                    Make("Twemoji", TwemojiLazy.Value, search),
                    Make("Circle (HatScripts)", CircleLazy.Value, search),
                    Make("Square (kapowaz)", SquareLazy.Value, search),
                    Make("Lipis (lipis/flag-icons 4x3)", LipisLazy.Value, search),
                    Make("FlagHub (Alpaq92 — FlagKit fork)", FlagHubLazy.Value, search),
                };
            }
            var (title, entries) = source switch {
                DemoSource.Twemoji => ("Twemoji", TwemojiLazy.Value),
                DemoSource.Circle => ("Circle (HatScripts)", CircleLazy.Value),
                DemoSource.Square => ("Square (kapowaz)", SquareLazy.Value),
                DemoSource.Lipis => ("Lipis (lipis/flag-icons 4x3)", LipisLazy.Value),
                DemoSource.FlagHub => ("FlagHub (Alpaq92 — FlagKit fork)", FlagHubLazy.Value),
                _ => (string.Empty, (IReadOnlyList<FlagEntry>)Array.Empty<FlagEntry>()),
            };
            return new[] { Make(title, entries, search) };
        }

        /// <summary>Convenience: flatten <see cref="Sections"/> into one entry list (no section info).</summary>
        public static IReadOnlyList<FlagEntry> Filter(DemoSource source, string? search)
            => Sections(source, search).SelectMany(s => s.Entries).ToArray();

        private static FlagSection Make(string title, IReadOnlyList<FlagEntry> entries, string? search) {
            if (string.IsNullOrWhiteSpace(search))
                return new FlagSection(title, entries);
            var filtered = entries
                .Where(e => e.Code.IndexOf(search!, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
            return new FlagSection(title, filtered);
        }
    }
}
