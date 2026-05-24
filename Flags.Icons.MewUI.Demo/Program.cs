using System;
using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Flags.Demo.Shared;
using Flags.Icons.MewUi;

namespace Flags.Icons.MewUi.Demo {
    /// <summary>
    /// Aprillz.MewUI sample demonstrating the Flags.Icons.MewUI helpers. Mirrors the UX of the
    /// other Flags.Icons.* demos: search box + variant ComboBox at the top, scrollable wrapping
    /// flag grid in the middle, copy-snippet TextBox at the bottom.
    /// </summary>
    public static class Program {
        private static WrapPanel _grid = null!;
        private static TextBox _snippet = null!;
        private static TextBox _search = null!;
        private static int _variantIndex;

        [STAThread]
        public static void Main() {
#if MEWUI_WIN
            Win32Platform.Register();
            Direct2DBackend.Register();
#elif MEWUI_LINUX
            X11Platform.Register();
            MewVGX11Backend.Register();
#elif MEWUI_MAC
            MacOSPlatform.Register();
            MewVGMacOSBackend.Register();
#endif

            _search = new TextBox().Placeholder("Search code");
            _search.OnTextChanged(_ => Rebuild());

            var variantBox = new ComboBox()
                .Items("SVG", "PNG @1x", "PNG @2x", "PNG @3x")
                .SelectedIndex(0)
                .OnSelectionChanged(item => {
                    _variantIndex = (item as string) switch {
                        "PNG @1x" => 1,
                        "PNG @2x" => 2,
                        "PNG @3x" => 3,
                        _ => 0,
                    };
                    Rebuild();
                });

            _grid = new WrapPanel { Spacing = 8, ItemWidth = 88 };

            _snippet = new TextBox().Placeholder("Click a flag to copy a C# snippet");

            var topBar = new DockPanel { LastChildFill = true, Spacing = 8 };
            DockPanel.SetDock(variantBox, Dock.Right);
            variantBox.Width(140);
            topBar.AddRange(variantBox, _search);

            var scroll = new ScrollViewer()
                .HorizontalScroll(ScrollMode.Disabled)
                .VerticalScroll(ScrollMode.Auto)
                .Content(_grid.CenterHorizontal());

            var root = new DockPanel { Spacing = 8, LastChildFill = true };
            DockPanel.SetDock(topBar, Dock.Top);
            DockPanel.SetDock(_snippet, Dock.Bottom);
            root.AddRange(topBar, _snippet, scroll);

            var window = new Window()
                .Title(typeof(Program).Assembly.GetName().Name!)
                .Resizable(820, 640, 480, 360, 4000, 3000)
                .Padding(8)
                .Content(root);
            window.Icon = IconSource.FromResource(typeof(Program).Assembly, "icon.ico");

            Rebuild();
            Application.Run(window);
        }

        private static void Rebuild() {
            _grid.Clear();
            var variant = FlagCatalog.AllVariants[_variantIndex];
            foreach (var entry in FlagCatalog.Filter(variant, _search.Text)) {
                _grid.Add(BuildTile(entry));
            }
        }

        private static Element BuildTile(FlagEntry entry) {
            var content = new StackPanel { Orientation = Orientation.Vertical, Spacing = 2 };
            content.Padding(4);
            content.AddRange(
                FlagIcon.Create(entry.Kind, 56, 42).CenterHorizontal(),
                new Label().Text(entry.Code).FontSize(11).CenterHorizontal());

            var btn = new Button().Content(content);
            btn.OnClick(() => _snippet.Text = $"new Image().Flag(FlagKind.{entry.Kind})");
            return btn;
        }
    }
}
