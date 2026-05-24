using System.Windows;
using System.Windows.Controls;
using Flags.Demo.Shared;

namespace Flags.Icons.WPF.Demo {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Title = GetType().Assembly.GetName().Name!;
        }

        private void OnFlagClick(object sender, RoutedEventArgs e) {
            if (sender is Button { Tag: FlagEntry entry } && DataContext is MainViewModel vm) {
                vm.Selected = entry;
            }
        }
    }
}
