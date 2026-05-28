using System;
using System.IO;
using System.Runtime.InteropServices;
using Flags.Demo.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Flags.Icons.WinUi.Demo;

public sealed partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Title = GetType().Assembly.GetName().Name!;
        ViewModel = new MainViewModel();
        TrySetWindowIcon();
    }

    public MainViewModel ViewModel { get; }

    private void OnFlagClicked(object sender, RoutedEventArgs e) {
        if (sender is Button { Tag: FlagEntry entry }) {
            ViewModel.Select(entry);
        }
    }

    // AppWindow.SetIcon(string) is unreliable on unpackaged WinUI 3 (the WindowsAppSDK
    // path silently no-ops in several SDK revisions, leaving the default executable icon
    // showing in the title bar). Try it first for any host where it does land, then fall
    // back to a raw Win32 WM_SETICON SendMessage against the HWND — same trick as the
    // sibling Uno demo. Both ICON_SMALL (16×16, title bar) and ICON_BIG (32×32, taskbar
    // / Alt-Tab) are loaded directly from the multi-image .ico so each surface picks
    // its own LOD instead of bilinearly rescaling a single bitmap.
    private void TrySetWindowIcon() {
        var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
        if (!File.Exists(iconPath)) return;

        try { AppWindow?.SetIcon(iconPath); } catch { /* unpackaged WinUI 3 quirk */ }

        try {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            if (hwnd == IntPtr.Zero) return;
            var small = LoadImageW(IntPtr.Zero, iconPath, IMAGE_ICON, 16, 16, LR_LOADFROMFILE);
            var big = LoadImageW(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);
            if (small != IntPtr.Zero) SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_SMALL, small);
            if (big != IntPtr.Zero) SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, big);
        }
        catch { /* best-effort cosmetic; never block window load */ }
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
}
