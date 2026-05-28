using System;
using System.Collections.Generic;
using System.IO;
using Eto.Drawing;
using Eto.Forms;
using Flags.Icons;
using SkiaSharp;
using Svg.Skia;

namespace Flags.Icons.Eto {
    /// <summary>
    /// Eto.Forms <see cref="ImageView"/> subclass that renders a single flag SVG from one of the 5
    /// bundled sources. Set exactly one of <see cref="Twemoji"/>, <see cref="Circle"/>,
    /// <see cref="Square"/>, <see cref="Lipis"/>, <see cref="FlagHub"/> — assigning to one of them
    /// clears the others. SVGs are rasterized to PNG via <c>Svg.Skia</c> and cached per (source, value).
    /// </summary>
    public class FlagIcon : ImageView {
        public const int DefaultSvgRasterWidth = 512;
        public const int DefaultSvgRasterHeight = 384;

        private static readonly Dictionary<(FlagSource, int), Bitmap?> SvgCache = new();
        private static readonly object SvgCacheLock = new();

        private TwemojiFlag _twemoji = TwemojiFlag.None;
        private CircleFlag _circle = CircleFlag.None;
        private SquareFlag _square = SquareFlag.None;
        private LipisFlag _lipis = LipisFlag.None;
        private FlagHubFlag _flagHub = FlagHubFlag.None;
        private bool _suppress;

        public TwemojiFlag Twemoji {
            get => _twemoji;
            set { if (_twemoji == value) return; _twemoji = value; OnKindChanged(FlagSource.Twemoji); }
        }
        public CircleFlag Circle {
            get => _circle;
            set { if (_circle == value) return; _circle = value; OnKindChanged(FlagSource.Circle); }
        }
        public SquareFlag Square {
            get => _square;
            set { if (_square == value) return; _square = value; OnKindChanged(FlagSource.Square); }
        }
        public LipisFlag Lipis {
            get => _lipis;
            set { if (_lipis == value) return; _lipis = value; OnKindChanged(FlagSource.Lipis); }
        }
        public FlagHubFlag FlagHub {
            get => _flagHub;
            set { if (_flagHub == value) return; _flagHub = value; OnKindChanged(FlagSource.FlagHub); }
        }

        private void OnKindChanged(FlagSource changed) {
            if (_suppress) return;
            _suppress = true;
            try {
                if (changed != FlagSource.Twemoji && _twemoji != TwemojiFlag.None) _twemoji = TwemojiFlag.None;
                if (changed != FlagSource.Circle && _circle != CircleFlag.None) _circle = CircleFlag.None;
                if (changed != FlagSource.Square && _square != SquareFlag.None) _square = SquareFlag.None;
                if (changed != FlagSource.Lipis && _lipis != LipisFlag.None) _lipis = LipisFlag.None;
                if (changed != FlagSource.FlagHub && _flagHub != FlagHubFlag.None) _flagHub = FlagHubFlag.None;
            } finally {
                _suppress = false;
            }
            Image = LoadImage();
        }

        private Image? LoadImage() {
            var key = FlagSourceDispatch.GetActive(_twemoji, _circle, _square, _lipis, _flagHub);
            if (key == null) return null;
            return GetOrCreate(key.Value, () => FlagSourceDispatch.OpenActive(_twemoji, _circle, _square, _lipis, _flagHub));
        }

        private static Bitmap? GetOrCreate((FlagSource, int) key, Func<Stream?> openStream) {
            lock (SvgCacheLock) {
                if (SvgCache.TryGetValue(key, out var cached)) return cached;
                var bitmap = RasterizeSvg(openStream());
                SvgCache[key] = bitmap;
                return bitmap;
            }
        }

        private static Bitmap? RasterizeSvg(Stream? raw) {
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
                return new Bitmap(data.ToArray());
            }
        }
    }
}
