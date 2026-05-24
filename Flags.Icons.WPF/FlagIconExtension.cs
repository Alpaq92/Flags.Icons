using System;
using System.Windows.Markup;

namespace Flags.Icons.WPF {
    /// <summary>
    /// XAML markup extension for inline use, e.g. <c>{flag:FlagIconExtension USSVG}</c>.
    /// </summary>
    public class FlagIconExtension : MarkupExtension {
        public FlagIconExtension() { }

        public FlagIconExtension(FlagKind kind) {
            Kind = kind;
        }

        public FlagIconExtension(FlagKind kind, double size) {
            Kind = kind;
            Size = size;
        }

        [ConstructorArgument("kind")]
        public FlagKind Kind { get; set; } = FlagKind.None;

        [ConstructorArgument("size")]
        public double? Size { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            var icon = new FlagIcon { Kind = Kind };
            if (Size.HasValue) {
                icon.Width = Size.Value;
                icon.Height = Size.Value;
            }
            return icon;
        }
    }
}
