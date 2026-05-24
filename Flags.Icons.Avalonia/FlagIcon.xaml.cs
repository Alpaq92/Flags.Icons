using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Svg.Skia;

namespace Flags.Icons.Avalonia {
    public class FlagIcon : TemplatedControl {
        public static readonly StyledProperty<FlagKind> KindProperty =
            AvaloniaProperty.Register<FlagIcon, FlagKind>(nameof(Kind), FlagKind.None);

        public static readonly DirectProperty<FlagIcon, IImage?> SourceProperty =
            AvaloniaProperty.RegisterDirect<FlagIcon, IImage?>(nameof(Source), icon => icon.Source);

        static FlagIcon() {
            KindProperty.Changed.AddClassHandler<FlagIcon>((x, _) => x.UpdateSource());
        }

        /// <summary>
        /// Which FlagKit asset to render. The enum is generated at build time from the files in
        /// <c>FlagKit/Assets/PNG</c> and <c>FlagKit/Assets/SVG</c>.
        /// </summary>
        public FlagKind Kind {
            get => GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        private IImage? _source;

        /// <summary>
        /// The currently-resolved <see cref="IImage"/> (a <see cref="Bitmap"/> for PNGs,
        /// an <see cref="SvgImage"/> for SVGs, or <c>null</c> when <see cref="Kind"/> is
        /// <see cref="FlagKind.None"/>).
        /// </summary>
        public IImage? Source {
            get => _source;
            private set => SetAndRaise(SourceProperty, ref _source, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            UpdateSource();
        }

        private void UpdateSource() {
            using var stream = FlagAssetLoader.OpenStream(Kind);
            if (stream == null) {
                Source = null;
                return;
            }

            if (FlagKindResolver.IsSvg(Kind)) {
                Source = new SvgImage { Source = SvgSource.LoadFromStream(stream) };
            }
            else {
                Source = new Bitmap(stream);
            }
        }
    }
}
