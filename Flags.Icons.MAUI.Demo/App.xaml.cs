using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Flags.Icons.Maui.Demo;

public partial class App : Application {
    public App() {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new Window(new MainPage()) { Title = GetType().Assembly.GetName().Name! };
}
