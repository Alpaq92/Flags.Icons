using System.IO;
using System.Reflection;

namespace Flags.Icons {
    /// <summary>
    /// Reads the raw bytes for a <see cref="FlagKind"/> from the <c>Flags.Icons</c> assembly's
    /// embedded manifest resources. Platform-specific Flags.Icons.* packages turn the resulting
    /// stream into their native image type (Avalonia <c>Bitmap</c>, WPF <c>BitmapImage</c>, etc.).
    /// </summary>
    public static class FlagAssetLoader {
        private static readonly Assembly Assembly = typeof(FlagAssetLoader).GetTypeInfo().Assembly;

        /// <summary>
        /// Opens the embedded asset stream for <paramref name="kind"/>. Returns <c>null</c>
        /// for <see cref="FlagKind.None"/>. Caller owns the returned stream.
        /// </summary>
        public static Stream? OpenStream(FlagKind kind) {
            var info = FlagKindResolver.GetInfo(kind);
            if (info == null) return null;
            return Assembly.GetManifestResourceStream(info.ResourceName);
        }

        /// <summary>Reads the embedded asset for <paramref name="kind"/> into a byte array.</summary>
        public static byte[]? ReadAllBytes(FlagKind kind) {
            using var stream = OpenStream(kind);
            if (stream == null) return null;
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            return memory.ToArray();
        }
    }
}
