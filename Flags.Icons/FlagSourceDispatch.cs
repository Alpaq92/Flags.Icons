using System.IO;

namespace Flags.Icons {
    /// <summary>
    /// Helpers consumed by the per-stack <c>FlagIcon</c> controls, which all expose 5 typed
    /// source properties (Twemoji / Circle / Square / Lipis / FlagHub) and need to dispatch on whichever
    /// one is currently non-<c>None</c>. Extracted to keep the 8 control implementations DRY.
    /// </summary>
    public static class FlagSourceDispatch {
        /// <summary>
        /// Opens the embedded SVG stream for whichever of the 5 arguments is non-<c>None</c>, in
        /// priority order (Twemoji → Circle → Square → Lipis → FlagHub). Returns <c>null</c> if all are <c>None</c>.
        /// </summary>
        public static Stream? OpenActive(TwemojiFlag twemoji, CircleFlag circle, SquareFlag square, LipisFlag lipis, FlagHubFlag flagHub) {
            if (twemoji != TwemojiFlag.None) return FlagAssetLoader.OpenStream(twemoji);
            if (circle != CircleFlag.None) return FlagAssetLoader.OpenStream(circle);
            if (square != SquareFlag.None) return FlagAssetLoader.OpenStream(square);
            if (lipis != LipisFlag.None) return FlagAssetLoader.OpenStream(lipis);
            if (flagHub != FlagHubFlag.None) return FlagAssetLoader.OpenStream(flagHub);
            return null;
        }

        /// <summary>
        /// Returns <c>(source, enumValueAsInt)</c> for the active flag. Useful as a cache key
        /// in stacks that rasterize and cache by (source, value). Returns <c>null</c> if all
        /// 5 are <c>None</c>.
        /// </summary>
        public static (FlagSource source, int value)? GetActive(TwemojiFlag twemoji, CircleFlag circle, SquareFlag square, LipisFlag lipis, FlagHubFlag flagHub) {
            if (twemoji != TwemojiFlag.None) return (FlagSource.Twemoji, (int)twemoji);
            if (circle != CircleFlag.None) return (FlagSource.Circle, (int)circle);
            if (square != SquareFlag.None) return (FlagSource.Square, (int)square);
            if (lipis != LipisFlag.None) return (FlagSource.Lipis, (int)lipis);
            if (flagHub != FlagHubFlag.None) return (FlagSource.FlagHub, (int)flagHub);
            return null;
        }
    }
}
