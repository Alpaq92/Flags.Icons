using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace Flags.Icons.Maui.Demo;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

#if WINDOWS
        builder.ConfigureLifecycleEvents(events => {
            events.AddWindows(windows => windows.OnWindowCreated(window => {
                var iconPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "icon.ico");
                if (System.IO.File.Exists(iconPath)) {
                    window.AppWindow.SetIcon(iconPath);
                }
            }));
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
