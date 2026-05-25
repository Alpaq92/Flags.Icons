using System;
using Avalonia.Markup.Xaml;

namespace Flags.Icons.Avalonia {
    /// <summary>
    /// XAML markup extension. Set exactly one of the source properties:
    /// <c>{flag:FlagIconExt Twemoji=US, Size=24}</c>,
    /// <c>{flag:FlagIconExt Circle=us, Size=24}</c>, etc.
    /// </summary>
    public class FlagIconExt : MarkupExtension {
        public TwemojiFlag Twemoji { get; set; } = TwemojiFlag.None;
        public CircleFlag Circle { get; set; } = CircleFlag.None;
        public SquareFlag Square { get; set; } = SquareFlag.None;
        public LipisFlag Lipis { get; set; } = LipisFlag.None;
        public double? Size { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            var icon = new FlagIcon();
            if (Twemoji != TwemojiFlag.None) icon.Twemoji = Twemoji;
            else if (Circle != CircleFlag.None) icon.Circle = Circle;
            else if (Square != SquareFlag.None) icon.Square = Square;
            else if (Lipis != LipisFlag.None) icon.Lipis = Lipis;
            if (Size.HasValue) { icon.Width = Size.Value; icon.Height = Size.Value; }
            return icon;
        }
    }
}
