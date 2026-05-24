using System;
using Eto.Drawing;
using Eto.Forms;
using Flags.Demo.Shared;
using Flags.Icons.Eto;

namespace Flags.Icons.Eto.Demo {
    public class MainForm : Form {
        private const int Columns = 10;

        private readonly TextBox _searchBox = new TextBox { PlaceholderText = "Search code" };
        private readonly DropDown _variantDrop = new DropDown {
            Items = { "SVG", "PNG @1x", "PNG @2x", "PNG @3x" },
            SelectedIndex = 0,
        };
        private readonly Scrollable _gridScroll = new Scrollable { ExpandContentWidth = true };
        private readonly TextBox _snippetBox = new TextBox {
            PlaceholderText = "Click a flag to copy a C# snippet",
            ReadOnly = true,
        };

        public MainForm() {
            Title = GetType().Assembly.GetName().Name!;
            ClientSize = new Size(960, 640);
            Icon = Icon.FromResource("Flags.Icons.Eto.Demo.icon.ico", typeof(MainForm).Assembly);

            _searchBox.TextChanged += (_, _) => Rebuild();
            _variantDrop.SelectedIndexChanged += (_, _) => Rebuild();

            var topBar = new StackLayout {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items = {
                    new StackLayoutItem(_searchBox, expand: true),
                    new StackLayoutItem(_variantDrop, expand: false),
                },
            };

            Content = new StackLayout {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Padding = 8,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items = {
                    topBar,
                    new StackLayoutItem(_gridScroll, expand: true),
                    _snippetBox,
                },
            };

            Rebuild();
        }

        private void Rebuild() {
            var variant = FlagCatalog.AllVariants[Math.Max(0, _variantDrop.SelectedIndex)];
            var filtered = FlagCatalog.Filter(variant, _searchBox.Text);

            var rows = Math.Max(1, (filtered.Count + Columns - 1) / Columns);
            var inner = new TableLayout(Columns, rows) { Spacing = new Size(4, 4) };
            for (int i = 0; i < filtered.Count; i++) {
                inner.Add(BuildTile(filtered[i]), i % Columns, i / Columns);
            }
            // TableLayout(int, int) auto-scales the last column/row to fill — undo that so the
            // grid shrinks to its content size.
            for (int c = 0; c < Columns; c++) inner.SetColumnScale(c, false);
            for (int r = 0; r < rows; r++) inner.SetRowScale(r, false);

            // Center the grid horizontally by wrapping it between two scaling spacer cells.
            _gridScroll.Content = new TableLayout(new TableRow(
                new TableCell(null, scaleWidth: true),
                new TableCell(inner),
                new TableCell(null, scaleWidth: true))) {
                Padding = new Padding(4),
            };
        }

        private Control BuildTile(FlagEntry entry) {
            var icon = new FlagIcon { Kind = entry.Kind, Size = new Size(56, 42) };
            var label = new Label { Text = entry.Code, TextAlignment = TextAlignment.Center, Font = SystemFonts.Default(9) };

            var stack = new StackLayout {
                Orientation = Orientation.Vertical,
                Spacing = 2,
                Padding = new Padding(4),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Items = { icon, label },
            };

            var panel = new Panel { Content = stack, Width = 80 };
            panel.MouseDown += (_, _) => _snippetBox.Text = $"<flag:FlagIcon Kind=\"{entry.Kind}\" Size=\"56,42\" />";
            return panel;
        }
    }
}
