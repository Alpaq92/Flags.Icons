using Flags.Icons;

namespace Flags.Demo.Shared {
    public class FlagEntry {
        public FlagEntry(FlagKind kind) {
            Kind = kind;
            var info = FlagKindResolver.GetInfo(kind)!;
            Code = info.Code;
            Variant = info.Format == FlagFormat.Svg ? DemoVariant.SVG
                : info.Scale switch {
                    FlagScale.X3 => DemoVariant.PNG3x,
                    FlagScale.X2 => DemoVariant.PNG2x,
                    _ => DemoVariant.PNG1x,
                };
        }

        public FlagKind Kind { get; }
        public string Code { get; }
        public DemoVariant Variant { get; }
    }
}
