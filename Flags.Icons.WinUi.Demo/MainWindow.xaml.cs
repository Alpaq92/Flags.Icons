using System;
using System.IO;
using Flags.Demo.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Flags.Icons.WinUi.Demo;

public sealed partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Title = GetType().Assembly.GetName().Name!;
        ViewModel = new MainViewModel();
        var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
        if (File.Exists(iconPath)) {
            AppWindow.SetIcon(iconPath);
        }
    }

    public MainViewModel ViewModel { get; }

    private void OnFlagClicked(object sender, RoutedEventArgs e) {
        if (sender is Button { Tag: FlagEntry entry }) {
            ViewModel.Select(entry);
        }
    }
}
