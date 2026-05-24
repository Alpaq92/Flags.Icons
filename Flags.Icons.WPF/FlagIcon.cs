using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Flags.Icons.WPF {
    public class FlagIcon : Control {
        static FlagIcon() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlagIcon),
                new FrameworkPropertyMetadata(typeof(FlagIcon)));
        }

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(
                nameof(Kind),
                typeof(FlagKind),
                typeof(FlagIcon),
                new FrameworkPropertyMetadata(FlagKind.None, OnKindChanged));

        private static readonly DependencyPropertyKey SourcePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Source),
                typeof(ImageSource),
                typeof(FlagIcon),
                new FrameworkPropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty SourceProperty = SourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Which FlagKit asset to render. The enum is generated at build time from the files in
        /// <c>FlagKit/Assets/PNG</c> and <c>FlagKit/Assets/SVG</c>.
        /// </summary>
        public FlagKind Kind {
            get => (FlagKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        /// <summary>
        /// The currently-resolved <see cref="ImageSource"/> (a <see cref="BitmapImage"/> for PNGs,
        /// a <see cref="DrawingImage"/> for SVGs, or <c>null</c> when <see cref="Kind"/> is
        /// <see cref="FlagKind.None"/>).
        /// </summary>
        public ImageSource? Source {
            get => (ImageSource?)GetValue(SourceProperty);
            private set => SetValue(SourcePropertyKey, value);
        }

        private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((FlagIcon)d).UpdateSource();
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            UpdateSource();
        }

        private void UpdateSource() {
            using var stream = FlagAssetLoader.OpenStream(Kind);
            if (stream == null) {
                Source = null;
                return;
            }

            if (FlagKindResolver.IsSvg(Kind)) {
                var settings = new WpfDrawingSettings();
                var reader = new FileSvgReader(settings);
                var drawing = reader.Read(stream);
                Source = drawing != null ? new DrawingImage(drawing) : null;
            }
            else {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                Source = bitmap;
            }
        }
    }
}
