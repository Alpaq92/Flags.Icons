using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Flags.Icons;

namespace Flags.Icons.MewUi {
    /// <summary>
    /// Fluent helpers for binding a <see cref="FlagKind"/> to an Aprillz.MewUI <see cref="Image"/>.
    /// </summary>
    public static class FlagIconExtensions {
        /// <summary>Sets <paramref name="image"/>.Source to the FlagKit asset for <paramref name="kind"/>.</summary>
        public static Image Flag(this Image image, FlagKind kind) {
            image.Source = FlagImageSource.For(kind);
            return image;
        }
    }
}
