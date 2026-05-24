using System;
using Uno.UI.Hosting;

namespace Flags.Icons.Uno.Demo;

internal static class Program {
    [STAThread]
    public static void Main(string[] args) {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
