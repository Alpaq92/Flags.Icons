# Flags.Icons

[![Flags.Icons](https://img.shields.io/nuget/v/Flags.Icons.svg?label=Flags.Icons)](https://www.nuget.org/packages/Flags.Icons)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Alpaq92/Flags.Icons/blob/main/LICENSE)

![Flags.Icons demo](https://raw.githubusercontent.com/Alpaq92/Flags.Icons/main/flag-icons-demo.png)

Flag icons from **4 upstream sources** — Twemoji (262 country + subdivision emoji), Circle (430, HatScripts), Square (417, kapowaz), Lipis (271, lipis/flag-icons 4×3) — packaged as drop-in controls for Avalonia, Eto.Forms, .NET MAUI, Aprillz.MewUI, Uno Platform, Windows Forms, WinUI 3 and WPF. Every SVG ships as an embedded resource in the core `Flags.Icons` package — no runtime download, no file-system access.

## Install

```bash
dotnet add package Flags.Icons.Avalonia    # or .Eto / .MAUI / .MewUI / .Uno / .WinForms / .WinUi / .WPF
```

Every platform package transitively pulls in `Flags.Icons` core.

## Usage

One strongly-typed enum per source: `TwemojiFlag`, `CircleFlag`, `SquareFlag`, `LipisFlag`. `FlagIcon` exposes one DependencyProperty per source; set exactly one.

```xml
<flag:FlagIcon Twemoji="US" Width="48" Height="36" />
<flag:FlagIcon Circle="us" Width="48" Height="36" />
<flag:FlagIcon Square="us" Width="48" Height="36" />
<flag:FlagIcon Lipis="us" Width="48" Height="36" />
```

```csharp
var flag = new FlagIcon { Twemoji = TwemojiFlag.US };
```

Per-stack XAML namespaces and code-first usage examples in the [full README on GitHub →](https://github.com/Alpaq92/Flags.Icons#usage)

## Links

- 📖 [Full documentation](https://github.com/Alpaq92/Flags.Icons)
- 🐛 [Issues](https://github.com/Alpaq92/Flags.Icons/issues)
- 📝 [Changelog](https://github.com/Alpaq92/Flags.Icons/blob/main/CHANGELOG.md)

Source: [MIT](https://github.com/Alpaq92/Flags.Icons/blob/main/LICENSE). Bundled flag SVGs from [jdecked/twemoji](https://github.com/jdecked/twemoji) (graphics CC-BY 4.0, code MIT), [HatScripts/circle-flags](https://github.com/HatScripts/circle-flags) (MIT), [kapowaz/square-flags](https://github.com/kapowaz/square-flags) (MIT), [lipis/flag-icons](https://github.com/lipis/flag-icons) (MIT).
