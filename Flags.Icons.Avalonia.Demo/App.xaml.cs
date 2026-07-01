using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flags.Icons.Avalonia.Demo.ViewModels;
using Flags.Icons.Avalonia.Demo.Views;

namespace Flags.Icons.Avalonia.Demo {
    public class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            var viewModel = new MainWindowViewModel();

            switch (ApplicationLifetime) {
                // Desktop head: a window hosting the shared MainView.
                case IClassicDesktopStyleApplicationLifetime desktop:
                    desktop.MainWindow = new MainWindow { DataContext = viewModel };
                    break;
                // Browser (WASM) head: MainView is the top level itself, no window.
                case ISingleViewApplicationLifetime singleView:
                    singleView.MainView = new MainView { DataContext = viewModel };
                    break;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
