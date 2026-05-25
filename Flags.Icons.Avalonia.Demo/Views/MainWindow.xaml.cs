using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flags.Icons.Avalonia.Demo.Views {
    public class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Title = GetType().Assembly.GetName().Name!;
            // AttachDevTools removed alongside Avalonia.Diagnostics — re-wire once a 12.x build of
            // the diagnostics package ships (see Flags.Icons.Avalonia.Demo.csproj for context).
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
