using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Flags.Demo.Shared;
using Flags.Icons;

namespace Flags.Icons.WinForms.Demo {
    public class MainForm : Form {
        private readonly TextBox _searchBox = new() {
            Dock = DockStyle.Fill,
            PlaceholderText = "Search code",
            Margin = new Padding(0, 0, 4, 0),
        };
        private readonly ComboBox _variantBox = new() {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0),
        };
        private readonly FlowLayoutPanel _grid = new() {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(4),
            Margin = new Padding(0, 4, 0, 4),
        };
        private readonly TextBox _snippetBox = new() {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            PlaceholderText = "Click a flag to copy a C# snippet",
            Margin = new Padding(0),
        };

        public MainForm() {
            Text = GetType().Assembly.GetName().Name!;
            ClientSize = new Size(960, 640);
            MinimumSize = new Size(480, 360);
            Icon = LoadAppIcon();

            foreach (var s in FlagCatalog.AllSources) _variantBox.Items.Add(s);
            _variantBox.SelectedIndex = 0;

            _searchBox.TextChanged += (_, _) => Rebuild();
            _variantBox.SelectedIndexChanged += (_, _) => Rebuild();
            _grid.Resize += (_, _) => CenterGridContent();

            // Top bar: search stretches, variant fixed-width on the right.
            var topBar = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0),
            };
            topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            topBar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            topBar.Controls.Add(_searchBox, 0, 0);
            topBar.Controls.Add(_variantBox, 1, 0);

            // 3-row root: top bar (auto), flag grid (fills), snippet TextBox (auto).
            // A plain Form + Dock-stack collapses the snippet because TextBox.PreferredSize feeds
            // into a Panel.AutoSize loop — TableLayoutPanel sidesteps that by sizing each row.
            var root = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(8),
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(topBar, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            root.Controls.Add(_snippetBox, 0, 2);

            Controls.Add(root);

            Rebuild();
        }

        private static Icon? LoadAppIcon() {
            using var stream = typeof(MainForm).Assembly.GetManifestResourceStream("Flags.Icons.WinForms.Demo.icon.ico");
            return stream == null ? null : new Icon(stream);
        }

        // Form.Icon (set in the ctor) handles the title-bar slot via WinForms' internal Win32
        // plumbing, but Windows 11's taskbar / Alt-Tab also reads ICON_BIG (32×32) explicitly —
        // and WinForms' single-icon assignment doesn't always surface that LOD to the shell
        // (especially on multi-image .ico files where the framework picks just one entry).
        // Re-set both ICON_SMALL and ICON_BIG via WM_SETICON once the HWND exists so every
        // surface picks its own LOD from the .ico directly.
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            TrySetWindowIconViaWin32();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadImageW(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x00000010;
        private const uint WM_SETICON = 0x0080;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;

        private void TrySetWindowIconViaWin32() {
            try {
                var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
                if (!File.Exists(iconPath)) return;
                var small = LoadImageW(IntPtr.Zero, iconPath, IMAGE_ICON, 16, 16, LR_LOADFROMFILE);
                var big = LoadImageW(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);
                if (small != IntPtr.Zero) SendMessage(Handle, WM_SETICON, (IntPtr)ICON_SMALL, small);
                if (big != IntPtr.Zero) SendMessage(Handle, WM_SETICON, (IntPtr)ICON_BIG, big);
            }
            catch { /* best-effort cosmetic; never block form load */ }
        }

        private void Rebuild() {
            _grid.SuspendLayout();
            try {
                // Snapshot before disposing — Control.Dispose() removes itself from the parent's
                // Controls collection, so iterating _grid.Controls directly mutates it mid-enumeration.
                var oldTiles = _grid.Controls.Cast<Control>().ToArray();
                _grid.Controls.Clear();
                foreach (var c in oldTiles) c.Dispose();

                var source = FlagCatalog.AllSources[Math.Max(0, _variantBox.SelectedIndex)];
                foreach (var section in FlagCatalog.Sections(source, _searchBox.Text)) {
                    _grid.Controls.Add(BuildSectionHeader(section.Title));
                    foreach (var entry in section.Entries) _grid.Controls.Add(BuildTile(entry));
                }
            } finally {
                _grid.ResumeLayout();
            }
            CenterGridContent();
        }

        // FlowLayoutPanel has no JustifyContent, so we center the row block by setting
        // symmetric horizontal padding based on how many tile-pitches fit in the viewport.
        // Equality-guarded so re-setting the same padding doesn't re-fire layout events.
        private void CenterGridContent() {
            if (_grid.Controls.Count == 0) return;
            var sample = _grid.Controls[0];
            int tilePitch = sample.Width + sample.Margin.Horizontal;
            if (tilePitch <= 0) return;

            int viewportWidth = _grid.ClientSize.Width;
            if (_grid.VerticalScroll.Visible) viewportWidth -= SystemInformation.VerticalScrollBarWidth;
            int tilesPerRow = Math.Max(1, viewportWidth / tilePitch);
            int contentWidth = tilesPerRow * tilePitch;
            int sidePad = Math.Max(0, (viewportWidth - contentWidth) / 2);

            if (_grid.Padding.Left != sidePad || _grid.Padding.Right != sidePad) {
                _grid.Padding = new Padding(sidePad, _grid.Padding.Top, sidePad, _grid.Padding.Bottom);
            }
        }

        private Control BuildSectionHeader(string title) {
            // Section header spans the FlowLayoutPanel width by SetFlowBreak, plus has flowed siblings.
            var lbl = new Label {
                Text = title,
                AutoSize = true,
                Font = new Font(Font.FontFamily, 11f, FontStyle.Bold),
                Margin = new Padding(4, 12, 4, 4),
            };
            _grid.SetFlowBreak(lbl, true);
            return lbl;
        }

        private Control BuildTile(FlagEntry entry) {
            var icon = new Flags.Icons.WinForms.FlagIcon {
                Width = 56,
                Height = 42,
                Anchor = AnchorStyles.None,
            };
            ApplyEntry(icon, entry);
            var label = new Label {
                Text = entry.Code,
                AutoSize = false,
                Width = 80,
                Height = 16,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(Font.FontFamily, 8.5f),
            };
            var tile = new TableLayoutPanel {
                ColumnCount = 1,
                RowCount = 2,
                Width = 80,
                Height = 64,
                Margin = new Padding(2),
                Cursor = Cursors.Hand,
            };
            tile.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            tile.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tile.Controls.Add(icon, 0, 0);
            tile.Controls.Add(label, 0, 1);

            // Make the whole tile clickable — propagate clicks from children up to the tile.
            void OnClick(object? sender, EventArgs e) =>
                _snippetBox.Text = $"new FlagIcon {{ {entry.Source} = {entry.Snippet} }}";
            tile.Click += OnClick;
            icon.Click += OnClick;
            label.Click += OnClick;

            return tile;
        }

        private static void ApplyEntry(Flags.Icons.WinForms.FlagIcon icon, FlagEntry entry) {
            switch (entry.Source) {
                case FlagSource.Twemoji: icon.Twemoji = entry.Twemoji; break;
                case FlagSource.Circle: icon.Circle = entry.Circle; break;
                case FlagSource.Square: icon.Square = entry.Square; break;
                case FlagSource.Lipis: icon.Lipis = entry.Lipis; break;
                case FlagSource.FlagHub: icon.FlagHub = entry.FlagHub; break;
            }
        }
    }
}
