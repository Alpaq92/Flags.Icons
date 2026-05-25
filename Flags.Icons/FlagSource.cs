namespace Flags.Icons {
    /// <summary>
    /// Discriminator for the 4 bundled upstream icon sources. Used by demos and any consumer that
    /// needs to switch between sources at runtime; controls expose one DependencyProperty per source
    /// for type-safe XAML, so this enum is purely informational at the control level.
    /// </summary>
    public enum FlagSource {
        /// <summary>jdecked/twemoji — country (1f1XX-1f1YY) + subdivision (1f3f4-…-e007f) emoji, filtered + ISO-renamed.</summary>
        Twemoji,
        /// <summary>HatScripts/circle-flags — circular flags, full upstream set including regional variants.</summary>
        Circle,
        /// <summary>kapowaz/square-flags — square flags, curated `flags/` subset.</summary>
        Square,
        /// <summary>lipis/flag-icons — rectangular 4:3 flags, full upstream set.</summary>
        Lipis,
    }
}
