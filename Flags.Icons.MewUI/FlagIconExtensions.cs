using Aprillz.MewUI.Controls;
using Flags.Icons;

namespace Flags.Icons.MewUi {
    /// <summary>
    /// Fluent helpers for binding a flag from any of the 4 bundled sources to an Aprillz.MewUI
    /// <see cref="Image"/>.
    /// </summary>
    public static class FlagIconExtensions {
        public static Image Flag(this Image image, TwemojiFlag flag) { image.Source = FlagImageSource.For(flag); return image; }
        public static Image Flag(this Image image, CircleFlag flag) { image.Source = FlagImageSource.For(flag); return image; }
        public static Image Flag(this Image image, SquareFlag flag) { image.Source = FlagImageSource.For(flag); return image; }
        public static Image Flag(this Image image, LipisFlag flag) { image.Source = FlagImageSource.For(flag); return image; }
    }
}
