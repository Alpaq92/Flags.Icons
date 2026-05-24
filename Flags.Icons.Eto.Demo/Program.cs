using System;
using System.Runtime.InteropServices;
using Eto;
using Eto.Forms;

namespace Flags.Icons.Eto.Demo {
    public static class Program {
        [STAThread]
        public static void Main() {
            new Application(SelectPlatform()).Run(new MainForm());
        }

        // Platforms.* are just string identifiers — Eto resolves the matching
        // Eto.Platform.* assembly (referenced conditionally in the csproj) at runtime.
        private static string SelectPlatform() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Platforms.Wpf;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return Platforms.Gtk;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return Platforms.Mac64;
            return Platforms.Wpf;
        }
    }
}
