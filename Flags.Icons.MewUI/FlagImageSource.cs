using System;
using Aprillz.MewUI;
using Flags.Icons;
using SkiaSharp;
using Svg.Skia;

namespace Flags.Icons.MewUi {
    /// <summary>
    /// Builds an Aprillz.MewUI <see cref="IImageSource"/> for a given <see cref="FlagKind"/>.
    /// PNG kinds stream the embedded asset directly. SVG kinds are rasterized to PNG via
    /// <c>Svg.Skia</c> at <see cref="DefaultSvgRasterWidth"/>×<see cref="DefaultSvgRasterHeight"/>
    /// (4:3, comfortably above typical UI sizes); pass an explicit size to <see cref="For(FlagKind, int, int)"/>
    /// when you need a sharper raster.
    /// </summary>
    public static class FlagImageSource {
        public const int DefaultSvgRasterWidth = 512;
        public const int DefaultSvgRasterHeight = 384;

        public static IImageSource? For(FlagKind kind)
            => For(kind, DefaultSvgRasterWidth, DefaultSvgRasterHeight);

        public static IImageSource? For(FlagKind kind, int svgRasterWidth, int svgRasterHeight) {
            if (kind == FlagKind.None) return null;
            if (FlagKindResolver.IsSvg(kind)) return RasterizeSvg(kind, svgRasterWidth, svgRasterHeight);
            var bytes = FlagAssetLoader.ReadAllBytes(kind);
            return bytes == null ? null : ImageSource.FromBytes(bytes);
        }

        private static IImageSource? RasterizeSvg(FlagKind kind, int width, int height) {
            using var raw = FlagAssetLoader.OpenStream(kind);
            if (raw == null) return null;

            using var svg = new SKSvg();
            svg.Load(raw);
            var picture = svg.Picture;
            if (picture == null) return null;

            var bounds = picture.CullRect;
            if (bounds.Width <= 0 || bounds.Height <= 0) return null;

            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            surface.Canvas.Clear(SKColors.Transparent);

            var scale = Math.Min(width / bounds.Width, height / bounds.Height);
            surface.Canvas.Translate(
                (width - bounds.Width * scale) / 2f,
                (height - bounds.Height * scale) / 2f);
            surface.Canvas.Scale(scale);
            surface.Canvas.DrawPicture(picture);
            surface.Canvas.Flush();

            using var snapshot = surface.Snapshot();
            using var data = snapshot.Encode(SKEncodedImageFormat.Png, 100);
            return ImageSource.FromBytes(data.ToArray());
        }
    }
}
