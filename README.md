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
  <a href="https://www.nuget.org/packages/Flags.Icons"><img src="https://img.shields.io/nuget/dt/Flags.Icons.svg?label=Downloads&color=informational" alt="NuGet downloads (core, transitively included by every platform package)" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License: MIT" /></a>
</p>

<p align="center">
  <img src="flag-icons-demo.png" alt="Flags.Icons demo" />
</p>

> **Note:** the README packaged with the NuGet packages is the shorter [`nuget-readme.md`](nuget-readme.md). This file is the GitHub source of truth.

Country flag icons from [madebybowtie/FlagKit](https://github.com/madebybowtie/FlagKit), packaged as drop-in controls for [Avalonia](https://github.com/AvaloniaUI/Avalonia), [Eto.Forms](https://github.com/picoe/Eto), [.NET MAUI](https://github.com/dotnet/maui), [Aprillz.MewUI](https://github.com/aprillz/MewUI), [Uno Platform](https://github.com/unoplatform/uno), [Windows Forms](https://github.com/dotnet/winforms), [WinUI 3](https://github.com/microsoft/WindowsAppSDK) and [WPF](https://github.com/dotnet/wpf). Every PNG (`@1x`/`@2x`/`@3x`) and SVG ships as an embedded resource in the core `Flags.Icons` package — **~1020 assets across 255 country codes** — reachable through a generated `FlagKind` enum. No runtime download, no file-system access. Platform packages are thin wrappers that convert the embedded streams into the native image type for each UI stack.

## Packages

| Package                  | Description                                                  | Target framework(s)                          |
|--------------------------|--------------------------------------------------------------|----------------------------------------------|
| `Flags.Icons`            | Core: `FlagKind`, asset streams, resolver helpers            | `netstandard2.0`                             |
| `Flags.Icons.Avalonia`   | Avalonia `FlagIcon` templated control                        | `net8.0`                                     |
| `Flags.Icons.Eto`        | Eto.Forms `FlagIcon` extending `ImageView`                   | `netstandard2.0`                             |
| `Flags.Icons.MAUI`       | .NET MAUI `FlagIcon` view                                    | `net10.0` + per-platform TFMs                |
| `Flags.Icons.MewUI`      | Aprillz.MewUI fluent helpers (`FlagIcon.Create` / `.Flag()`) | `net10.0`                                    |
| `Flags.Icons.Uno`        | Uno Platform `FlagIcon` control                              | `net10.0` + per-platform TFMs                |
| `Flags.Icons.WinForms`   | Windows Forms `FlagIcon` extending `PictureBox`              | `net8.0-windows`                             |
| `Flags.Icons.WinUi`      | WinUI 3 `FlagIcon` control                                   | `net8.0-windows10.0.19041.0`                 |
| `Flags.Icons.WPF`        | WPF `FlagIcon` control                                       | `net8.0-windows`                             |

SVG rendering on stacks without a native SVG type (Eto, MAUI, MewUI, WinForms) goes through [`Svg.Skia`](https://github.com/wieslawsoltes/Svg.Skia). Avalonia uses [`Svg.Controls.Skia.Avalonia`](https://github.com/wieslawsoltes/Svg.Skia), WPF uses `SharpVectors`, Uno/WinUI use the framework's `SvgImageSource`.

## Installation

```bash
dotnet add package Flags.Icons.Avalonia    # or .Eto / .MAUI / .MewUI / .Uno / .WinForms / .WinUi / .WPF
```

Every platform package transitively pulls in `Flags.Icons` core, so you don't need to reference it separately.

## Usage

`FlagKind` members encode the country code, scale (for PNGs), and format. Hyphens in FlagKit file names become underscores (C# identifiers don't allow `-`).

| FlagKit file               | `FlagKind` member |
|----------------------------|-------------------|
| `Assets/PNG/US.png`        | `USPNG`           |
| `Assets/PNG/US@2x.png`     | `US2xPNG`         |
| `Assets/PNG/US@3x.png`     | `US3xPNG`         |
| `Assets/SVG/US.svg`        | `USSVG`           |
| `Assets/PNG/GB-ENG@2x.png` | `GB_ENG2xPNG`     |

### Avalonia

In `App.axaml`:

```xml
<StyleInclude Source="avares://Flags.Icons.Avalonia/App.xaml" />
```

Then:

```xml
<Window xmlns:flag="clr-namespace:Flags.Icons.Avalonia;assembly=Flags.Icons.Avalonia">
    <flag:FlagIcon Kind="USSVG" Width="48" Height="36" />
    <Button Content="{flag:FlagIconExt FR2xPNG, 24}" />
</Window>
```

### Eto.Forms

```csharp
using Flags.Icons;
using Flags.Icons.Eto;

var flag = new FlagIcon { Kind = FlagKind.USSVG, Size = new Size(48, 36) };
```

### .NET MAUI

```xml
<ContentPage xmlns:flag="clr-namespace:Flags.Icons.Maui;assembly=Flags.Icons.MAUI">
    <flag:FlagIcon Kind="USSVG" WidthRequest="48" HeightRequest="36" />
</ContentPage>
```

### Aprillz.MewUI

```csharp
using Flags.Icons;
using Flags.Icons.MewUi;

var flag = FlagIcon.Create(FlagKind.USSVG, 48, 36);
var img  = new Image().Flag(FlagKind.FR2xPNG).Width(24).Height(18);
```

### Uno Platform

```xml
<Page xmlns:flag="using:Flags.Icons.Uno">
    <flag:FlagIcon Kind="USSVG" Width="48" Height="36" />
</Page>
```

### Windows Forms

```csharp
using Flags.Icons;
using Flags.Icons.WinForms;

var flag = new FlagIcon { Kind = FlagKind.USSVG, Width = 48, Height = 36 };
```

### WinUI 3

```xml
<Window xmlns:flag="using:Flags.Icons.WinUi">
    <flag:FlagIcon Kind="USSVG" Width="48" Height="36" />
</Window>
```

### WPF

```xml
<Window xmlns:flag="clr-namespace:Flags.Icons.WPF;assembly=Flags.Icons.WPF">
    <flag:FlagIcon Kind="USSVG" Width="48" Height="36" />
</Window>
```

### Runtime discovery

```csharp
using Flags.Icons;

foreach (var kind in FlagIcons.AvailableKinds) { /* "USSVG", "US2xPNG", ... */ }

FlagInfo info = FlagKindResolver.GetInfo(FlagKind.USSVG)!;
// info.Code = "US", info.Format = FlagFormat.Svg, info.ResourceName = "assets/SVG/US.svg"

using Stream? raw = FlagAssetLoader.OpenStream(FlagKind.USSVG);
```

## Demos

Each UI stack ships a demo that renders every `FlagKind` in a wrapping grid with country-code search, variant picker, and click-to-copy markup snippets.

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

FlagKit lives as a git submodule at `FlagKit/`. Clone with submodules:

```bash
git clone --recurse-submodules https://github.com/Alpaq92/Flags.Icons.git
```

…or after a plain clone:

```bash
git submodule update --init --recursive
```

The build embeds every PNG/SVG as a manifest resource and generates `FlagKind.g.cs` from the file list before `CoreCompile`. If `FlagKit/Assets/{PNG,SVG}` is empty, the generated enum will only contain `None = 0`.

## License

[MIT](LICENSE) for source code. The bundled flag assets retain FlagKit's [MIT license](FlagKit/LICENSE). The project icon (`icon.png`) is from the [Jellyfin UX icon set](https://github.com/jellyfin/jellyfin-ux), licensed under [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).

## Credits

- **Flag assets** — [madebybowtie/FlagKit](https://github.com/madebybowtie/FlagKit) (MIT)
- **Icon** — [Jellyfin UX](https://github.com/jellyfin/jellyfin-ux) (CC BY-SA 4.0)
- **Inspiration** — [Material.Icons.Avalonia](https://github.com/SKProCH/Material.Icons) — templated-control + markup-extension pattern adapted from this project

---

*Release history in [CHANGELOG.md](CHANGELOG.md). Versions are driven by [Conventional Commits](https://www.conventionalcommits.org/) via [release-please](https://github.com/googleapis/release-please); branch ruleset details in [`.github/rulesets/README.md`](.github/rulesets/README.md).*
