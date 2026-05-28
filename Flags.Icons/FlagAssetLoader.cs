using System.IO;
using System.Reflection;

namespace Flags.Icons {
    /// <summary>
    /// Reads the raw bytes for any of the 5 source enums (<see cref="TwemojiFlag"/>,
    /// <see cref="CircleFlag"/>, <see cref="SquareFlag"/>, <see cref="LipisFlag"/>,
    /// <see cref="FlagHubFlag"/>) from the <c>Flags.Icons</c> assembly's embedded manifest
    /// resources. Platform-specific Flags.Icons.* packages turn the resulting stream into their
    /// native image type (Avalonia <c>Bitmap</c>, WPF <c>BitmapImage</c>, etc.).
    /// </summary>
    public static class FlagAssetLoader {
        private const string TwemojiPrefix = "assets/twemoji/";
        private const string CirclePrefix = "assets/circle-flags/";
        private const string SquarePrefix = "assets/square-flags/";
        private const string LipisPrefix = "assets/flag-icons/";
        private const string FlagHubPrefix = "assets/flaghub/";

        private static readonly Assembly Assembly = typeof(FlagAssetLoader).GetTypeInfo().Assembly;

        /// <summary>Opens the embedded SVG stream for <paramref name="flag"/>. Returns <c>null</c> for <see cref="TwemojiFlag.None"/>.</summary>
        public static Stream? OpenStream(TwemojiFlag flag) => Open(TwemojiPrefix, TwemojiFlagFiles.GetFileName(flag));

        /// <summary>Opens the embedded SVG stream for <paramref name="flag"/>. Returns <c>null</c> for <see cref="CircleFlag.None"/>.</summary>
        public static Stream? OpenStream(CircleFlag flag) => Open(CirclePrefix, CircleFlagFiles.GetFileName(flag));

        /// <summary>Opens the embedded SVG stream for <paramref name="flag"/>. Returns <c>null</c> for <see cref="SquareFlag.None"/>.</summary>
        public static Stream? OpenStream(SquareFlag flag) => Open(SquarePrefix, SquareFlagFiles.GetFileName(flag));

        /// <summary>Opens the embedded SVG stream for <paramref name="flag"/>. Returns <c>null</c> for <see cref="LipisFlag.None"/>.</summary>
        public static Stream? OpenStream(LipisFlag flag) => Open(LipisPrefix, LipisFlagFiles.GetFileName(flag));

        /// <summary>Opens the embedded SVG stream for <paramref name="flag"/>. Returns <c>null</c> for <see cref="FlagHubFlag.None"/>.</summary>
        public static Stream? OpenStream(FlagHubFlag flag) => Open(FlagHubPrefix, FlagHubFlagFiles.GetFileName(flag));

        /// <summary>Reads the embedded SVG for <paramref name="flag"/> into a byte array.</summary>
        public static byte[]? ReadAllBytes(TwemojiFlag flag) => ReadAll(OpenStream(flag));

        /// <inheritdoc cref="ReadAllBytes(TwemojiFlag)"/>
        public static byte[]? ReadAllBytes(CircleFlag flag) => ReadAll(OpenStream(flag));

        /// <inheritdoc cref="ReadAllBytes(TwemojiFlag)"/>
        public static byte[]? ReadAllBytes(SquareFlag flag) => ReadAll(OpenStream(flag));

        /// <inheritdoc cref="ReadAllBytes(TwemojiFlag)"/>
        public static byte[]? ReadAllBytes(LipisFlag flag) => ReadAll(OpenStream(flag));

        /// <inheritdoc cref="ReadAllBytes(TwemojiFlag)"/>
        public static byte[]? ReadAllBytes(FlagHubFlag flag) => ReadAll(OpenStream(flag));

        private static Stream? Open(string prefix, string? fileName)
            => fileName == null ? null : Assembly.GetManifestResourceStream(prefix + fileName);

        private static byte[]? ReadAll(Stream? stream) {
            if (stream == null) return null;
            using (stream) {
                using var memory = new MemoryStream();
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
    }
}
