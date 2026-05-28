using System;
using System.IO;
using Aprillz.MewUI;
using Flags.Icons;
using SkiaSharp;
using Svg.Skia;

namespace Flags.Icons.MewUi {
    /// <summary>
    /// Builds an Aprillz.MewUI <see cref="IImageSource"/> for a flag from one of the 5 bundled
    /// sources. One overload per source enum; pass the explicit raster size if you need a sharper
    /// result than the default 512×384.
    /// </summary>
    public static class FlagImageSource {
        public const int DefaultSvgRasterWidth = 512;
        public const int DefaultSvgRasterHeight = 384;

        public static IImageSource? For(TwemojiFlag flag) => For(flag, DefaultSvgRasterWidth, DefaultSvgRasterHeight);
        public static IImageSource? For(CircleFlag flag) => For(flag, DefaultSvgRasterWidth, DefaultSvgRasterHeight);
        public static IImageSource? For(SquareFlag flag) => For(flag, DefaultSvgRasterWidth, DefaultSvgRasterHeight);
        public static IImageSource? For(LipisFlag flag) => For(flag, DefaultSvgRasterWidth, DefaultSvgRasterHeight);
        public static IImageSource? For(FlagHubFlag flag) => For(flag, DefaultSvgRasterWidth, DefaultSvgRasterHeight);

        public static IImageSource? For(TwemojiFlag flag, int width, int height)
            => flag == TwemojiFlag.None ? null : Rasterize(FlagAssetLoader.OpenStream(flag), width, height);
        public static IImageSource? For(CircleFlag flag, int width, int height)
            => flag == CircleFlag.None ? null : Rasterize(FlagAssetLoader.OpenStream(flag), width, height);
        public static IImageSource? For(SquareFlag flag, int width, int height)
            => flag == SquareFlag.None ? null : Rasterize(FlagAssetLoader.OpenStream(flag), width, height);
        public static IImageSource? For(LipisFlag flag, int width, int height)
            => flag == LipisFlag.None ? null : Rasterize(FlagAssetLoader.OpenStream(flag), width, height);
        public static IImageSource? For(FlagHubFlag flag, int width, int height)
            => flag == FlagHubFlag.None ? null : Rasterize(FlagAssetLoader.OpenStream(flag), width, height);

        private static IImageSource? Rasterize(Stream? raw, int width, int height) {
            if (raw == null) return null;
            using (raw) {
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
}
