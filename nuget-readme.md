# Flags.Icons

![Flags.Icons demo](https://raw.githubusercontent.com/Alpaq92/Flags.Icons/main/flag-icons-demo.png)

Flag icons from **5 upstream sources** — Twemoji (262 country + subdivision emoji), Circle (430, HatScripts), Square (417, kapowaz), Lipis (271, lipis/flag-icons 4×3), FlagHub (255, Alpaq92's maintained fork of madebybowtie/FlagKit) — packaged as drop-in controls for Avalonia, Eto.Forms, .NET MAUI, Aprillz.MewUI, Uno Platform, Windows Forms, WinUI 3 and WPF. Every SVG ships as an embedded resource in the core `Flags.Icons` package — no runtime download, no file-system access.

> *Counts as of 2026-06-01; refreshed monthly via the upstream-bump workflow, so any given release may carry slightly different totals — the actual numbers print in the build log.*

> **v3:** the FlagKit-style artwork from v1 is back, sourced from the maintained [Alpaq92/FlagHub](https://github.com/Alpaq92/FlagHub) fork and surfaced as a fifth source (`FlagHub="US"` / `FlagHubFlag.US`). Additive on top of v2, still breaking vs v1 (no `FlagKind` enum — use one of the five typed source properties).

## Install

```bash
dotnet add package Flags.Icons.Avalonia    # or .Eto / .MAUI / .MewUI / .Uno / .WinForms / .WinUi / .WPF
```

Every platform package transitively pulls in `Flags.Icons` core.

## Usage

One strongly-typed enum per source: `TwemojiFlag`, `CircleFlag`, `SquareFlag`, `LipisFlag`, `FlagHubFlag`. `FlagIcon` exposes one property per source (DependencyProperty / BindableProperty / StyledProperty / plain CLR property, depending on the UI stack); set exactly one and the others auto-clear.

```xml
<flag:FlagIcon Twemoji="US" Width="48" Height="36" />
<flag:FlagIcon Circle="US"  Width="48" Height="36" />
<flag:FlagIcon Square="US"  Width="48" Height="36" />
<flag:FlagIcon Lipis="US"   Width="48" Height="36" />
<flag:FlagIcon FlagHub="US" Width="48" Height="36" />
```

```csharp
var flag = new FlagIcon { Twemoji = TwemojiFlag.US };
```

Per-stack XAML namespaces and code-first usage examples in the [full README on GitHub →](https://github.com/Alpaq92/Flags.Icons#usage)

## Links

- 📖 [Full documentation](https://github.com/Alpaq92/Flags.Icons)
- 🐛 [Issues](https://github.com/Alpaq92/Flags.Icons/issues)
- 📝 [Changelog](https://github.com/Alpaq92/Flags.Icons/blob/main/CHANGELOG.md)

Source: [MIT](https://github.com/Alpaq92/Flags.Icons/blob/main/LICENSE). Bundled flag SVGs from [jdecked/twemoji](https://github.com/jdecked/twemoji) (graphics CC-BY 4.0, code MIT), [HatScripts/circle-flags](https://github.com/HatScripts/circle-flags) (MIT), [kapowaz/square-flags](https://github.com/kapowaz/square-flags) (MIT), [lipis/flag-icons](https://github.com/lipis/flag-icons) (MIT), [Alpaq92/FlagHub](https://github.com/Alpaq92/FlagHub) (MIT, FlagKit fork).
