using System;
using System.IO;
using Flags.Icons;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using SkiaSharp;
using Svg.Skia;

namespace Flags.Icons.Maui {
    /// <summary>
    /// .NET MAUI view that renders a single flag SVG from one of the 4 bundled sources. Set exactly
    /// one of <see cref="Twemoji"/>, <see cref="Circle"/>, <see cref="Square"/>, <see cref="Lipis"/> —
    /// assigning to one of them clears the others. SVGs are rasterized to PNG at the control's
    /// actual pixel size (display density-aware) so the result stays crisp.
    /// </summary>
    public class FlagIcon : ContentView {
        public static readonly BindableProperty TwemojiProperty = BindableProperty.Create(
            nameof(Twemoji), typeof(TwemojiFlag), typeof(FlagIcon), TwemojiFlag.None,
            propertyChanged: (b, _, __) => ((FlagIcon)b).OnKindChanged(FlagSource.Twemoji));

        public static readonly BindableProperty CircleProperty = BindableProperty.Create(
            nameof(Circle), typeof(CircleFlag), typeof(FlagIcon), CircleFlag.None,
            propertyChanged: (b, _, __) => ((FlagIcon)b).OnKindChanged(FlagSource.Circle));

        public static readonly BindableProperty SquareProperty = BindableProperty.Create(
            nameof(Square), typeof(SquareFlag), typeof(FlagIcon), SquareFlag.None,
            propertyChanged: (b, _, __) => ((FlagIcon)b).OnKindChanged(FlagSource.Square));

        public static readonly BindableProperty LipisProperty = BindableProperty.Create(
            nameof(Lipis), typeof(LipisFlag), typeof(FlagIcon), LipisFlag.None,
            propertyChanged: (b, _, __) => ((FlagIcon)b).OnKindChanged(FlagSource.Lipis));

        private readonly Image _image = new Image { Aspect = Aspect.AspectFit };
        private FlagSource? _lastSource;
        private int _lastKindValue;
        private int _lastWidthPx;
        private int _lastHeightPx;
        private bool _suppress;

        public FlagIcon() {
            WidthRequest = 24;
            HeightRequest = 18;
            Content = _image;
            SizeChanged += (_, _) => Dispatcher.Dispatch(UpdateSource);
        }

        public TwemojiFlag Twemoji { get => (TwemojiFlag)GetValue(TwemojiProperty); set => SetValue(TwemojiProperty, value); }
        public CircleFlag Circle { get => (CircleFlag)GetValue(CircleProperty); set => SetValue(CircleProperty, value); }
        public SquareFlag Square { get => (SquareFlag)GetValue(SquareProperty); set => SetValue(SquareProperty, value); }
        public LipisFlag Lipis { get => (LipisFlag)GetValue(LipisProperty); set => SetValue(LipisProperty, value); }

        private void OnKindChanged(FlagSource changed) {
            if (_suppress) return;
            _suppress = true;
            try {
                if (changed != FlagSource.Twemoji && Twemoji != TwemojiFlag.None) Twemoji = TwemojiFlag.None;
                if (changed != FlagSource.Circle && Circle != CircleFlag.None) Circle = CircleFlag.None;
                if (changed != FlagSource.Square && Square != SquareFlag.None) Square = SquareFlag.None;
                if (changed != FlagSource.Lipis && Lipis != LipisFlag.None) Lipis = LipisFlag.None;
            } finally {
                _suppress = false;
            }
            UpdateSource();
        }

        private void UpdateSource() {
            var active = FlagSourceDispatch.GetActive(Twemoji, Circle, Square, Lipis);
            if (active == null) {
                _image.Source = null;
                _lastSource = null;
                _lastWidthPx = _lastHeightPx = 0;
                return;
            }

            if (Width <= 0 || Height <= 0) return;

            var density = DeviceDisplay.MainDisplayInfo.Density;
            if (density <= 0) density = 1;
            var w = (int)Math.Max(1, Math.Ceiling(Width * density));
            var h = (int)Math.Max(1, Math.Ceiling(Height * density));

            var (source, value) = active.Value;
            if (source == _lastSource && value == _lastKindValue && w == _lastWidthPx && h == _lastHeightPx) return;
            _lastSource = source;
            _lastKindValue = value;
            _lastWidthPx = w;
            _lastHeightPx = h;

            _image.Source = ImageSource.FromStream(() => RasterizeSvg(FlagSourceDispatch.OpenActive(Twemoji, Circle, Square, Lipis), w, h));
        }

        private static Stream RasterizeSvg(Stream? raw, int width, int height) {
            if (raw == null) return Stream.Null;
            using (raw) {
                using var svg = new SKSvg();
                svg.Load(raw);
                var picture = svg.Picture;
                if (picture == null) return Stream.Null;

                var bounds = picture.CullRect;
                if (bounds.Width <= 0 || bounds.Height <= 0) return Stream.Null;

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

                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                var output = new MemoryStream();
                data.SaveTo(output);
                output.Position = 0;
                return output;
            }
        }
    }
}
