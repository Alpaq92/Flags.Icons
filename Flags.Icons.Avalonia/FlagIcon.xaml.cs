using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Svg.Skia;

namespace Flags.Icons.Avalonia {
    /// <summary>
    /// Avalonia templated control that renders a single flag SVG from one of the 4 bundled sources.
    /// Set exactly one of <see cref="Twemoji"/>, <see cref="Circle"/>, <see cref="Square"/>,
    /// <see cref="Lipis"/> — assigning to one of them clears the others.
    /// </summary>
    public class FlagIcon : TemplatedControl {
        public static readonly StyledProperty<TwemojiFlag> TwemojiProperty =
            AvaloniaProperty.Register<FlagIcon, TwemojiFlag>(nameof(Twemoji), TwemojiFlag.None);

        public static readonly StyledProperty<CircleFlag> CircleProperty =
            AvaloniaProperty.Register<FlagIcon, CircleFlag>(nameof(Circle), CircleFlag.None);

        public static readonly StyledProperty<SquareFlag> SquareProperty =
            AvaloniaProperty.Register<FlagIcon, SquareFlag>(nameof(Square), SquareFlag.None);

        public static readonly StyledProperty<LipisFlag> LipisProperty =
            AvaloniaProperty.Register<FlagIcon, LipisFlag>(nameof(Lipis), LipisFlag.None);

        public static readonly DirectProperty<FlagIcon, IImage?> SourceProperty =
            AvaloniaProperty.RegisterDirect<FlagIcon, IImage?>(nameof(Source), icon => icon.Source);

        static FlagIcon() {
            TwemojiProperty.Changed.AddClassHandler<FlagIcon>((x, _) => x.OnKindChanged(FlagSource.Twemoji));
            CircleProperty.Changed.AddClassHandler<FlagIcon>((x, _) => x.OnKindChanged(FlagSource.Circle));
            SquareProperty.Changed.AddClassHandler<FlagIcon>((x, _) => x.OnKindChanged(FlagSource.Square));
            LipisProperty.Changed.AddClassHandler<FlagIcon>((x, _) => x.OnKindChanged(FlagSource.Lipis));
        }

        public TwemojiFlag Twemoji { get => GetValue(TwemojiProperty); set => SetValue(TwemojiProperty, value); }
        public CircleFlag Circle { get => GetValue(CircleProperty); set => SetValue(CircleProperty, value); }
        public SquareFlag Square { get => GetValue(SquareProperty); set => SetValue(SquareProperty, value); }
        public LipisFlag Lipis { get => GetValue(LipisProperty); set => SetValue(LipisProperty, value); }

        private IImage? _source;
        public IImage? Source {
            get => _source;
            private set => SetAndRaise(SourceProperty, ref _source, value);
        }

        private bool _suppress;

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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            UpdateSource();
        }

        private void UpdateSource() {
            using var stream = FlagSourceDispatch.OpenActive(Twemoji, Circle, Square, Lipis);
            Source = stream == null ? null : new SvgImage { Source = SvgSource.LoadFromStream(stream) };
        }
    }
}
