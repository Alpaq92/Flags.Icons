using System.Runtime.InteropServices;

namespace Flags.Icons.Uno.Demo;

public partial class App : Application {
    public App() => InitializeComponent();

    protected Window? MainWindow { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args) {
        MainWindow = new Window { Title = GetType().Assembly.GetName().Name! };

        TrySetWindowIcon(MainWindow);

        if (MainWindow.Content is not Frame rootFrame) {
            rootFrame = new Frame();
            MainWindow.Content = rootFrame;
            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (rootFrame.Content == null) {
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        MainWindow.Activate();
    }

    private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }

    private static void TrySetWindowIcon(Window window) {
        var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
        if (!File.Exists(iconPath)) return;

        // AppWindow.SetIcon throws on Uno's Skia-desktop host (the Win32 backend doesn't
        // wire it up to the title-bar HICON), so the title bar silently keeps Uno's default
        // globe glyph. Try it first for any future Uno release that does implement it, then
        // fall back to a raw Win32 WM_SETICON SendMessage on Windows.
        try { window.AppWindow?.SetIcon(iconPath); } catch { /* expected on Skia-desktop */ }

        if (OperatingSystem.IsWindows()) {
            TrySetWin32WindowIcon(window, iconPath);
        }
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

    private static void TrySetWin32WindowIcon(Window window, string iconPath) {
        try {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            if (hwnd == IntPtr.Zero) return;

            // Load the title-bar (16×16) and taskbar/Alt-Tab (32×32) sizes from the multi-image
            // .ico so Windows picks the right LOD without bilinear-rescaling a single bitmap.
            var small = LoadImageW(IntPtr.Zero, iconPath, IMAGE_ICON, 16, 16, LR_LOADFROMFILE);
            var big = LoadImageW(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);

            if (small != IntPtr.Zero) SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_SMALL, small);
            if (big != IntPtr.Zero) SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, big);
        }
        catch { /* best-effort cosmetic; never block app launch */ }
    }
}
