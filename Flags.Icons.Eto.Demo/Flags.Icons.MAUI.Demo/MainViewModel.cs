using System.Windows.Input;
using Flags.Demo.Shared;
using Microsoft.Maui.Controls;

namespace Flags.Icons.Maui.Demo;

public class MainViewModel : DemoViewModel {
    public MainViewModel() {
        SelectCommand = new Command<FlagEntry>(Select);
    }

    public ICommand SelectCommand { get; }
}
