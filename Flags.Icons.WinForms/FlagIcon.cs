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
    /// Windows Forms <see cref="PictureBox"/> subclass that renders a single flag SVG from one of the
    /// 4 bundled sources. Set exactly one of <see cref="Twemoji"/>, <see cref="Circle"/>,
    /// <see cref="Square"/>, <see cref="Lipis"/> — assigning to one of them clears the others.
    /// SVGs are rasterized to PNG via <c>Svg.Skia</c> at <see cref="DefaultSvgRasterWidth"/>×
    /// <see cref="DefaultSvgRasterHeight"/> and cached per (source, value) so repeated layouts
    /// don't re-rasterize.
    /// </summary>
    public class FlagIcon : PictureBox {
        public const int DefaultSvgRasterWidth = 512;
        public const int DefaultSvgRasterHeight = 384;

        private static readonly Dictionary<(FlagSource, int), byte[]?> SvgPngCache = new();
        private static readonly object SvgCacheLock = new();

        private TwemojiFlag _twemoji = TwemojiFlag.None;
        private CircleFlag _circle = CircleFlag.None;
        private SquareFlag _square = SquareFlag.None;
        private LipisFlag _lipis = LipisFlag.None;

        public FlagIcon() {
            SizeMode = PictureBoxSizeMode.Zoom;
        }

        [DefaultValue(TwemojiFlag.None)]
        public TwemojiFlag Twemoji {
            get => _twemoji;
            set { if (_twemoji == value) return; _twemoji = value; OnKindChanged(FlagSource.Twemoji); }
        }
        [DefaultValue(CircleFlag.None)]
        public CircleFlag Circle {
            get => _circle;
            set { if (_circle == value) return; _circle = value; OnKindChanged(FlagSource.Circle); }
        }
        [DefaultValue(SquareFlag.None)]
        public SquareFlag Square {
            get => _square;
            set { if (_square == value) return; _square = value; OnKindChanged(FlagSource.Square); }
        }
        [DefaultValue(LipisFlag.None)]
        public LipisFlag Lipis {
            get => _lipis;
            set { if (_lipis == value) return; _lipis = value; OnKindChanged(FlagSource.Lipis); }
        }

        private bool _suppress;

        private void OnKindChanged(FlagSource changed) {
            if (_suppress) return;
            _suppress = true;
            try {
                if (changed != FlagSource.Twemoji && _twemoji != TwemojiFlag.None) _twemoji = TwemojiFlag.None;
                if (changed != FlagSource.Circle && _circle != CircleFlag.None) _circle = CircleFlag.None;
                if (changed != FlagSource.Square && _square != SquareFlag.None) _square = SquareFlag.None;
                if (changed != FlagSource.Lipis && _lipis != LipisFlag.None) _lipis = LipisFlag.None;
            } finally {
                _suppress = false;
            }
            Image?.Dispose();
            Image = LoadImage();
        }

        private Image? LoadImage() {
            var active = FlagSourceDispatch.GetActive(_twemoji, _circle, _square, _lipis);
            if (active == null) return null;
            var bytes = GetOrCreateSvgPng(active.Value, () => FlagSourceDispatch.OpenActive(_twemoji, _circle, _square, _lipis));
            if (bytes == null) return null;
            // Use a copy that owns its memory so the Image survives even if the cache is
            // mutated; Image.FromStream requires the stream to stay open for the lifetime
            // of the image, which a backing MemoryStream over a fresh array satisfies.
            return Image.FromStream(new MemoryStream(bytes, writable: false));
        }

        private static byte[]? GetOrCreateSvgPng((FlagSource, int) key, Func<Stream?> openStream) {
            lock (SvgCacheLock) {
                if (SvgPngCache.TryGetValue(key, out var cached)) return cached;
                var bytes = RasterizeSvg(openStream());
                SvgPngCache[key] = bytes;
                return bytes;
            }
        }

        private static byte[]? RasterizeSvg(Stream? raw) {
            if (raw == null) return null;
            using (raw) {
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
}
