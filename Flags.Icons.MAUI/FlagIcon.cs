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
    /// .NET MAUI view that renders a flag icon for a given <see cref="FlagKind"/>. PNG kinds
    /// stream the embedded asset directly into a <see cref="Image"/>; SVG kinds are rasterized
    /// to PNG at the control's actual pixel size (display density-aware) so the result stays
    /// crisp regardless of how small the source viewBox is.
    /// </summary>
    public class FlagIcon : ContentView {
        public static readonly BindableProperty KindProperty =
            BindableProperty.Create(
                nameof(Kind),
                typeof(FlagKind),
                typeof(FlagIcon),
                FlagKind.None,
                propertyChanged: (b, _, __) => ((FlagIcon)b).UpdateSource());

        private readonly Image _image = new Image { Aspect = Aspect.AspectFit };
        private FlagKind _lastKind;
        private int _lastWidthPx;
        private int _lastHeightPx;

        public FlagIcon() {
            WidthRequest = 24;
            HeightRequest = 18;
            Content = _image;
            SizeChanged += (_, _) => Dispatcher.Dispatch(UpdateSource);
        }

        public FlagKind Kind {
            get => (FlagKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        private void UpdateSource() {
            var kind = Kind;

            if (kind == FlagKind.None) {
                _image.Source = null;
                _lastKind = FlagKind.None;
                _lastWidthPx = _lastHeightPx = 0;
                return;
            }

            if (!FlagKindResolver.IsSvg(kind)) {
                _image.Source = ImageSource.FromStream(() => OpenPng(kind));
                _lastKind = kind;
                _lastWidthPx = _lastHeightPx = 0;
                return;
            }

            if (Width <= 0 || Height <= 0) return;

            var density = DeviceDisplay.MainDisplayInfo.Density;
            if (density <= 0) density = 1;
            var w = (int)Math.Max(1, Math.Ceiling(Width * density));
            var h = (int)Math.Max(1, Math.Ceiling(Height * density));

            if (kind == _lastKind && w == _lastWidthPx && h == _lastHeightPx) return;
            _lastKind = kind;
            _lastWidthPx = w;
            _lastHeightPx = h;

            _image.Source = ImageSource.FromStream(() => RasterizeSvg(kind, w, h));
        }

        private static Stream OpenPng(FlagKind kind) {
            using var raw = FlagAssetLoader.OpenStream(kind);
            if (raw == null) return Stream.Null;
            var memory = new MemoryStream();
            raw.CopyTo(memory);
            memory.Position = 0;
            return memory;
        }

        private static Stream RasterizeSvg(FlagKind kind, int width, int height) {
            using var raw = FlagAssetLoader.OpenStream(kind);
            if (raw == null) return Stream.Null;

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
