using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Flags.Icons;
using SkiaSharp;
using Svg.Skia;

namespace Flags.Icons.WinForms {
    /// <summary>
    /// Windows Forms <see cref="PictureBox"/> subclass that renders a flag for a given
    /// <see cref="FlagKind"/>. PNG kinds load directly via <see cref="Image.FromStream"/>.
    /// SVG kinds are rasterized to PNG via <c>Svg.Skia</c> at <see cref="DefaultSvgRasterWidth"/>×
    /// <see cref="DefaultSvgRasterHeight"/> and cached per kind so repeated layouts don't
    /// re-rasterize.
    /// </summary>
    public class FlagIcon : PictureBox {
        public const int DefaultSvgRasterWidth = 512;
        public const int DefaultSvgRasterHeight = 384;

        private static readonly Dictionary<FlagKind, byte[]?> SvgPngCache = new();
        private static readonly object SvgCacheLock = new();

        private FlagKind _kind = FlagKind.None;

        public FlagIcon() {
            SizeMode = PictureBoxSizeMode.Zoom;
        }

        [DefaultValue(FlagKind.None)]
        public FlagKind Kind {
            get => _kind;
            set {
                if (_kind == value) return;
                _kind = value;
                Image?.Dispose();
                Image = LoadImage(value);
            }
        }

        private static Image? LoadImage(FlagKind kind) {
            if (kind == FlagKind.None) return null;
            byte[]? bytes = FlagKindResolver.IsSvg(kind)
                ? GetOrCreateSvgPng(kind)
                : FlagAssetLoader.ReadAllBytes(kind);
            if (bytes == null) return null;
            // Use a copy that owns its memory so the Image survives even if the cache is
            // mutated; Image.FromStream requires the stream to stay open for the lifetime
            // of the image, which a backing MemoryStream over a fresh array satisfies.
            return Image.FromStream(new MemoryStream(bytes, writable: false));
        }

        private static byte[]? GetOrCreateSvgPng(FlagKind kind) {
            lock (SvgCacheLock) {
                if (SvgPngCache.TryGetValue(kind, out var cached)) return cached;
                var bytes = RasterizeSvg(kind);
                SvgPngCache[kind] = bytes;
                return bytes;
            }
        }

        private static byte[]? RasterizeSvg(FlagKind kind) {
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
            return data.ToArray();
        }
    }
}
