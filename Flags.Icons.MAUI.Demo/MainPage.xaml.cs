using Microsoft.Maui.Controls;

namespace Flags.Icons.Maui.Demo;

public partial class MainPage : ContentPage {
    public MainPage() {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}
