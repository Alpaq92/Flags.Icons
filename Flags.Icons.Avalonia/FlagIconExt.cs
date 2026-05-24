using System;
using Avalonia.Markup.Xaml;

namespace Flags.Icons.Avalonia {
    public class FlagIconExt : MarkupExtension {
        public FlagIconExt() { }

        public FlagIconExt(FlagKind kind) {
            Kind = kind;
        }

        public FlagIconExt(FlagKind kind, double? size) {
            Kind = kind;
            Size = size;
        }

        [ConstructorArgument("kind")]
        public FlagKind Kind { get; set; } = FlagKind.None;

        [ConstructorArgument("size")]
        public double? Size { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            var icon = new FlagIcon {
                Kind = Kind,
            };

            if (Size.HasValue) {
                icon.Width = Size.Value;
                icon.Height = Size.Value;
            }

            return icon;
        }
    }
}
