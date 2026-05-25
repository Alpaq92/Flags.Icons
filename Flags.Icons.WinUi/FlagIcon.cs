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
    /// WinUI 3 control that renders a single flag SVG from one of the 4 bundled sources. Set exactly
    /// one of <see cref="Twemoji"/>, <see cref="Circle"/>, <see cref="Square"/>, <see cref="Lipis"/> —
    /// assigning to one of them clears the others.
    /// </summary>
    public sealed class FlagIcon : ContentControl {
        public static readonly DependencyProperty TwemojiProperty = DependencyProperty.Register(
            nameof(Twemoji), typeof(TwemojiFlag), typeof(FlagIcon),
            new PropertyMetadata(TwemojiFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Twemoji)));

        public static readonly DependencyProperty CircleProperty = DependencyProperty.Register(
            nameof(Circle), typeof(CircleFlag), typeof(FlagIcon),
            new PropertyMetadata(CircleFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Circle)));

        public static readonly DependencyProperty SquareProperty = DependencyProperty.Register(
            nameof(Square), typeof(SquareFlag), typeof(FlagIcon),
            new PropertyMetadata(SquareFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Square)));

        public static readonly DependencyProperty LipisProperty = DependencyProperty.Register(
            nameof(Lipis), typeof(LipisFlag), typeof(FlagIcon),
            new PropertyMetadata(LipisFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Lipis)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(ImageSource), typeof(FlagIcon), new PropertyMetadata(null));

        private readonly Image _image;
        private bool _suppress;

        public FlagIcon() {
            Width = 24;
            Height = 18;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            _image = new Image { Stretch = Stretch.Uniform };
            Content = _image;
            SizeChanged += (_, _) => UpdateSvgRasterSize();
        }

        public TwemojiFlag Twemoji { get => (TwemojiFlag)GetValue(TwemojiProperty); set => SetValue(TwemojiProperty, value); }
        public CircleFlag Circle { get => (CircleFlag)GetValue(CircleProperty); set => SetValue(CircleProperty, value); }
        public SquareFlag Square { get => (SquareFlag)GetValue(SquareProperty); set => SetValue(SquareProperty, value); }
        public LipisFlag Lipis { get => (LipisFlag)GetValue(LipisProperty); set => SetValue(LipisProperty, value); }

        public ImageSource? Source {
            get => (ImageSource?)GetValue(SourceProperty);
            private set => SetValue(SourceProperty, value);
        }

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

        private void UpdateSvgRasterSize() {
            if (_image.Source is not SvgImageSource svg) return;
            var scale = XamlRoot?.RasterizationScale ?? 1.0;
            var w = ActualWidth > 0 ? ActualWidth : Width;
            var h = ActualHeight > 0 ? ActualHeight : Height;
            if (w <= 0 || h <= 0) return;
            svg.RasterizePixelWidth = w * scale;
            svg.RasterizePixelHeight = h * scale;
        }

        private void UpdateSource() {
            using var stream = FlagSourceDispatch.OpenActive(Twemoji, Circle, Square, Lipis);
            if (stream == null) {
                _image.Source = null;
                Source = null;
                return;
            }

            var ras = CopyToRandomAccessStream(stream);
            var svg = new SvgImageSource();
            var scale = XamlRoot?.RasterizationScale ?? 1.0;
            var w = ActualWidth > 0 ? ActualWidth : Width;
            var h = ActualHeight > 0 ? ActualHeight : Height;
            if (w > 0 && h > 0) {
                svg.RasterizePixelWidth = w * scale;
                svg.RasterizePixelHeight = h * scale;
            }
            _image.Source = svg;
            Source = svg;
            _ = LoadSvgAsync(svg, ras);
        }

        private static async Task LoadSvgAsync(SvgImageSource svg, IRandomAccessStream ras) {
            try { await svg.SetSourceAsync(ras); }
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
