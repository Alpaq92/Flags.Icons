using System;
using System.IO;
using System.Threading.Tasks;
using Flags.Icons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace Flags.Icons.WinUi {
    /// <summary>
    /// WinUI 3 control that renders a flag icon for a given <see cref="FlagKind"/>. PNG kinds
    /// stream the embedded asset into a <see cref="BitmapImage"/>; SVG kinds use
    /// <see cref="SvgImageSource"/> so vector flags scale crisply at any size.
    /// </summary>
    public sealed class FlagIcon : ContentControl {
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(
                nameof(Kind),
                typeof(FlagKind),
                typeof(FlagIcon),
                new PropertyMetadata(FlagKind.None, (d, _) => ((FlagIcon)d).UpdateSource()));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(ImageSource),
                typeof(FlagIcon),
                new PropertyMetadata(null));

        private readonly Image _image;

        public FlagIcon() {
            Width = 24;
            Height = 18;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            _image = new Image { Stretch = Stretch.Uniform };
            Content = _image;
            SizeChanged += (_, _) => UpdateSvgRasterSize();
        }

        private void UpdateSvgRasterSize() {
            if (_image.Source is not SvgImageSource svg) return;
            var scale = XamlRoot?.RasterizationScale ?? 1.0;
            var w = ActualWidth > 0 ? ActualWidth : Width;
            var h = ActualHeight > 0 ? ActualHeight : Height;
            if (w <= 0 || h <= 0) return;
            svg.RasterizePixelWidth = w * scale;
            svg.RasterizePixelHeight = h * scale;
        }

        public FlagKind Kind {
            get => (FlagKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public ImageSource? Source {
            get => (ImageSource?)GetValue(SourceProperty);
            private set => SetValue(SourceProperty, value);
        }

        private void UpdateSource() {
            var kind = Kind;
            if (kind == FlagKind.None) {
                _image.Source = null;
                Source = null;
                return;
            }

            using var stream = FlagAssetLoader.OpenStream(kind);
            if (stream == null) {
                _image.Source = null;
                Source = null;
                return;
            }

            var ras = CopyToRandomAccessStream(stream);
            ImageSource source;
            if (FlagKindResolver.IsSvg(kind)) {
                var svg = new SvgImageSource();
                var scale = XamlRoot?.RasterizationScale ?? 1.0;
                var w = ActualWidth > 0 ? ActualWidth : Width;
                var h = ActualHeight > 0 ? ActualHeight : Height;
                if (w > 0 && h > 0) {
                    svg.RasterizePixelWidth = w * scale;
                    svg.RasterizePixelHeight = h * scale;
                }
                source = svg;
                _ = LoadSvgAsync(svg, ras);
            } else {
                var bitmap = new BitmapImage();
                source = bitmap;
                _ = LoadBitmapAsync(bitmap, ras);
            }

            _image.Source = source;
            Source = source;
        }

        private static async Task LoadSvgAsync(SvgImageSource svg, IRandomAccessStream ras) {
            try { await svg.SetSourceAsync(ras); }
            catch { }
            finally { ras.Dispose(); }
        }

        private static async Task LoadBitmapAsync(BitmapImage bitmap, IRandomAccessStream ras) {
            try { await bitmap.SetSourceAsync(ras); }
            catch { }
            finally { ras.Dispose(); }
        }

        private static IRandomAccessStream CopyToRandomAccessStream(Stream stream) {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms.AsRandomAccessStream();
        }
    }
}
