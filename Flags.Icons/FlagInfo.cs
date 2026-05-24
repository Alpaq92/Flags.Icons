namespace Flags.Icons {
    /// <summary>
    /// Decoded metadata for a single <see cref="FlagKind"/> — the underlying country/region code,
    /// raster scale, file format, on-disk filename and the stable manifest-resource path.
    /// </summary>
    public sealed class FlagInfo {
        public FlagInfo(FlagKind kind, string code, FlagFormat format, FlagScale scale, string fileName, string resourceName) {
            Kind = kind;
            Code = code;
            Format = format;
            Scale = scale;
            FileName = fileName;
            ResourceName = resourceName;
        }

        public FlagKind Kind { get; }

        /// <summary>FlagKit country/region code with hyphens preserved (e.g. <c>US</c>, <c>GB-ENG</c>).</summary>
        public string Code { get; }

        public FlagFormat Format { get; }
        public FlagScale Scale { get; }

        /// <summary>FlagKit file name including <c>@2x</c>/<c>@3x</c> suffix (e.g. <c>US@2x.png</c>).</summary>
        public string FileName { get; }

        /// <summary>Manifest-resource name as embedded in <c>Flags.Icons.dll</c> (e.g. <c>assets/PNG/US@2x.png</c>).</summary>
        public string ResourceName { get; }
    }
}
