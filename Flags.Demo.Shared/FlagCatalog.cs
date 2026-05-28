using System;
using System.Collections.Generic;
using System.Linq;
using Flags.Icons;

namespace Flags.Demo.Shared {
    /// <summary>
    /// Demo-side catalog of every flag from every source. Always returns a list of
    /// <see cref="FlagSection"/>s so XAML and code-first demos can render section headers
    /// uniformly (single-source views are 1 section; <see cref="DemoSource.All"/> is N).
    /// </summary>
    public static class FlagCatalog {
        // Adding a new bundled source = one row here.
        private static readonly IReadOnlyList<SourceDescriptor> Descriptors = new SourceDescriptor[] {
            new(DemoSource.Twemoji, "Twemoji",                          () => FlagIcons.TwemojiFlags.Select(f => new FlagEntry(f))),
            new(DemoSource.Circle,  "Circle (HatScripts)",              () => FlagIcons.CircleFlags.Select(f => new FlagEntry(f))),
            new(DemoSource.Square,  "Square (kapowaz)",                 () => FlagIcons.SquareFlags.Select(f => new FlagEntry(f))),
            new(DemoSource.Lipis,   "Lipis (lipis/flag-icons 4x3)",     () => FlagIcons.LipisFlags.Select(f => new FlagEntry(f))),
            new(DemoSource.FlagHub, "FlagHub (Alpaq92 — FlagKit fork)", () => FlagIcons.FlagHubFlags.Select(f => new FlagEntry(f))),
        };

        /// <summary>Bundled sources in display order, plus the <see cref="DemoSource.All"/> umbrella as the last entry.</summary>
        public static IReadOnlyList<DemoSource> AllSources { get; } =
            Descriptors.Select(d => d.Source).Append(DemoSource.All).ToArray();

        public static IReadOnlyList<FlagSection> Sections(DemoSource source, string? search) =>
            (source == DemoSource.All ? Descriptors : Descriptors.Where(d => d.Source == source))
                .Select(d => d.MakeSection(search))
                .ToArray();

        /// <summary>Convenience: flatten <see cref="Sections"/> into one entry list (no section info).</summary>
        public static IReadOnlyList<FlagEntry> Filter(DemoSource source, string? search)
            => Sections(source, search).SelectMany(s => s.Entries).ToArray();

        private sealed class SourceDescriptor {
            private readonly Lazy<IReadOnlyList<FlagEntry>> _entries;

            public SourceDescriptor(DemoSource source, string title, Func<IEnumerable<FlagEntry>> entryFactory) {
                Source = source;
                Title = title;
                _entries = new Lazy<IReadOnlyList<FlagEntry>>(
                    () => entryFactory().OrderBy(e => e.Code, StringComparer.Ordinal).ToArray());
            }

            public DemoSource Source { get; }
            public string Title { get; }

            public FlagSection MakeSection(string? search) {
                var entries = _entries.Value;
                if (!string.IsNullOrWhiteSpace(search)) {
                    entries = entries.Where(e => e.Code.IndexOf(search!, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                }
                return new FlagSection(Title, entries);
            }
        }
    }
}
