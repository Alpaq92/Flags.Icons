# Flags.Icons

[![Flags.Icons](https://img.shields.io/nuget/v/Flags.Icons.svg?label=Flags.Icons)](https://www.nuget.org/packages/Flags.Icons)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Alpaq92/Flags.Icons/blob/main/LICENSE)

![Flags.Icons demo](https://raw.githubusercontent.com/Alpaq92/Flags.Icons/main/flag-icons-demo.png)

Country flag icons from [madebybowtie/FlagKit](https://github.com/madebybowtie/FlagKit), packaged as drop-in controls for Avalonia, Eto.Forms, .NET MAUI, Aprillz.MewUI, Uno Platform, Windows Forms, WinUI 3 and WPF. **~1020 assets across 255 country codes** ship as embedded resources in the core `Flags.Icons` package — no runtime download, no file-system access.

## Install

```bash
dotnet add package Flags.Icons.Avalonia    # or .Eto / .MAUI / .MewUI / .Uno / .WinForms / .WinUi / .WPF
```

Every platform package transitively pulls in `Flags.Icons` core.

## Usage

`FlagKind` members encode the country code, scale (for PNGs), and format — `USSVG`, `US2xPNG`, `GB_ENG2xPNG`, etc.

```xml
<flag:FlagIcon Kind="USSVG" Width="48" Height="36" />
```

```csharp
var flag = new FlagIcon { Kind = FlagKind.USSVG };
```

Per-stack XAML namespaces and code-first usage examples in the [full README on GitHub →](https://github.com/Alpaq92/Flags.Icons#usage)

## Links

- 📖 [Full documentation](https://github.com/Alpaq92/Flags.Icons)
- 🐛 [Issues](https://github.com/Alpaq92/Flags.Icons/issues)
- 📝 [Changelog](https://github.com/Alpaq92/Flags.Icons/blob/main/CHANGELOG.md)

Source: [MIT](https://github.com/Alpaq92/Flags.Icons/blob/main/LICENSE). Flag assets retain FlagKit's [MIT license](https://github.com/madebybowtie/FlagKit/blob/master/LICENSE).
