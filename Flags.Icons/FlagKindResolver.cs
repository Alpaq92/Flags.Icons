using System;

namespace Flags.Icons {
    /// <summary>
    /// Decodes <see cref="FlagKind"/> enum members into the metadata that backs them. The enum
    /// names follow <c>{Code}[{Scale}]{Format}</c>, e.g. <c>USPNG</c>, <c>US2xPNG</c>, <c>USSVG</c>,
    /// <c>GB_ENG3xPNG</c>. Hyphens in FlagKit codes are encoded as underscores in the enum.
    /// </summary>
    public static class FlagKindResolver {
        public static FlagInfo? GetInfo(FlagKind kind) {
            if (kind == FlagKind.None) return null;
            var name = kind.ToString();

            if (TryStrip(name, "3xPNG", out var core)) return Build(kind, core, FlagFormat.Png, FlagScale.X3, "@3x.png", "assets/PNG/");
            if (TryStrip(name, "2xPNG", out core)) return Build(kind, core, FlagFormat.Png, FlagScale.X2, "@2x.png", "assets/PNG/");
            if (TryStrip(name, "PNG", out core)) return Build(kind, core, FlagFormat.Png, FlagScale.X1, ".png", "assets/PNG/");
            if (TryStrip(name, "SVG", out core)) return Build(kind, core, FlagFormat.Svg, FlagScale.X1, ".svg", "assets/SVG/");
            return null;
        }

        public static bool IsSvg(FlagKind kind) => GetInfo(kind)?.Format == FlagFormat.Svg;

        public static string? GetResourceName(FlagKind kind) => GetInfo(kind)?.ResourceName;

        private static bool TryStrip(string name, string suffix, out string core) {
            if (name.EndsWith(suffix, StringComparison.Ordinal)) {
                core = name.Substring(0, name.Length - suffix.Length);
                return true;
            }
            core = string.Empty;
            return false;
        }

        private static FlagInfo Build(FlagKind kind, string core, FlagFormat format, FlagScale scale, string fileSuffix, string resourcePrefix) {
            var code = core.Replace('_', '-');
            var fileName = code + fileSuffix;
            return new FlagInfo(kind, code, format, scale, fileName, resourcePrefix + fileName);
        }
    }
}
