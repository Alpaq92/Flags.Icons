using System;
using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Flags.Icons;

namespace Flags.Icons.MewUi {
    /// <summary>
    /// Convenience factory that returns a preconfigured <see cref="Image"/> rendering a flag from
    /// one of the 5 bundled sources. Provided for naming parity with the other Flags.Icons.*
    /// packages — Aprillz.MewUI's <see cref="Image"/> control is sealed, so this is a factory
    /// rather than a custom subclass.
    /// </summary>
    public static class FlagIcon {
        /// <summary>Oversampling multiplier applied to SVG kinds (raster size = ceil(displaySize × this)).</summary>
        public const int SvgOversample = 8;

        public static Image Create(TwemojiFlag flag) => NewImage().Flag(flag);
        public static Image Create(CircleFlag flag) => NewImage().Flag(flag);
        public static Image Create(SquareFlag flag) => NewImage().Flag(flag);
        public static Image Create(LipisFlag flag) => NewImage().Flag(flag);
        public static Image Create(FlagHubFlag flag) => NewImage().Flag(flag);

        public static Image Create(TwemojiFlag flag, double width, double height) {
            var (w, h) = OversampleRaster(width, height);
            var img = NewImage();
            img.Source = FlagImageSource.For(flag, w, h);
            return img.Width(width).Height(height);
        }
        public static Image Create(CircleFlag flag, double width, double height) {
            var (w, h) = OversampleRaster(width, height);
            var img = NewImage();
            img.Source = FlagImageSource.For(flag, w, h);
            return img.Width(width).Height(height);
        }
        public static Image Create(SquareFlag flag, double width, double height) {
            var (w, h) = OversampleRaster(width, height);
            var img = NewImage();
            img.Source = FlagImageSource.For(flag, w, h);
            return img.Width(width).Height(height);
        }
        public static Image Create(LipisFlag flag, double width, double height) {
            var (w, h) = OversampleRaster(width, height);
            var img = NewImage();
            img.Source = FlagImageSource.For(flag, w, h);
            return img.Width(width).Height(height);
        }
        public static Image Create(FlagHubFlag flag, double width, double height) {
            var (w, h) = OversampleRaster(width, height);
            var img = NewImage();
            img.Source = FlagImageSource.For(flag, w, h);
            return img.Width(width).Height(height);
        }

        private static Image NewImage() => new Image { ImageScaleQuality = ImageScaleQuality.HighQuality };

        private static (int w, int h) OversampleRaster(double width, double height) =>
            ((int)Math.Max(FlagImageSource.DefaultSvgRasterWidth, Math.Ceiling(width * SvgOversample)),
             (int)Math.Max(FlagImageSource.DefaultSvgRasterHeight, Math.Ceiling(height * SvgOversample)));
    }
}
