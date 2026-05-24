using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace Flags.Icons.Avalonia.Demo.Views {
    public class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Title = GetType().Assembly.GetName().Name!;

            var isDebug = false;
            CheckIfDebug(ref isDebug);

            if (isDebug) this.AttachDevTools();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        [Conditional("DEBUG")]
        private static void CheckIfDebug(ref bool isDebug) => isDebug = true;
    }
}
