# Flags.Icons

<p align="center">
  <a href="https://www.nuget.org/packages/Flags.Icons"><img src="https://img.shields.io/nuget/v/Flags.Icons.svg?label=Flags.Icons" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.Avalonia"><img src="https://img.shields.io/nuget/v/Flags.Icons.Avalonia.svg?label=Avalonia" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.Eto"><img src="https://img.shields.io/nuget/v/Flags.Icons.Eto.svg?label=Eto" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.MAUI"><img src="https://img.shields.io/nuget/v/Flags.Icons.MAUI.svg?label=MAUI" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.MewUI"><img src="https://img.shields.io/nuget/v/Flags.Icons.MewUI.svg?label=MewUI" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.Uno"><img src="https://img.shields.io/nuget/v/Flags.Icons.Uno.svg?label=Uno" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.WinForms"><img src="https://img.shields.io/nuget/v/Flags.Icons.WinForms.svg?label=WinForms" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.WinUi"><img src="https://img.shields.io/nuget/v/Flags.Icons.WinUi.svg?label=WinUI" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons.WPF"><img src="https://img.shields.io/nuget/v/Flags.Icons.WPF.svg?label=WPF" alt="NuGet" /></a>
  <a href="https://github.com/Alpaq92/Flags.Icons/actions/workflows/ci.yml"><img src="https://img.shields.io/github/actions/workflow/status/Alpaq92/Flags.Icons/ci.yml?branch=main&label=CI" alt="CI" /></a>
  <a href="https://github.com/Alpaq92/Flags.Icons/actions/workflows/release.yml"><img src="https://img.shields.io/github/actions/workflow/status/Alpaq92/Flags.Icons/release.yml?branch=main&label=Release" alt="Release" /></a>
  <a href="https://www.nuget.org/packages/Flags.Icons"><img src="https://img.shields.io/nuget/dt/Flags.Icons.svg?label=Downloads&color=blue&cacheSeconds=3600" alt="NuGet downloads (core, transitively included by every platform package)" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License: MIT" /></a>
</p>

<p align="center">
  <img src="flag-icons-demo.png" alt="Flags.Icons demo" />
</p>

Flag icons from **4 upstream sources** — Twemoji, Circle (HatScripts), Square (kapowaz), Lipis (lipis/flag-icons) — packaged as drop-in controls for [Avalonia](https://github.com/AvaloniaUI/Avalonia), [Eto.Forms](https://github.com/picoe/Eto), [.NET MAUI](https://github.com/dotnet/maui), [Aprillz.MewUI](https://github.com/aprillz/MewUI), [Uno Platform](https://github.com/unoplatform/uno), [Windows Forms](https://github.com/dotnet/winforms), [WinUI 3](https://github.com/microsoft/WindowsAppSDK) and [WPF](https://github.com/dotnet/wpf). Every SVG ships as an embedded resource in the core `Flags.Icons` package, reachable through a per-source strongly-typed enum (`TwemojiFlag`, `CircleFlag`, `SquareFlag`, `LipisFlag`). No runtime download, no file-system access. Platform packages are thin wrappers that convert the embedded streams into the native image type for each UI stack.

> **v2 note:** v1 sourced everything from [madebybowtie/FlagKit](https://github.com/madebybowtie/FlagKit), which has been unmaintained since 2019 (no flag refreshes, no support for newer subdivision codes). v2 drops it in favour of 4 actively-maintained upstreams and replaces the single `FlagKind` enum with one typed enum per source. Migration: swap `<flag:FlagIcon Kind="USSVG"/>` for `<flag:FlagIcon Twemoji="US"/>` (or `Circle="us"` / `Square="us"` / `Lipis="us"` for the other styles). API surface details in the [Usage](#usage) section.

## Bundled sources

| Source                                                            | Style              | Count | Member name shape (ISO uppercase) |
|-------------------------------------------------------------------|--------------------|-------|-----------------------------------|
| [jdecked/twemoji](https://github.com/jdecked/twemoji)             | Emoji-style flat   | 262   | `TwemojiFlag.US`, `TwemojiFlag.GB_ENG` |
| [HatScripts/circle-flags](https://github.com/HatScripts/circle-flags) | Circular        | 430   | `CircleFlag.US`, `CircleFlag.AF_EMIRATE` |
| [kapowaz/square-flags](https://github.com/kapowaz/square-flags)   | Square             | 417   | `SquareFlag.US`                   |
| [lipis/flag-icons](https://github.com/lipis/flag-icons)           | Rectangular (4×3)  | 271   | `LipisFlag.US`                    |

All 4 sources live as git submodules under `submodules/`. The build pipeline (run on every `dotnet build`) syncs submodules → extracts SVGs into `assets/{source}/` (regenerated each build, gitignored) → embeds them as manifest resources → code-generates 4 strongly-typed enums.

## Packages

| Package                  | Description                                                  | Target framework(s)                          |
|--------------------------|--------------------------------------------------------------|----------------------------------------------|
| `Flags.Icons`            | Core: 4 generated enums, `FlagAssetLoader`, `FlagIcons` index | `netstandard2.0`                            |
| `Flags.Icons.Avalonia`   | Avalonia `FlagIcon` templated control                        | `net8.0`                                     |
| `Flags.Icons.Eto`        | Eto.Forms `FlagIcon` extending `ImageView`                   | `netstandard2.0`                             |
| `Flags.Icons.MAUI`       | .NET MAUI `FlagIcon` view                                    | `net10.0` + per-platform TFMs                |
| `Flags.Icons.MewUI`      | Aprillz.MewUI fluent helpers (`FlagIcon.Create` / `.Flag()`) | `net10.0`                                    |
| `Flags.Icons.Uno`        | Uno Platform `FlagIcon` control                              | `net10.0` + per-platform TFMs                |
| `Flags.Icons.WinForms`   | Windows Forms `FlagIcon` extending `PictureBox`              | `net8.0-windows`                             |
| `Flags.Icons.WinUi`      | WinUI 3 `FlagIcon` control                                   | `net8.0-windows10.0.19041.0`                 |
| `Flags.Icons.WPF`        | WPF `FlagIcon` control                                       | `net8.0-windows`                             |

Every bundled flag is SVG. Stacks without a native SVG type (Eto, MAUI, MewUI, WinForms) rasterize at runtime through [`Svg.Skia`](https://github.com/wieslawsoltes/Svg.Skia). Avalonia uses [`Svg.Controls.Skia.Avalonia`](https://github.com/wieslawsoltes/Svg.Skia), WPF uses `SharpVectors`, Uno/WinUI use the framework's `SvgImageSource`.

## Installation

```bash
dotnet add package Flags.Icons.Avalonia    # or .Eto / .MAUI / .MewUI / .Uno / .WinForms / .WinUi / .WPF
```

Every platform package transitively pulls in `Flags.Icons` core, so you don't need to reference it separately.

## Usage

`FlagIcon` exposes one property per source — `Twemoji`, `Circle`, `Square`, `Lipis`. Set exactly one; the others are cleared automatically.

### Avalonia

In `App.axaml`:

```xml
<StyleInclude Source="avares://Flags.Icons.Avalonia/App.xaml" />
```

Then:

```xml
<Window xmlns:flag="clr-namespace:Flags.Icons.Avalonia;assembly=Flags.Icons.Avalonia">
    <flag:FlagIcon Twemoji="US" Width="48" Height="36" />
    <flag:FlagIcon Circle="us" Width="48" Height="36" />
    <Button Content="{flag:FlagIconExt Lipis=fr, Size=24}" />
</Window>
```

### Eto.Forms

```csharp
using Flags.Icons;
using Flags.Icons.Eto;

var flag = new FlagIcon { Twemoji = TwemojiFlag.US, Size = new Size(48, 36) };
```

### .NET MAUI

```xml
<ContentPage xmlns:flag="clr-namespace:Flags.Icons.Maui;assembly=Flags.Icons.MAUI">
    <flag:FlagIcon Square="us" WidthRequest="48" HeightRequest="36" />
</ContentPage>
```

### Aprillz.MewUI

```csharp
using Flags.Icons;
using Flags.Icons.MewUi;

var flag = FlagIcon.Create(TwemojiFlag.US, 48, 36);
var img  = new Image().Flag(CircleFlag.fr).Width(24).Height(18);
```

### Uno Platform

```xml
<Page xmlns:flag="using:Flags.Icons.Uno">
    <flag:FlagIcon Lipis="us" Width="48" Height="36" />
</Page>
```

### Windows Forms

```csharp
using Flags.Icons;
using Flags.Icons.WinForms;

var flag = new FlagIcon { Twemoji = TwemojiFlag.US, Width = 48, Height = 36 };
```

### WinUI 3

```xml
<Window xmlns:flag="using:Flags.Icons.WinUi">
    <flag:FlagIcon Circle="us" Width="48" Height="36" />
</Window>
```

### WPF

```xml
<Window xmlns:flag="clr-namespace:Flags.Icons.WPF;assembly=Flags.Icons.WPF">
    <flag:FlagIcon Twemoji="US" Width="48" Height="36" />
</Window>
```

### Runtime discovery

```csharp
using Flags.Icons;

// Enumerate per source
foreach (var f in FlagIcons.TwemojiFlags) { /* TwemojiFlag values, e.g. US, FR, GB_ENG, ... */ }
foreach (var f in FlagIcons.CircleFlags)  { /* CircleFlag values  */ }
foreach (var f in FlagIcons.SquareFlags)  { /* SquareFlag values  */ }
foreach (var f in FlagIcons.LipisFlags)   { /* LipisFlag values   */ }

// Open the raw embedded SVG
using Stream? raw = FlagAssetLoader.OpenStream(TwemojiFlag.US);

// Original on-disk filename (preserves upstream casing)
string? fileName = TwemojiFlagFiles.GetFileName(TwemojiFlag.US);   // "US.svg"
string? fileName2 = CircleFlagFiles.GetFileName(CircleFlag.US);    // "us.svg"
```

## Demos

Each UI stack ships a demo that renders every flag in a wrapping grid with country-code search, source picker (Twemoji / Circle / Square / Lipis / All), section headers, and click-to-copy markup snippets.

```bash
dotnet run --project Flags.Icons.Avalonia.Demo
dotnet run --project Flags.Icons.Eto.Demo       # cross-platform
dotnet run --project Flags.Icons.MAUI.Demo      # Windows or macCatalyst
dotnet run --project Flags.Icons.MewUI.Demo     # cross-platform
dotnet run --project Flags.Icons.Uno.Demo       # desktop
dotnet run --project Flags.Icons.WinForms.Demo  # Windows
dotnet run --project Flags.Icons.WinUi.Demo     # Windows (x64/ARM64)
dotnet run --project Flags.Icons.WPF.Demo       # Windows
```

## Building from source

All 4 upstream sources are git submodules under `submodules/`. Clone with submodules:

```bash
git clone --recurse-submodules https://github.com/Alpaq92/Flags.Icons.git
```

…or after a plain clone (the first `dotnet build` will also auto-init if it sees `submodules/` empty):

```bash
git submodule update --init --recursive
```

### Submodule layout

| Path | Upstream | Build picks |
|---|---|---|
| `submodules/twemoji/` | jdecked/twemoji | `assets/svg/*.svg`, filtered to country (`1f1XX-1f1YY`) + subdivision (`1f3f4-…-e007f`) emoji, renamed to ISO codes |
| `submodules/circle-flags/` | HatScripts/circle-flags | `flags/*.svg` (full set; ~24 Windows-clone symlink stubs auto-resolved) |
| `submodules/square-flags/` | kapowaz/square-flags | `flags/*.svg` (curated subset, not `flags-original/`) |
| `submodules/flag-icons/` | lipis/flag-icons | `flags/4x3/*.svg` |

### Build pipeline

Every `dotnet build` runs `build/Flags.Icons.Assets.targets`, which orchestrates one shared `StageFlagAssets` MSBuild task across all 4 sources:

1. **Sync submodules** — local-only auto-init if `submodules/` is empty (CI's `actions/checkout submodules:true` already does this). Manual force-sync: `dotnet msbuild Flags.Icons/Flags.Icons.csproj -t:SyncSubmodules`.
2. **Stage assets** — one `StageFlagAssets` invocation per source. Two modes:
   - `Mode="TwemojiFilter"` for twemoji — regex-filters for country + subdivision codepoints and renames to ISO codes.
   - `Mode="RawCopy"` for the other 3 — copies every `*.svg` verbatim, resolving Windows-clone symlink stubs along the way.

   Both modes share write-if-different + prune-stale logic in the same task, so subsequent builds keep stable mtimes (incremental-friendly) and upstream-removed files disappear from `assets/`. `assets/` is **gitignored** — pure build output.
3. **Embed** — each `assets/{source}/{filename}.svg` becomes a manifest resource of the same path.
4. **Generate enums** — `TwemojiFlag.g.cs`, `CircleFlag.g.cs`, `SquareFlag.g.cs`, `LipisFlag.g.cs` in `obj/`, each with the enum + a `{Source}FlagFiles.GetFileName(flag)` lookup that preserves upstream filename casing. `_GenerateFlagEnums` short-circuits via `Inputs`/`Outputs` when the staged set is unchanged.

Runtime helpers in `Flags.Icons` (the core package): `FlagAssetLoader.OpenStream` has 4 typed overloads (one per source enum); `FlagSourceDispatch.OpenActive`/`GetActive` is the shared dispatch the 8 per-stack `FlagIcon` controls use to walk their 4 source DPs and pick the active one.

If a submodule is missing (e.g. CI checked out without submodules), the corresponding enum will only contain `None = 0` and a warning is emitted.

## License

[MIT](LICENSE) for source code. Bundled flag SVGs come from:

- [jdecked/twemoji](https://github.com/jdecked/twemoji) — graphics [CC-BY 4.0](https://creativecommons.org/licenses/by/4.0/) (attribution: Twemoji © Twitter, Inc / jdecked contributors), code [MIT](https://github.com/jdecked/twemoji/blob/main/LICENSE).
- [HatScripts/circle-flags](https://github.com/HatScripts/circle-flags) — [MIT](https://github.com/HatScripts/circle-flags/blob/master/LICENSE.md).
- [kapowaz/square-flags](https://github.com/kapowaz/square-flags) — [MIT](https://github.com/kapowaz/square-flags/blob/main/LICENSE.md).
- [lipis/flag-icons](https://github.com/lipis/flag-icons) — [MIT](https://github.com/lipis/flag-icons/blob/main/LICENSE).

The project icon (`icon.png`) is from the [Jellyfin UX icon set](https://github.com/jellyfin/jellyfin-ux), licensed under [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).

## Credits

- **Flag assets** — [jdecked/twemoji](https://github.com/jdecked/twemoji), [HatScripts/circle-flags](https://github.com/HatScripts/circle-flags), [kapowaz/square-flags](https://github.com/kapowaz/square-flags), [lipis/flag-icons](https://github.com/lipis/flag-icons)
- **Icon** — [Jellyfin UX](https://github.com/jellyfin/jellyfin-ux) (CC BY-SA 4.0)
- **Inspiration** — [Material.Icons.Avalonia](https://github.com/SKProCH/Material.Icons) — templated-control + markup-extension pattern adapted from this project

## Development

Parts of this codebase — the per-platform `FlagIcon` wrappers, demo scaffolding, and CI/CD configuration — were developed with assistance from an audit-based AI workflow, with each change reviewed and verified by a human maintainer before being merged. Bug reports and PRs are welcome.

---

*Release history in [CHANGELOG.md](CHANGELOG.md). Versions are driven by [Conventional Commits](https://www.conventionalcommits.org/) via [release-please](https://github.com/googleapis/release-please); branch ruleset details in [`.github/rulesets/README.md`](.github/rulesets/README.md).*
