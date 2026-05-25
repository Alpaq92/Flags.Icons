using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Flags.Demo.Shared;
using Flags.Icons;

namespace Flags.Icons.Eto.Demo {
    public class MainForm : Form {
        private const int Columns = 10;

        private readonly TextBox _searchBox = new TextBox { PlaceholderText = "Search code" };
        private readonly DropDown _variantDrop = new DropDown { SelectedIndex = 0 };
        private readonly Scrollable _gridScroll = new Scrollable { ExpandContentWidth = true };
        private readonly TextBox _snippetBox = new TextBox {
            PlaceholderText = "Click a flag to copy a C# snippet",
            ReadOnly = true,
        };

        public MainForm() {
            Title = GetType().Assembly.GetName().Name!;
            ClientSize = new Size(960, 640);
            Icon = Icon.FromResource("Flags.Icons.Eto.Demo.icon.ico", typeof(MainForm).Assembly);

            foreach (var s in FlagCatalog.AllSources) _variantDrop.Items.Add(s.ToString());
            _variantDrop.SelectedIndex = 0;

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
            var source = FlagCatalog.AllSources[Math.Max(0, _variantDrop.SelectedIndex)];
            var sections = FlagCatalog.Sections(source, _searchBox.Text);

            // One vertical stack of section headers + per-section grids.
            var outer = new StackLayout {
                Orientation = Orientation.Vertical,
                Spacing = 12,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };
            foreach (var section in sections) {
                if (section.Entries.Count == 0) continue;
                outer.Items.Add(new Label {
                    Text = section.Title,
                    Font = SystemFonts.Bold(11),
                });

                var rows = Math.Max(1, (section.Entries.Count + Columns - 1) / Columns);
                var inner = new TableLayout(Columns, rows) { Spacing = new Size(4, 4) };
                for (int i = 0; i < section.Entries.Count; i++) {
                    inner.Add(BuildTile(section.Entries[i]), i % Columns, i / Columns);
                }
                for (int c = 0; c < Columns; c++) inner.SetColumnScale(c, false);
                for (int r = 0; r < rows; r++) inner.SetRowScale(r, false);
                outer.Items.Add(inner);
            }

            _gridScroll.Content = new TableLayout(new TableRow(
                new TableCell(null, scaleWidth: true),
                new TableCell(outer),
                new TableCell(null, scaleWidth: true))) {
                Padding = new Padding(4),
            };
        }

        private Control BuildTile(FlagEntry entry) {
            var icon = new Flags.Icons.Eto.FlagIcon { Size = new Size(56, 42) };
            ApplyEntry(icon, entry);
            var label = new Label { Text = entry.Code, TextAlignment = TextAlignment.Center, Font = SystemFonts.Default(9) };

            var stack = new StackLayout {
                Orientation = Orientation.Vertical,
                Spacing = 2,
                Padding = new Padding(4),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Items = { icon, label },
            };

            var panel = new Panel { Content = stack, Width = 80 };
            panel.MouseDown += (_, _) =>
                _snippetBox.Text = $"<flag:FlagIcon {entry.Source}=\"{entry.Code}\" Size=\"56,42\" />";
            return panel;
        }

        private static void ApplyEntry(Flags.Icons.Eto.FlagIcon icon, FlagEntry entry) {
            switch (entry.Source) {
                case FlagSource.Twemoji: icon.Twemoji = entry.Twemoji; break;
                case FlagSource.Circle: icon.Circle = entry.Circle; break;
                case FlagSource.Square: icon.Square = entry.Square; break;
                case FlagSource.Lipis: icon.Lipis = entry.Lipis; break;
            }
        }
    }
}
