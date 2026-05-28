using System;
using System.Windows.Markup;

namespace Flags.Icons.WPF {
    /// <summary>
    /// XAML markup extension. Set exactly one of the source properties:
    /// <c>{flag:FlagIconExtension Twemoji=US, Size=24}</c>,
    /// <c>{flag:FlagIconExtension Circle=us, Size=24}</c>, etc.
    /// </summary>
    public class FlagIconExtension : MarkupExtension {
        public TwemojiFlag Twemoji { get; set; } = TwemojiFlag.None;
        public CircleFlag Circle { get; set; } = CircleFlag.None;
        public SquareFlag Square { get; set; } = SquareFlag.None;
        public LipisFlag Lipis { get; set; } = LipisFlag.None;
        public FlagHubFlag FlagHub { get; set; } = FlagHubFlag.None;
        public double? Size { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            var icon = new FlagIcon();
            if (Twemoji != TwemojiFlag.None) icon.Twemoji = Twemoji;
            else if (Circle != CircleFlag.None) icon.Circle = Circle;
            else if (Square != SquareFlag.None) icon.Square = Square;
            else if (Lipis != LipisFlag.None) icon.Lipis = Lipis;
            else if (FlagHub != FlagHubFlag.None) icon.FlagHub = FlagHub;
            if (Size.HasValue) { icon.Width = Size.Value; icon.Height = Size.Value; }
            return icon;
        }
    }
}
