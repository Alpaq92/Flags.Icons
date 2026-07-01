using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;

[assembly: SupportedOSPlatform("browser")]

namespace Flags.Icons.Avalonia.Demo.Browser {
    internal class Program {
        private static Task Main(string[] args) => BuildAvaloniaApp()
            .StartBrowserAppAsync("out");

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>();
    }
}
