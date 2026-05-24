using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using Flags.Icons;
using SkiaSharp;
using Svg.Skia;

namespace Flags.Icons.Eto {
    /// <summary>
    /// Eto.Forms <see cref="ImageView"/> subclass that renders a flag for a given <see cref="FlagKind"/>.
    /// PNG kinds load directly via <see cref="Bitmap"/>. SVG kinds are rasterized to PNG via
    /// <c>Svg.Skia</c> at <see cref="DefaultSvgRasterWidth"/>×<see cref="DefaultSvgRasterHeight"/>
    /// and cached per kind so repeated layouts don't re-rasterize.
    /// </summary>
    public class FlagIcon : ImageView {
        public const int DefaultSvgRasterWidth = 512;
        public const int DefaultSvgRasterHeight = 384;

        private static readonly Dictionary<FlagKind, Bitmap?> SvgCache = new();
        private static readonly object SvgCacheLock = new();

        private FlagKind _kind = FlagKind.None;

        public FlagKind Kind {
            get => _kind;
            set {
                if (_kind == value) return;
                _kind = value;
                Image = LoadImage(value);
            }
        }

        private static Image? LoadImage(FlagKind kind) {
            if (kind == FlagKind.None) return null;
            if (FlagKindResolver.IsSvg(kind)) return GetOrCreateSvg(kind);
            var bytes = FlagAssetLoader.ReadAllBytes(kind);
            return bytes == null ? null : new Bitmap(bytes);
        }

        private static Bitmap? GetOrCreateSvg(FlagKind kind) {
            lock (SvgCacheLock) {
                if (SvgCache.TryGetValue(kind, out var cached)) return cached;
                var bitmap = RasterizeSvg(kind);
                SvgCache[kind] = bitmap;
                return bitmap;
            }
        }

        private static Bitmap? RasterizeSvg(FlagKind kind) {
            using var raw = FlagAssetLoader.OpenStream(kind);
            if (raw == null) return null;

            using var svg = new SKSvg();
            svg.Load(raw);
            var picture = svg.Picture;
            if (picture == null) return null;

            var bounds = picture.CullRect;
            if (bounds.Width <= 0 || bounds.Height <= 0) return null;

            const int w = DefaultSvgRasterWidth;
            const int h = DefaultSvgRasterHeight;
            var info = new SKImageInfo(w, h);
            using var surface = SKSurface.Create(info);
            surface.Canvas.Clear(SKColors.Transparent);

            var scale = Math.Min((float)w / bounds.Width, (float)h / bounds.Height);
            surface.Canvas.Translate(
                (w - bounds.Width * scale) / 2f,
                (h - bounds.Height * scale) / 2f);
            surface.Canvas.Scale(scale);
            surface.Canvas.DrawPicture(picture);
            surface.Canvas.Flush();

            using var snapshot = surface.Snapshot();
            using var data = snapshot.Encode(SKEncodedImageFormat.Png, 100);
            return new Bitmap(data.ToArray());
        }
    }
}
