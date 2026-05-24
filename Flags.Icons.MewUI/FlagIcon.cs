using System;
using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Flags.Icons;

namespace Flags.Icons.MewUi {
    /// <summary>
    /// Convenience factory that returns a preconfigured <see cref="Image"/> rendering a flag for
    /// the given <see cref="FlagKind"/>. Provided for naming parity with the other Flags.Icons.*
    /// packages — Aprillz.MewUI's <see cref="Image"/> control is sealed, so this is a factory
    /// rather than a custom subclass.
    /// </summary>
    public static class FlagIcon {
        /// <summary>Oversampling multiplier applied to SVG kinds (raster size = ceil(displaySize × this)).</summary>
        public const int SvgOversample = 8;

        /// <summary>Creates an Image preconfigured to display <paramref name="kind"/>.</summary>
        public static Image Create(FlagKind kind)
            => new Image { ImageScaleQuality = ImageScaleQuality.HighQuality }.Flag(kind);

        /// <summary>
        /// Creates an Image with explicit pixel dimensions, preconfigured to display <paramref name="kind"/>.
        /// SVG kinds are rasterized at <paramref name="width"/>×<paramref name="height"/> × <see cref="SvgOversample"/>
        /// (with the FlagImageSource defaults as a floor) so they stay crisp when the Image is scaled down.
        /// </summary>
        public static Image Create(FlagKind kind, double width, double height) {
            var rasterW = (int)Math.Max(FlagImageSource.DefaultSvgRasterWidth, Math.Ceiling(width * SvgOversample));
            var rasterH = (int)Math.Max(FlagImageSource.DefaultSvgRasterHeight, Math.Ceiling(height * SvgOversample));
            var img = new Image { ImageScaleQuality = ImageScaleQuality.HighQuality };
            img.Source = FlagImageSource.For(kind, rasterW, rasterH);
            return img.Width(width).Height(height);
        }
    }
}
