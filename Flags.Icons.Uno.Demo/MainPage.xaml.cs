using Flags.Demo.Shared;

namespace Flags.Icons.Uno.Demo;

public sealed partial class MainPage : Page {
    public MainPage() {
        InitializeComponent();
        ViewModel = new MainViewModel();
    }

    public MainViewModel ViewModel { get; }

    private void OnFlagClicked(object sender, RoutedEventArgs e) {
        if (sender is Button { Tag: FlagEntry entry }) {
            ViewModel.Select(entry);
        }
    }
}
