using System;
using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Flags.Demo.Shared;
using Flags.Icons;
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

            var sourceLabels = new System.Collections.Generic.List<string>();
            foreach (var s in FlagCatalog.AllSources) sourceLabels.Add(s.ToString());
            var variantBox = new ComboBox()
                .Items(sourceLabels.ToArray())
                .SelectedIndex(0)
                .OnSelectionChanged(item => {
                    var idx = sourceLabels.IndexOf(item as string ?? "");
                    _variantIndex = idx < 0 ? 0 : idx;
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
            var source = FlagCatalog.AllSources[_variantIndex];
            foreach (var section in FlagCatalog.Sections(source, _search.Text)) {
                if (section.Entries.Count == 0) continue;
                var header = new Label().Text(section.Title).FontSize(14);
                // Force the header onto its own row by setting it to a large width.
                _grid.Add(header);
                foreach (var entry in section.Entries) _grid.Add(BuildTile(entry));
            }
        }

        private static Element BuildTile(FlagEntry entry) {
            var content = new StackPanel { Orientation = Orientation.Vertical, Spacing = 2 };
            content.Padding(4);
            content.AddRange(
                BuildFlagImage(entry).CenterHorizontal(),
                new Label().Text(entry.Code).FontSize(11).CenterHorizontal());

            var btn = new Button().Content(content);
            btn.OnClick(() => _snippet.Text = $"new Image().Flag({entry.Snippet})");
            return btn;
        }

        private static Image BuildFlagImage(FlagEntry entry) => entry.Source switch {
            FlagSource.Twemoji => FlagIcon.Create(entry.Twemoji, 56, 42),
            FlagSource.Circle => FlagIcon.Create(entry.Circle, 56, 42),
            FlagSource.Square => FlagIcon.Create(entry.Square, 56, 42),
            FlagSource.Lipis => FlagIcon.Create(entry.Lipis, 56, 42),
            FlagSource.FlagHub => FlagIcon.Create(entry.FlagHub, 56, 42),
            _ => FlagIcon.Create(entry.Twemoji, 56, 42),
        };
    }
}
