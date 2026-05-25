using System;
using System.Collections.Generic;

namespace Flags.Icons {
    /// <summary>
    /// Runtime indexes for enumerating every bundled flag, per source. Each list excludes the
    /// <c>None</c> sentinel and is sorted by enum value.
    /// </summary>
    public static class FlagIcons {
        private static readonly Lazy<IReadOnlyList<TwemojiFlag>> TwemojiLazy = new(() => All<TwemojiFlag>(TwemojiFlag.None));
        private static readonly Lazy<IReadOnlyList<CircleFlag>> CircleLazy = new(() => All<CircleFlag>(CircleFlag.None));
        private static readonly Lazy<IReadOnlyList<SquareFlag>> SquareLazy = new(() => All<SquareFlag>(SquareFlag.None));
        private static readonly Lazy<IReadOnlyList<LipisFlag>> LipisLazy = new(() => All<LipisFlag>(LipisFlag.None));

        /// <summary>All <see cref="TwemojiFlag"/> members except <see cref="TwemojiFlag.None"/>.</summary>
        public static IReadOnlyList<TwemojiFlag> TwemojiFlags => TwemojiLazy.Value;

        /// <summary>All <see cref="CircleFlag"/> members except <see cref="CircleFlag.None"/>.</summary>
        public static IReadOnlyList<CircleFlag> CircleFlags => CircleLazy.Value;

        /// <summary>All <see cref="SquareFlag"/> members except <see cref="SquareFlag.None"/>.</summary>
        public static IReadOnlyList<SquareFlag> SquareFlags => SquareLazy.Value;

        /// <summary>All <see cref="LipisFlag"/> members except <see cref="LipisFlag.None"/>.</summary>
        public static IReadOnlyList<LipisFlag> LipisFlags => LipisLazy.Value;

        private static IReadOnlyList<T> All<T>(T none) where T : struct, Enum {
            var values = (T[])Enum.GetValues(typeof(T));
            var list = new List<T>(values.Length - 1);
            foreach (var v in values) if (!EqualityComparer<T>.Default.Equals(v, none)) list.Add(v);
            return list;
        }
    }
}
