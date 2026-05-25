using System.Collections.Generic;

namespace Flags.Demo.Shared {
    /// <summary>One per-source group of flag tiles in the demo grid.</summary>
    public class FlagSection {
        public FlagSection(string title, IReadOnlyList<FlagEntry> entries) {
            Title = title;
            Entries = entries;
        }
        public string Title { get; }
        public IReadOnlyList<FlagEntry> Entries { get; }
    }
}
