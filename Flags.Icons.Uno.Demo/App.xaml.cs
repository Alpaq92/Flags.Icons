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
        try { window.AppWindow.SetIcon(iconPath); } catch { /* Uno desktop: SetIcon may throw on unpackaged hosts */ }
    }
}
