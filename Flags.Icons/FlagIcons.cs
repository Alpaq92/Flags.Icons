using System;
using System.Collections.Generic;

namespace Flags.Icons {
    /// <summary>
    /// Runtime helpers for enumerating the flag assets bundled with the library.
    /// </summary>
    public static class FlagIcons {
        private static readonly Lazy<IReadOnlyList<FlagKind>> AllLazy =
            new Lazy<IReadOnlyList<FlagKind>>(BuildAll);

        /// <summary>All <see cref="FlagKind"/> members except <see cref="FlagKind.None"/>.</summary>
        public static IReadOnlyList<FlagKind> AvailableKinds => AllLazy.Value;

        private static IReadOnlyList<FlagKind> BuildAll() {
            var values = (FlagKind[])Enum.GetValues(typeof(FlagKind));
            var list = new List<FlagKind>(values.Length);
            foreach (var v in values) {
                if (v != FlagKind.None) list.Add(v);
            }
            return list;
        }
    }
}
