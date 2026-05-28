using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Flags.Icons.WPF {
    /// <summary>
    /// WPF control that renders a single flag SVG from one of the 5 bundled sources. Set exactly
    /// one of <see cref="Twemoji"/>, <see cref="Circle"/>, <see cref="Square"/>, <see cref="Lipis"/>,
    /// <see cref="FlagHub"/> — assigning to one of them clears the others.
    /// </summary>
    public class FlagIcon : Control {
        static FlagIcon() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlagIcon), new FrameworkPropertyMetadata(typeof(FlagIcon)));
        }

        public static readonly DependencyProperty TwemojiProperty = DependencyProperty.Register(
            nameof(Twemoji), typeof(TwemojiFlag), typeof(FlagIcon),
            new FrameworkPropertyMetadata(TwemojiFlag.None, (d, e) => ((FlagIcon)d).OnKindChanged(FlagSource.Twemoji)));

        public static readonly DependencyProperty CircleProperty = DependencyProperty.Register(
            nameof(Circle), typeof(CircleFlag), typeof(FlagIcon),
            new FrameworkPropertyMetadata(CircleFlag.None, (d, e) => ((FlagIcon)d).OnKindChanged(FlagSource.Circle)));

        public static readonly DependencyProperty SquareProperty = DependencyProperty.Register(
            nameof(Square), typeof(SquareFlag), typeof(FlagIcon),
            new FrameworkPropertyMetadata(SquareFlag.None, (d, e) => ((FlagIcon)d).OnKindChanged(FlagSource.Square)));

        public static readonly DependencyProperty LipisProperty = DependencyProperty.Register(
            nameof(Lipis), typeof(LipisFlag), typeof(FlagIcon),
            new FrameworkPropertyMetadata(LipisFlag.None, (d, e) => ((FlagIcon)d).OnKindChanged(FlagSource.Lipis)));

        public static readonly DependencyProperty FlagHubProperty = DependencyProperty.Register(
            nameof(FlagHub), typeof(FlagHubFlag), typeof(FlagIcon),
            new FrameworkPropertyMetadata(FlagHubFlag.None, (d, e) => ((FlagIcon)d).OnKindChanged(FlagSource.FlagHub)));

        private static readonly DependencyPropertyKey SourcePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Source), typeof(ImageSource), typeof(FlagIcon),
            new FrameworkPropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty SourceProperty = SourcePropertyKey.DependencyProperty;

        /// <summary>Twemoji (jdecked/twemoji) flag to render.</summary>
        public TwemojiFlag Twemoji { get => (TwemojiFlag)GetValue(TwemojiProperty); set => SetValue(TwemojiProperty, value); }
        /// <summary>Circle (HatScripts/circle-flags) flag to render.</summary>
        public CircleFlag Circle { get => (CircleFlag)GetValue(CircleProperty); set => SetValue(CircleProperty, value); }
        /// <summary>Square (kapowaz/square-flags) flag to render.</summary>
        public SquareFlag Square { get => (SquareFlag)GetValue(SquareProperty); set => SetValue(SquareProperty, value); }
        /// <summary>Lipis (lipis/flag-icons 4x3) flag to render.</summary>
        public LipisFlag Lipis { get => (LipisFlag)GetValue(LipisProperty); set => SetValue(LipisProperty, value); }
        /// <summary>FlagHub (Alpaq92/FlagHub — maintained FlagKit fork) flag to render.</summary>
        public FlagHubFlag FlagHub { get => (FlagHubFlag)GetValue(FlagHubProperty); set => SetValue(FlagHubProperty, value); }

        /// <summary>The currently-resolved <see cref="ImageSource"/> for whichever source-DP is non-<c>None</c>.</summary>
        public ImageSource? Source {
            get => (ImageSource?)GetValue(SourceProperty);
            private set => SetValue(SourcePropertyKey, value);
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
                if (changed != FlagSource.FlagHub && FlagHub != FlagHubFlag.None) FlagHub = FlagHubFlag.None;
            } finally {
                _suppress = false;
            }
            UpdateSource();
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            UpdateSource();
        }

        private void UpdateSource() {
            using var stream = FlagSourceDispatch.OpenActive(Twemoji, Circle, Square, Lipis, FlagHub);
            if (stream == null) { Source = null; return; }
            var reader = new FileSvgReader(new WpfDrawingSettings());
            var drawing = reader.Read(stream);
            Source = drawing != null ? new DrawingImage(drawing) : null;
        }
    }
}
